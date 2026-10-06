using UnityEngine;

namespace HerosCode.Toolkit.Gameplay.Player
{
    public class PlayerAnimationDriver : IPlayerDriver
    {
        private Animator animator;
        public PlayerAnimationDriver(Animator _animator)
        {
            animator = _animator;
        }

        public void Apply(PlayerContext context)
        {
            animator.SetFloat("speed", context.moveInput.magnitude);
        }
    }
}