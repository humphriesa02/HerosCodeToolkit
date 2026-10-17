using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Applies actual movement to the player
    /// via <see cref="PlayerContext"/>
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementExecutor : PlayerExecutor
    {
        private PlayerValues playerValues;
        private CharacterController controller;

        public override void Initialize(PlayerContext context, PlayerValues values)
        {
            playerValues = values;
            controller = GetComponent<CharacterController>();
        }

        public override void Execute(PlayerContext context)
        {
            if (!context.ignoreGravity)
            {
                context.gravityVelocity.y += context.gravityValue * Time.deltaTime;
                if (controller.isGrounded && context.gravityVelocity.y < 0)
                {
                    context.gravityVelocity.y = -2f; // stick to ground
                }
            }

            Vector3 vel = context.desiredVelocity + context.impulseVelocity;
            vel.y = context.gravityVelocity.y;
            controller.Move(vel * Time.deltaTime);

            float drag = context.isGrounded ? playerValues.impulseGroundDrag : playerValues.impulseAirDrag;
            context.impulseVelocity = Vector3.MoveTowards(context.impulseVelocity, Vector3.zero, drag * Time.deltaTime);

            // rotation
            if (context.lookDirection.sqrMagnitude > 0f)
            {
                var target = Quaternion.LookRotation(context.lookDirection);
                controller.transform.rotation = context.snapLook ? target
                    : Quaternion.Slerp(controller.transform.rotation, target, playerValues.rotationDampTime);
            }
        }
    }
}