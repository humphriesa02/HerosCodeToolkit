using UnityEngine;

/// <summary>
/// Inair state
/// </summary>
public class AirState : PlayerLocomotionState
{
    bool grounded;
    Vector3 airVelocity;
    public AirState(PlayerController _player, StateMachine _stateMachine, PlayerValues _values,PlayerContext _context) : base(_player, _stateMachine, _values, _context) { }

    public override void Enter()
    {
        base.Enter();
        grounded = false;
        context.gravityVelocity.y = 0;

        player.animator.SetFloat("speed", 0);
        player.animator.SetTrigger("jump");
        Jump();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (grounded)
        {
            stateMachine.ChangeState(player.landingState);
            
        }
        else // In air
        {
            airVelocity = new Vector3(context.moveInput.x, 0, context.moveInput.y);

            context.velocity = context.velocity.x * player.focus.right.normalized + context.velocity.z * player.focus.forward.normalized;
            context.velocity.y = 0f;
            airVelocity = airVelocity.x * player.focus.right.normalized + airVelocity.z * player.focus.forward.normalized;
            airVelocity.y = 0f;
            player.controller.Move(context.gravityVelocity * Time.deltaTime + (airVelocity*values.airControl+context.velocity * (1- values.airControl)) * values.moveSpeed * Time.deltaTime);
        }
        
        context.gravityVelocity.y += player.gravityValue * Time.deltaTime;
        grounded = player.controller.isGrounded;
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void Jump()
    {
        // TODO - dynamic jump amount based on velocity
        context.gravityVelocity.y += Mathf.Sqrt(values.jumpHeight * -3.0f * player.gravityValue);
    }
}
