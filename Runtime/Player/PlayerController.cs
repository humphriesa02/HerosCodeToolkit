using UnityEngine;

namespace HerosCode.Toolkit.Player
{
     /// <summary>
    /// A generic player controller that utilizes a state machine
    /// </summary>
    [RequireComponent(typeof(PlayerInputSensor))]
    [RequireComponent(typeof(PlayerWorldSensor))]
    [RequireComponent(typeof(PlayerAnimationExecutor))]
    [RequireComponent(typeof(PlayerMovementExecutor))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Control")]
        [SerializeField] private PlayerValues playerValues;

        // Context
        private PlayerContext context;

        // Sensors -> Read outside data
        private PlayerInputSensor playerInputSensor;
        private PlayerWorldSensor playerWorldSensor;

        // Updaters -> Update internal data
        private PlayerActionUpdater playerActionUpdater;
        private PlayerLocomotionStateUpdater playerLocomotionStateUpdater;

        // Executors -> Execute on the internal data
        private PlayerMovementExecutor playerMovementExecutor;
        private PlayerAnimationExecutor playerAnimationExecutor;
        

        void Awake()
        {
            // Context
            context = new();

            // Sensors
            playerInputSensor = GetComponent<PlayerInputSensor>();
            playerWorldSensor = GetComponent<PlayerWorldSensor>();

            // Updaters
            playerActionUpdater = new(context, playerValues);
            playerLocomotionStateUpdater = new(context, playerValues);

            // Executors
            playerAnimationExecutor = GetComponent<PlayerAnimationExecutor>();
            playerAnimationExecutor.Initialize(context, playerValues);
            
            playerMovementExecutor = GetComponent<PlayerMovementExecutor>();
            playerMovementExecutor.Initialize(context, playerValues);
        }

        void Update()
        {
            context.ResetFrame();

            // Set the context via sensors
            playerInputSensor.CollectData(context);
            playerWorldSensor.CollectData(context);

            // Action updater determines player intent
            playerActionUpdater.Tick(context);

            // Locomotion updater determines locomotion modifiers
            playerLocomotionStateUpdater.Tick(context);

            // Execute animation updates
            playerAnimationExecutor.Execute(context);
        }

        void LateUpdate()
        {
            playerLocomotionStateUpdater.LateTick(context);
        }

        void FixedUpdate()
        {
            playerLocomotionStateUpdater.FixedTick(context);

            // Ensures movement is always physics driven. Design choice
            playerMovementExecutor.Execute(context);
        }

        // TODO - dedicated debug component
        void OnGUI()
        {
            GUI.Label(new Rect(25, 25, 300, 100), playerLocomotionStateUpdater.GetCurrentState().ToString());
        }
    }
}