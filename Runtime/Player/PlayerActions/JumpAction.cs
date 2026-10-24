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
            return context.onGround || context.onSteep || (values.maxAirJumps > 0 && context.jumpPhase <= values.maxAirJumps);;
        }

        public void Start(PlayerContext context)
        {
            Vector3 jumpDirection;

            if (context.onGround)
            {
                context.jumpPhase = 0;
                jumpDirection = context.contactNormal;
            }
            else if (context.onSteep)
            {
                jumpDirection = context.steepNormal;
                context.jumpPhase = 0;
            }
            else
            {
                if (context.jumpPhase == 0) {
				    context.jumpPhase = 1;
			    }
                jumpDirection = context.contactNormal;
            }
            context.jumpPhase += 1;
            // vertical bias
            jumpDirection = (jumpDirection + Vector3.up).normalized;

			float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * values.jumpHeight);
			float alignedSpeed = Vector3.Dot(context.velocity, jumpDirection);
			if (alignedSpeed > 0f) {
				jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
			}
			context.pendingImpulse += jumpDirection * jumpSpeed;
            context.animTrigger = "jump";
        }
    }
}