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
        private PlayerLocomotionStateDriver playerLocomotionStateDriver;

        // State Machine
        private StateMachine movementSM;
        public GroundState groundState;
        public AirState airState;

        // Actions
        public JumpAction jumpAction;
        public DiveAction diveAction;

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

            // Locomotion State
            movementSM = new StateMachine();
            groundState = new(playerValues, context);
            airState = new(playerValues, context);

            // Motors
            playerMovementDriver = new(controller, playerValues);
            playerAnimationDriver = new(animator);
            playerActionDriver = new();
            playerLocomotionStateDriver = new(movementSM, groundState, airState);

            // Actions
            jumpAction = new(playerValues);
            diveAction = new(playerValues);

            // Action binding
            // TODO - this should be done via inspector?
            playerActionDriver.Bind(Keybind.Primary, jumpAction);
            playerActionDriver.Bind(Keybind.Secondary, diveAction);
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

            // Determine locomotion
            playerLocomotionStateDriver.Apply(context);

            // Locomotion state machine
            movementSM.LogicUpdate();

            // Apply intent
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