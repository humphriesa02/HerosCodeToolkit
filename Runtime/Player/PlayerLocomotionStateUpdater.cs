using System.Collections.Generic;
using HerosCode.Toolkit.Core;
using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Based on <see cref="PlayerContext"/>
    /// resolve the locomotion state we are currently
    /// in and set it
    /// </summary>
    public class PlayerLocomotionStateUpdater : IPlayerUpdater
    {
        // State Machine
        private StateMachine stateMachine;

        private readonly List<PlayerLocomotionState> playerStatesInOrder = new(); // of priority
        
        public State GetCurrentState() => stateMachine.GetCurrentState();

        public PlayerLocomotionStateUpdater(PlayerContext context, PlayerValues values)
        {
            stateMachine = new StateMachine();
            playerStatesInOrder.Add(new GroundState(values, context));
            playerStatesInOrder.Add(new AirState(values, context));

            stateMachine.Initialize(playerStatesInOrder[0]);
        }

        public  void Tick(PlayerContext context)
        {
            ResolveState(context);
            stateMachine.LogicUpdate();
        }

        public  void LateTick(PlayerContext context)
        {
            stateMachine.LateUpdate();
        }

        public  void FixedTick(PlayerContext context)
        {
            stateMachine.PhysicsUpdate();
        }

        public void ResolveState(PlayerContext context)
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

