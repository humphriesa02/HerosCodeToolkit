using UnityEngine;
using HerosCode.Toolkit.Core;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Inair state
    /// </summary>
    public class AirState : PlayerLocomotionState
    {
        public override bool CanEnter(PlayerContext context) => !context.onGround;

        public AirState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

        public override void Enter()
        {
            base.Enter();
            PlayerStateUtil.ConvertMoveInputToDesiredVelocity(context, values.maxSpeed);
        }

        public override void PhysicsUpdate()
        {
            base.LogicUpdate();
            PlayerStateUtil.AdjustVelocity(context, values.maxAirAcceleration);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}