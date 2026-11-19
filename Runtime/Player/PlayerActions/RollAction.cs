using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public class RollAction : IPlayerAction
    {
        private readonly PlayerValues values;

        public RollAction(PlayerValues _values)
        {
            values = _values;
        }

        public bool CanStart(PlayerContext context)
        {
            // if(!context.isClimbing && !context.inWater)
            // {
            //     return true;
            // }

            return true;
        }

        public void Start(PlayerContext context)
        {
            context.isRolling = !context.isRolling;
        }
    }
}