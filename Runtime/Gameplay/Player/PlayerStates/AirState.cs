using UnityEngine;

/// <summary>
/// Inair state
/// </summary>
public class AirState : PlayerLocomotionState
{
    public AirState(PlayerController _player, StateMachine _stateMachine, PlayerValues _values, PlayerContext _context) : base(_player, _stateMachine, _values, _context) { }

    public override void Enter()
    {
        base.Enter();
        context.gravityVelocity.y = 0;

        Jump();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        Vector3 blended = context.moveDirection * values.airControl + context.velocity * (1 - values.airControl);
        context.desiredVelocity = blended * values.moveSpeed;

        if (context.isGrounded) stateMachine.ChangeState(player.landingState);
    }

    public override void Exit()
    {
        base.Exit();
    }

    // TODO: Move to an action
    private void Jump()
    {
        // TODO - dynamic jump amount based on velocity
        context.gravityVelocity.y += Mathf.Sqrt(values.jumpHeight * -3.0f * player.gravityValue);
    }
}
