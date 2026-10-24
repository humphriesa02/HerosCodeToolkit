using HerosCode.Toolkit.Player;
using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public static class PlayerStateUtil
    {
        // Set context velocity via desired velocity and a given acceleration
        public static void AdjustVelocity(PlayerContext context, float acceleration)
        {
            float currentX = Vector3.Dot(context.velocity, context.xAxis);
            float currentZ = Vector3.Dot(context.velocity, context.zAxis);
            
            float maxSpeedChange = acceleration * Time.fixedDeltaTime;

            float newX =
                Mathf.MoveTowards(currentX, context.desiredVelocity.x, maxSpeedChange);
            float newZ =
                Mathf.MoveTowards(currentZ, context.desiredVelocity.z, maxSpeedChange);

            context.velocity += context.xAxis * (newX - currentX) + context.zAxis * (newZ - currentZ);
        }

        // Apply ground normal based velocity to our current velocity to snap to the ground
        public static void ApplyGroundSnap(PlayerContext context)
        {
            if (!context.snapToGround) return;
            context.snapToGround = false;

            float dot = Vector3.Dot(context.velocity, context.contactNormal);
            if (dot <= 0f) return;

            float speed = context.velocity.magnitude;
            context.velocity = (context.velocity - context.contactNormal * dot).normalized * speed;
        }
    }
}