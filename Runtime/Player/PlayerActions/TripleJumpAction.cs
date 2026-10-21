using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public class TripleJumpAction : IPlayerAction
    {
        private readonly PlayerValues values;
        int step;
        private static string[] animTriggers = {"jump", "jump", "jump2", "jump3"};

        public TripleJumpAction(PlayerValues _values)
        {
            values = _values;
        }

        public bool CanStart(PlayerContext context)
        {
            return context.isGrounded;
        }

        public void Start(PlayerContext context)
        {
            // Determine which jump to use
            // var moving = context.isMoving;
            // var inWindow = context.timeSinceLanded <= values.chainWindow;
            // var canAdvanceStep = inWindow && moving && step > 0 && step < 3;
            // if (canAdvanceStep)
            // {
            //     step +=1;
            // }
            // else
            // {
            //     step = 1; // default to normal jump
            // }
            // // retrieve heights via step from values
            // var height = values.jumpHeight * values.chainHeightsMultipliers[step - 1];
            // context.gravityVelocity.y = Mathf.Sqrt(height * -3.0f * context.gravityValue);
            // context.animTrigger = animTriggers[step];
        }
    }
}