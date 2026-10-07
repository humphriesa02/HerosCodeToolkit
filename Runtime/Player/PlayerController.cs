using UnityEngine;
using UnityEngine.InputSystem;
using HerosCode.Toolkit.Core;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// A generic player controller that utilizes a state machine
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Control")]
        [SerializeField] private PlayerValues playerValues;

        [Header("References")]
        public Transform focus;
        public Animator animator;

        // Static Values
        [HideInInspector] public CharacterController controller;
        [HideInInspector] public PlayerInput playerInput;
        [HideInInspector] public Vector3 playerVelocity;

        // Context
        private PlayerContext context;

        // Sensors
        private PlayerInputSensor playerInputSensor;
        private PlayerWorldSensor playerWorldSensor;

        // Drivers
        private PlayerMovementDriver playerMovementDriver;
        private PlayerAnimationDriver playerAnimationDriver;
        private PlayerActionDriver playerActionDriver;

        // State Machine
        private StateMachine movementSM;
        public GroundState groundState;
        public AirState airState;

        // Actions
        public JumpAction jumpAction;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            if (animator == null)
            {
                // Assumes "visual" is a child
                animator = GetComponentInChildren<Animator>();
            }
            
            // Context
            context = new();

            // Sensors
            playerInputSensor = new(playerInput, focus);
            playerWorldSensor = new(controller);

            // Motors
            playerMovementDriver = new(controller, playerValues);
            playerAnimationDriver = new(animator);
            playerActionDriver = new();

            // Locomotion State
            movementSM = new StateMachine();
            groundState = new GroundState(this, movementSM, playerValues, context);
            airState = new AirState(this, movementSM, playerValues, context);

            // Actions
            jumpAction = new(playerValues);

            // Action binding
            // TODO - this should be done via inspector?
            playerActionDriver.Bind(Keybind.Primary, jumpAction);
        }

        void Start()
        {
            if (focus == null)
            {
                focus = Camera.main.transform;
            }

            movementSM.Initialize(groundState);
        }

        void Update()
        {
            context.ResetFrame();

            // Set the context via sensors
            playerInputSensor.CollectData(context);
            playerWorldSensor.CollectData(context);

            // Action driver
            playerActionDriver.Apply(context);

            // Locomotion state machine
            movementSM.LogicUpdate();

            playerMovementDriver.Apply(context);
            playerAnimationDriver.Apply(context);
        }

        void LateUpdate()
        {
            movementSM.LateUpdate();
        }

        void FixedUpdate()
        {
            movementSM.PhysicsUpdate();
        }

        void OnGUI()
        {
            GUI.Label(new Rect(15, 15, 300, 100), movementSM.GetCurrentState().ToString());
        }
    }
}