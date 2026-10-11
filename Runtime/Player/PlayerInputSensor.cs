using UnityEngine;
using UnityEngine.InputSystem;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Collect information from the player -
    /// populate <see cref="PlayerContext"/>
    /// </summary>
    public class PlayerInputSensor : IPlayerSensor
    {
        /// Input
        private readonly PlayerInput playerInput;
        private readonly Transform playerFocus;
        
        protected InputAction moveAction; // Movement
        protected InputAction primaryButtonAction;
        protected InputAction secondaryButtonAction;

        private const float MaxPressAge = 999f;

        public PlayerInputSensor(PlayerInput _playerInput, Transform _playerFocus)
        {
            playerInput = _playerInput;
            playerFocus = _playerFocus;

            moveAction = playerInput.actions["Move"];
            primaryButtonAction = playerInput.actions["Primary"];
            secondaryButtonAction = playerInput.actions["Secondary"];
        }

        public void CollectData(PlayerContext context)
        {
            context.moveInput = moveAction.ReadValue<Vector2>();
            context.primaryPressedTime   = primaryButtonAction.WasPressedThisFrame()   ? 0f : Mathf.Min(context.primaryPressedTime + Time.deltaTime, MaxPressAge);
            context.secondaryPressedTime = secondaryButtonAction.WasPressedThisFrame() ? 0f : Mathf.Min(context.secondaryPressedTime + Time.deltaTime);

            Vector3 right = playerFocus.right;
            right.y = 0f;
            right.Normalize();

            Vector3 forward = playerFocus.forward;
            forward.y = 0f;
            forward.Normalize();

            context.moveDirection = right * context.moveInput.x + forward * context.moveInput.y;
        }
    }
}