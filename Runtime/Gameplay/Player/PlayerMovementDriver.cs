using UnityEngine;

/// <summary>
/// Applies actual movement to the player
/// via <see cref="PlayerContext"/>
/// </summary>
public class PlayerMovementDriver : IPlayerDriver
{
    private CharacterController controller;
    private PlayerValues playerValues;
    private float gravity;

    public PlayerMovementDriver(CharacterController _controller, PlayerValues _playerValues, float _gravity)
    {
        controller = _controller;
        playerValues = _playerValues;
        gravity = _gravity;
    }

    public void Apply(PlayerContext context)
    {
        if (!context.ignoreGravity)
        {
            context.gravityVelocity.y += gravity * Time.deltaTime;
            if (controller.isGrounded && context.gravityVelocity.y < 0)
            {
                context.gravityVelocity.y = 0f;
            }
        }

        Vector3 vel = context.desiredVelocity;
        vel.y = context.gravityVelocity.y;
        controller.Move(vel * Time.deltaTime);

        // rotation
        if (context.lookDirection.sqrMagnitude > 0f)
        {
            var target = Quaternion.LookRotation(context.lookDirection);
            controller.transform.rotation = context.snapLook ? target
                : Quaternion.Slerp(controller.transform.rotation, target, playerValues.rotationDampTime);
        }
    }
}
