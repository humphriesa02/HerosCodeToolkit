using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Both player's intent
    /// and world forces acting on the player
    /// for *this given frame*
    /// </summary>
    public class PlayerContext
    {
        #region Movement
        public Vector2 moveInput;           // Direct from controller
        public Vector3 moveDirection;       // camera-relative, unscaled
        public Vector3 desiredVelocity;     // Horizontal, world space, units per second
        public Vector3 gravityVelocity;     // Our current player gravity accumulation
        public float gravityValue;          // how much gravity applies each frame
        public bool ignoreGravity;          // If false - don't apply gravity in Mover
        public Vector3 velocity;            // maintained across each state
        public Vector3 lookDirection;       // The direction our mover code will try and rotate the player
        public bool snapLook;               // If true, no lerping to change player rotation, simply snap it

        public Vector3 impulseVelocity;     // decaying horizontal burst, written by actions
        public Vector3 facing;              // written by the world sensor (flattened transform.forward)
        public int landingCount;            // incremented by the world sensor each time we touch down
        public float timeSinceLanded;       // reset to 0 on touchdown
        #endregion

        #region Controls
        /// <summary>
        /// Maps 1:1 with keybindings
        /// </summary>
        public bool isPrimaryPressed;
        public bool isSecondaryPressed;
        #endregion

        #region Animation
        public string animTrigger; // Use to one shot a "trigger" using the string provided
        #endregion

        #region World
        public bool isGrounded; // Set by CharacterController natively, passed down
        public bool nearbyWall;
        public bool insideWater;
        public float waterDepth;
        #endregion

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