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

    public GroundState(PlayerController _player, StateMachine _stateMachine, PlayerValues _values) : base(_player, _stateMachine, _values) { }

    public override void Enter()
    {
        base.Enter();
        moveInput = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
 
        playerSpeed = values.moveSpeed;
        grounded = player.controller.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        moveInput = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(moveInput.x, 0.0f, moveInput.y);

        velocity = velocity.x * player.focus.right.normalized + velocity.z * player.focus.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.animator.SetFloat("speed", moveInput.magnitude, values.speedDampTime, Time.deltaTime);
        
        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = player.controller.isGrounded;

        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, values.velocityDampTime);
        player.controller.Move(playerSpeed * Time.deltaTime * currentVelocity + gravityVelocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity), values.rotationDampTime);
        }
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(moveInput.x, 0, moveInput.y);
        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
