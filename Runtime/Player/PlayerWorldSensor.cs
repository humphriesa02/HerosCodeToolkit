using UnityEngine;

namespace HerosCode.Toolkit.Gameplay.Player
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

        public PlayerWorldSensor(CharacterController _controller)
        {
            controller = _controller;
        }

        public void CollectData(PlayerContext context)
        {
            context.isGrounded = controller.isGrounded;
            context.gravityValue = gravityValue;
        }
    }
}