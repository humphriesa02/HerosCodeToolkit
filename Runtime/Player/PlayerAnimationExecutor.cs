using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    public class PlayerAnimationExecutor : PlayerExecutor
    {
        [SerializeField, Tooltip("Retrieved from child visual object")]
        private Animator animator;

        public override void Initialize(PlayerContext context, PlayerValues values)
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        public override void Execute(PlayerContext context)
        {
            animator.SetFloat("speed", context.moveInput.magnitude);
            animator.SetBool("grounded", context.isGrounded);
            if (!string.IsNullOrEmpty(context.animTrigger))
            {
                animator.SetTrigger(context.animTrigger);
            }
        }
    }
}