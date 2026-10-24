using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public class PunchAction : IPlayerAction
    {
        private readonly PlayerValues values;

        public PunchAction(PlayerValues _values)
        {
            values = _values;
        }

        public bool CanStart(PlayerContext context)
        {
            // On the ground and moving
            if (context.onGround && !context.isMoving)
            {
                return true;
            }

            return false;
        }

        public void Start(PlayerContext context)
        {
            // Do some crazy punch logic boi
            context.animTrigger = "punch";
        }
    }
}