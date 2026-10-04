using UnityEngine;

/// <summary>
/// Inair state
/// </summary>
public class AirState : PlayerLocomotionState
{
    bool grounded;
    Vector3 airVelocity;
    public AirState(PlayerController _player, StateMachine _stateMachine, PlayerValues _values) : base(_player, _stateMachine, _values) { }

    public override void Enter()
    {
        base.Enter();
        grounded = false;
        gravityVelocity.y = 0;

        player.animator.SetFloat("speed", 0);
        player.animator.SetTrigger("jump");
        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        moveInput = moveAction.ReadValue<Vector2>();
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
            airVelocity = new Vector3(moveInput.x, 0, moveInput.y);

            velocity = velocity.x * player.focus.right.normalized + velocity.z * player.focus.forward.normalized;
            velocity.y = 0f;
            airVelocity = airVelocity.x * player.focus.right.normalized + airVelocity.z * player.focus.forward.normalized;
            airVelocity.y = 0f;
            player.controller.Move(gravityVelocity * Time.deltaTime + (airVelocity*values.airControl+velocity * (1- values.airControl)) * values.moveSpeed * Time.deltaTime);
        }
        
        gravityVelocity.y += player.gravityValue * Time.deltaTime;
        grounded = player.controller.isGrounded;
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void Jump()
    {
        // TODO - dynamic jump amount based on velocity
        gravityVelocity.y += Mathf.Sqrt(values.jumpHeight * -3.0f * player.gravityValue);
    }
}
