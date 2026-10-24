using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public class JumpAction : IPlayerAction
    {
        private readonly PlayerValues values;

        public JumpAction(PlayerValues _values)
        {
            values = _values;
        }

        public bool CanStart(PlayerContext context)
        {
            return context.onGround || context.onSteep || context.jumpPhase < values.maxAirJumps;
        }

        public void Start(PlayerContext context)
        {
            context.jumpPhase += 1;
			float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * values.jumpHeight);
			float alignedSpeed = Vector3.Dot(context.velocity, context.contactNormal);
			if (alignedSpeed > 0f) {
				jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
			}
			context.pendingImpulse += context.contactNormal * jumpSpeed;
            context.animTrigger = "jump";
        }
    }
}