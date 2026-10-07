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
            var moving = context.moveDirection.sqrMagnitude > 0.01f;
            if (context.isGrounded) return moving;
            return lastDiveLanding != context.landingCount;
        }

        public void Start(PlayerContext context)
        {
            var dir = context.moveDirection.sqrMagnitude > 0.01f ? context.moveDirection.normalized : context.facing;

            context.impulseVelocity = dir * values.diveSpeed;
            context.gravityVelocity.y = Mathf.Max(context.gravityVelocity.y, values.diveHop);
            context.lookDirection = dir;
            context.snapLook = true;
            context.animTrigger = "dive";

            lastDiveLanding = context.landingCount;
        }
    }
}