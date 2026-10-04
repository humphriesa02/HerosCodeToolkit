using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A state purely represents locomotion available
/// the player. Things like basic land movement,
/// swimming, falling, etc.
/// 
/// Actions are triggers for locomotion changes,
/// or, simply do things within a locomotion state.
/// (Ground combat vs air combat)
/// </summary>
public class PlayerLocomotionState : State
{
    // States store ref to player and their owning machine.
    // They handle swapping themselves to other states
    protected PlayerController player;
    protected StateMachine stateMachine;
    protected PlayerValues values;

    // Some good 
    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 moveInput;

    protected InputAction moveAction; // Movement

    public PlayerLocomotionState(PlayerController _player, StateMachine _stateMachine, PlayerValues _values)
    {
        player = _player;
        stateMachine = _stateMachine;
        values = _values;

        moveAction = player.playerInput.actions["Move"];
    }
}
