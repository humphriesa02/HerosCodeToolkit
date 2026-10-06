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

        public Vector3 gravityVelocity;
        public bool ignoreGravity;
        public Vector3 velocity; // maintained across each state

        public Vector3 lookDirection;
        public bool snapLook;

        /// World
        public bool isGrounded;
        public bool nearbyWall;
        public bool insideWater;
        public float waterDepth;
    }
}