using HerosCode.Toolkit.Core;
using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Based on <see cref="PlayerContext"/>
    /// resolve the locomotion state we are currently
    /// in and set it
    /// </summary>
    public class PlayerLocomotionStateDriver : IPlayerDriver
    {
        private readonly StateMachine stateMachine;
        private readonly PlayerLocomotionState[] playerStatesInOrder; // of priority

        public PlayerLocomotionStateDriver(StateMachine _machine, params PlayerLocomotionState[] _playerStatesInOrder)
        {
            stateMachine = _machine;
            playerStatesInOrder = _playerStatesInOrder;
        }

        public void Apply(PlayerContext context)
        {
            var current = stateMachine.GetCurrentState();

            foreach (var s in playerStatesInOrder)
            {
                bool valid = s == current ? s.CanStay(context) : s.CanEnter(context);
                if (!valid) continue;

                if (s != current) stateMachine.ChangeState(s);
                return;
            }
        }
    }
}

