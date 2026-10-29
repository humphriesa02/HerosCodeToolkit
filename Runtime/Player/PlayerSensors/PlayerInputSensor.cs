using UnityEngine;
using UnityEngine.InputSystem;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Collect information from the player -
    /// populate <see cref="PlayerContext"/>
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputSensor : MonoBehaviour, IPlayerSensor
    {
        /// Input
        private PlayerInput playerInput;
        [SerializeField] private Transform playerFocus;
        
        private InputAction moveAction; // Movement
        private InputAction lookAction; // For camera, mainly 
        private InputAction primaryButtonAction;
        private InputAction secondaryButtonAction;

        private const float MaxPressAge = 999f;

        public void Initialize(PlayerContext context, PlayerValues values)
        {
            if (playerFocus == null)
            {
                playerFocus = UnityEngine.Camera.main.transform;
            }
            playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            primaryButtonAction = playerInput.actions["Primary"];
            secondaryButtonAction = playerInput.actions["Secondary"];
        }

        public void CollectData(PlayerContext context)
        {
            context.moveInput = moveAction.ReadValue<Vector2>();

            // Look uses mouse if no controllers are connected
            // TODO - unify this in settings choices (mouse v keyboard)
            context.lookInput = lookAction.ReadValue<Vector2>();
            context.lookViaMouse = lookAction.activeControl?.device is Mouse;

            context.primaryPressedTime   = primaryButtonAction.WasPressedThisFrame()   ? 0f : Mathf.Min(context.primaryPressedTime + Time.deltaTime, MaxPressAge);
            context.secondaryPressedTime = secondaryButtonAction.WasPressedThisFrame() ? 0f : Mathf.Min(context.secondaryPressedTime + Time.deltaTime, MaxPressAge);
            
            Vector3 right = playerFocus.right;
            right.y = 0f;
            right.Normalize();

            Vector3 forward = playerFocus.forward;
            forward.y = 0f;
            forward.Normalize();

            context.moveDirection = right * context.moveInput.x + forward * context.moveInput.y;
        }

        public void ClearData(PlayerContext context) { }

        public void FixedCollectData(PlayerContext context){ }
    }
}