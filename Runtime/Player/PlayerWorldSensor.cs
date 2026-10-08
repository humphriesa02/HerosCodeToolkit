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

            if (context.isGrounded)
            {
                context.timeSinceLanded += Time.deltaTime;
            }
            if (context.isGrounded && !wasGrounded)
            {
                context.landingCount++;
                context.timeSinceLanded = 0;
            } 
            wasGrounded = context.isGrounded;

            context.gravityValue = gravityValue;
            context.facing = Vector3.ProjectOnPlane(controller.transform.forward, Vector3.up).normalized;
        }
    }
}