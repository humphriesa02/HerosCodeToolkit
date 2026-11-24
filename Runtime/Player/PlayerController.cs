using UnityEngine;
using HerosCode.Toolkit.Cameras;

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
        [SerializeField, Tooltip("The values that dictate how the player works.")]
        private PlayerValues playerValues;

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
        private PlayerRollingVisualExecutor playerRollingVisualExecutor;

        [Header("References")]
        // Non-player related (but controlled by player)
        [SerializeField] private OrbitCamera playerCamera; // For player controller camera
        private bool playerControlledCamera = false;
        [Tooltip("The space that defines which way forward will be when walking. I.e., camera's forward. Can be set dynamically")]
        public Transform playerInputSpace = default;

        private void Awake()
        {
            // Context
            context = new();

            // Sensors
            playerInputSensor = GetComponent<PlayerInputSensor>();
            playerInputSensor.Initialize(context, playerValues);
            playerWorldSensor = GetComponent<PlayerWorldSensor>();
            playerWorldSensor.Initialize(context, playerValues);

            // Updaters
            playerActionUpdater = new(context, playerValues);
            playerLocomotionStateUpdater = new(context, playerValues);

            // Executors
            playerAnimationExecutor = GetComponent<PlayerAnimationExecutor>();
            playerAnimationExecutor.Initialize(context, playerValues);

            playerMovementExecutor = GetComponent<PlayerMovementExecutor>();
            playerMovementExecutor.Initialize(context, playerValues);

            playerRollingVisualExecutor = GetComponent<PlayerRollingVisualExecutor>();
            playerRollingVisualExecutor.Initialize(context, playerValues);

            if (playerCamera != null) playerControlledCamera = true;
        }

        private void Update()
        {
            context.ResetFrame();

            // Update context via PlayerController specifically
            UpdateReferences();

            // Set the context via sensors
            playerInputSensor.CollectData(context);

            // Action updater determines player intent
            playerActionUpdater.Tick(context);

            // Locomotion updater determines locomotion modifiers
            playerLocomotionStateUpdater.Tick(context);

            // Execute animation updates
            playerAnimationExecutor.Execute(context);
        }

        private void LateUpdate()
        {
            playerLocomotionStateUpdater.LateTick(context);

            // Apply rotations to player if player controlled camera
            if (playerControlledCamera) playerCamera.UpdateCamera(context.lookInput, context.lookViaMouse);
        }

        private void FixedUpdate()
        {
            playerWorldSensor.FixedCollectData(context);
            playerLocomotionStateUpdater.FixedTick(context);
            playerMovementExecutor.Execute(context);
            playerRollingVisualExecutor.Execute(context);
            playerWorldSensor.ClearData(context);
        }

        private void UpdateReferences()
        {
            if (playerInputSpace != null)
            {
                context.playerInputSpace = playerInputSpace;
            }
        }

        // TODO - dedicated debug component
        private void OnGUI()
        {
            GUI.Label(new Rect(25, 25, 300, 100), playerLocomotionStateUpdater.GetCurrentState().ToString());
        }
    }
}
