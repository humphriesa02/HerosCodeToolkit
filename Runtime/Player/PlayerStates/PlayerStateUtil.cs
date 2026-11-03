using HerosCode.Toolkit.Player;
using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public static class PlayerStateUtil
    {
        // Set context velocity via desired velocity and a given acceleration
        public static void AdjustVelocity(PlayerContext context, float acceleration)
        {
            var relativeVelocity = context.velocity - context.connectionVelocity;
            var currentX = Vector3.Dot(relativeVelocity, context.xAxis);
            var currentZ = Vector3.Dot(relativeVelocity, context.zAxis);
            
            var maxSpeedChange = acceleration * Time.fixedDeltaTime;

            var newX =
                Mathf.MoveTowards(currentX, context.desiredVelocity.x, maxSpeedChange);
            var newZ =
                Mathf.MoveTowards(currentZ, context.desiredVelocity.z, maxSpeedChange);

            context.velocity += context.xAxis * (newX - currentX) + context.zAxis * (newZ - currentZ);
        }

        // Apply ground normal based velocity to our current velocity to snap to the ground
        public static void ApplyGroundSnap(PlayerContext context)
        {
            if (!context.snapToGround) return;
            context.snapToGround = false;

            var dot = Vector3.Dot(context.velocity, context.contactNormal);
            if (dot <= 0f) return;

            var speed = context.velocity.magnitude;
            context.velocity = (context.velocity - context.contactNormal * dot).normalized * speed;
        }

        // Given a 2D vector comprising move input action values, turn them into a desired velocity
        // based on max speed and the player's input space, if one exists (else world space)
        public static void ConvertMoveInputToDesiredVelocity(PlayerContext context, float maxSpeed)
        {
            Vector2 input = Vector2.ClampMagnitude(context.moveInput, 1f);
            if (context.playerInputSpace)
            {
			    Vector3 forward = context.playerInputSpace.forward;
                forward.y = 0f;
                forward.Normalize();

                Vector3 right = context.playerInputSpace.right;
                right.y = 0f;
                right.Normalize();

                context.desiredVelocity = (forward * input.y + right * input.x) * maxSpeed;
            }
            else
            {
                context.desiredVelocity = new Vector3(input.x, 0f, input.y) * maxSpeed;
            }
        }
    }
}