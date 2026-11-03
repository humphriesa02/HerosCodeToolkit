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
            if (context.playerInputSpace && !context.isClimbing)
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

        /// <summary>
        /// Custom gravity implementation for states that use it.
        /// </summary>
        /// <param name="context"></param>
        public static void ApplyGravity(PlayerContext context)
        {
            context.velocity += Physics.gravity * Time.fixedDeltaTime;
        }

        /// <summary>
        /// Has player face wherever desired velocity is headed (the direction the player aims to move)
        /// </summary>
        /// <param name="c"></param>
        public static void FaceMoveDirection(PlayerContext c)
        {
            Vector3 dir = c.desiredVelocity;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f) c.lookDirection = dir;
        }

        /// <summary>
        /// Resets the jump phase if criteria are correct
        /// </summary>
        /// <param name="c"></param>
        public static void ResetJumpPhase(PlayerContext context)
        {
            if (context.onGround && context.stepsSinceImpulse > 1 && context.pendingImpulse == Vector3.zero)
                context.jumpPhase = 0;
        }

        /// <summary>
        /// Projects a direction to the plan represented by the normal
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3 ProjectOnPlane(Vector3 direction, Vector3 normal)=> (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }
}