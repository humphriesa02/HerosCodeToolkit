using UnityEngine;

/// <summary>
/// Locomotion states set these,
/// and are read by actions to determine
/// which can be done when
/// </summary>
public enum PlayerLocomotionTags
{
    Grounded,
    Airborne,
    Submerged
}

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
    protected PlayerContext context;


    public PlayerLocomotionState(PlayerController _player, StateMachine _stateMachine, PlayerValues _values, PlayerContext _context)
    {
        player = _player;
        stateMachine = _stateMachine;
        values = _values;
        context = _context;
    }
}
