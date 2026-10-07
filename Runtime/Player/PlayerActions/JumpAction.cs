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
            return context.isGrounded;
        }

        public void Start(PlayerContext context)
        {
            context.gravityVelocity.y = Mathf.Sqrt(values.jumpHeight * -3.0f * context.gravityValue);
            context.animTrigger = "jump";
        }
    }
}