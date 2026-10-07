using UnityEngine;

namespace HerosCode.Toolkit.Gameplay.Player
{
    /// <summary>
    /// Both player's intent
    /// and world forces acting on the player
    /// for *this given frame*
    /// </summary>
    public class PlayerContext
    {
        /// Input
        public Vector2 moveInput;
        public Vector3 moveDirection; // camera-relative, unscaled
        public Vector3 desiredVelocity; // Horizontal, world space, units per second
        public bool isPrimaryPressed;
        public bool isSecondaryPressed;

        public Vector3 gravityVelocity; // Our current player gravity accumulation
        public float gravityValue; // how much gravity applies each frame
        public bool ignoreGravity;
        public Vector3 velocity; // maintained across each state

        public Vector3 lookDirection;
        public bool snapLook;

        // animation
        public string animTrigger;

        /// World
        public bool isGrounded;
        public bool nearbyWall;
        public bool insideWater;
        public float waterDepth;

        /// <summary>
        /// Maps Action derived <see cref="Keybind"/>
        /// to context specific pressed bools
        /// </summary>
        /// <param name="keybind"></param>
        /// <returns></returns>
        public bool WasPressed(Keybind keybind)
        {
            switch (keybind)
            {
                case Keybind.Primary:
                    return isPrimaryPressed;
                case Keybind.Secondary:
                    return isSecondaryPressed;
            }
            return false;
        }

        /// <summary>
        /// Clears the one-frame requests that states and actions write.
        /// Call once per frame, before sensors and states run.
        /// </summary>
        public void ResetFrame()
        {
            desiredVelocity = Vector3.zero;
            ignoreGravity = false;
            lookDirection = Vector3.zero;
            snapLook = false;
            animTrigger = null;
        }
    }
}