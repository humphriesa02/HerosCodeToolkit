using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public class DiveAction : IPlayerAction
    {
        private readonly PlayerValues values;
        private int lastDiveLanding = -1;

        public DiveAction(PlayerValues _values)
        {
            values = _values;
        }

        public bool CanStart(PlayerContext context)
        {
            // On the ground and moving
            if (context.isGrounded && context.moveDirection.sqrMagnitude > 0.01f)
            {
                return true;
            }
            // In the air and we haven't dove yet (set lastDive landing to whatever context holds to prevent)
            // multiple dives until we land
            else if (!context.isGrounded && lastDiveLanding != context.landingCount)
            {
                return true;
            }

            return false;
        }

        public void Start(PlayerContext context)
        {
            // Pick a direction and add an impulse
            var dir = context.facing;
            context.impulseVelocity = dir * values.diveSpeed;
            
            // Add some air to the dive
            context.gravityVelocity.y = Mathf.Max(context.gravityVelocity.y, values.diveHop);
            context.lookDirection = dir;
            context.snapLook = true;
            context.animTrigger = "dive";

            lastDiveLanding = context.landingCount;
        }
    }
}