using UnityEngine;

namespace HerosCode.Toolkit.Player
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
            animator.SetBool("grounded", context.isGrounded);
            if(context.animTrigger != "")
            {
                animator.SetTrigger(context.animTrigger);
            }
        }
    }
}