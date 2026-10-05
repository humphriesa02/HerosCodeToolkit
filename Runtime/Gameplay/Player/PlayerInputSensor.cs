using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Collect information from the player -
/// populate <see cref="PlayerContext"/>
/// </summary>
public class PlayerInputSensor : IPlayerSensor
{
    /// Input
    private PlayerInput playerInput; 
    
    protected InputAction moveAction; // Movement
    protected InputAction primaryButtonAction;
    protected InputAction secondaryButtonAction;

    public PlayerInputSensor(PlayerInput _playerInput)
    {
        playerInput = _playerInput;

        moveAction = playerInput.actions["Move"];
        primaryButtonAction = playerInput.actions["Primary"];
        secondaryButtonAction = playerInput.actions["Secondary"];
    }

    public void CollectData(PlayerContext context)
    {
        context.moveInput = moveAction.ReadValue<Vector2>();
        context.isPrimaryPressed = primaryButtonAction.WasPressedThisFrame();
        context.isSecondaryPressed = secondaryButtonAction.WasPressedThisFrame();
    }
}
