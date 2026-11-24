using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public class WaterDiveAction : IPlayerAction
    {
        private readonly PlayerValues values;
        public WaterDiveAction(PlayerValues _values) { values = _values; }

        public bool CanStart(PlayerContext context) => context.inWater;

        public void Start(PlayerContext context)
        {
            context.pendingImpulse += Vector3.down * values.diveImpulse;
            context.animTrigger = "dive";
        }
    }
}
