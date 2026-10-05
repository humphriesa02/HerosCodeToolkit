using UnityEngine;

/// <summary>
/// Basic grounded movement
/// </summary>
public class GroundState : PlayerLocomotionState
{
    float gravityValue;
    Vector3 currentVelocity;
    bool grounded;
    float playerSpeed;

    private Vector3 cVelocity;

    public GroundState(PlayerController _player, StateMachine _stateMachine, PlayerValues _values, PlayerContext _context) : base(_player, _stateMachine, _values, _context) { }

    public override void Enter()
    {
        base.Enter();
        context.velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        context.gravityVelocity.y = 0;
 
        playerSpeed = values.moveSpeed;
        grounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        context.velocity = new Vector3(context.moveInput.x, 0.0f, context.moveInput.y);

        context.velocity = context.velocity.x * player.focus.right.normalized + context.velocity.z * player.focus.forward.normalized;
        context.velocity.y = 0f;

        player.animator.SetFloat("speed", context.moveInput.magnitude, values.speedDampTime, Time.deltaTime);
        
        context.gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = player.controller.isGrounded;

        if (grounded && context.gravityVelocity.y < 0)
        {
            context.gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, context.velocity, ref cVelocity, values.velocityDampTime);
        player.controller.Move(playerSpeed * Time.deltaTime * currentVelocity + context.gravityVelocity * Time.deltaTime);

        if (context.velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(context.velocity), values.rotationDampTime);
        }
    }

    public override void Exit()
    {
        base.Exit();

        context.gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(context.moveInput.x, 0, context.moveInput.y);
        if (context.velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(context.velocity);
        }
    }
}
