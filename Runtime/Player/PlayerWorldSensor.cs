using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Collect information from the world -
    /// populate <see cref="PlayerContext"/>
    /// </summary>
    public class PlayerWorldSensor : IPlayerSensor
    {
        // ex;
        // wall collider
        // depth guage
        // etc.
        private readonly CharacterController controller;
        /// TODO - this should come from some world setting,
        /// localized or static, that we derive here
        public float gravityValue = -9.81f;

        private bool wasGrounded = false;

        public PlayerWorldSensor(CharacterController _controller)
        {
            controller = _controller;
        }

        public void CollectData(PlayerContext context)
        {
            context.isGrounded = controller.isGrounded;
            context.gravityValue = gravityValue;

            if (context.isGrounded && !wasGrounded) context.landingCount++;
            wasGrounded = context.isGrounded;
            context.facing = Vector3.ProjectOnPlane(controller.transform.forward, Vector3.up).normalized;
        }
    }
}