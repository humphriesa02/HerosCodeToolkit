using HerosCode.Toolkit.Player;
using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public static class PlayerStateUtil
    {
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
    }
}