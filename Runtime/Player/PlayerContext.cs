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
        public bool isMoving =>
        moveDirection.sqrMagnitude > 0.01f; // Quick access to those that want to know
        public Vector3 desiredVelocity;     // Horizontal, world space, units per second
        public Vector3 velocity;            // maintained across each state
        public Vector3 xAxis;               // The current x axis, used for inversion
        public Vector3 zAxis;               // The current z axis
        public Vector3 contactNormal;       // What is "up" depending on how we land
        public Vector3 pendingImpulse;
        public int stepsSinceLastGrounded;
        public int stepsSinceImpulse = 999;
        public int jumpPhase;               // Double jump access. If phase > 1 we're double jumping

        public Vector3 lookDirection;       // direction we want to face. Persists, so we keep facing it after input stops
        public bool snapLook;               // one-shot: face lookDirection instantly. Drained by the executor
        #endregion

        #region Controls
        /// <summary>
        /// Maps 1:1 with keybindings
        /// </summary>
         
        // Swapped from bools to floats
        // to introduce input buffers
        public float primaryPressedTime = 999f;
        public float secondaryPressedTime = 999f;
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
        public bool WasPressed(Keybind keybind, float window)
        {
            switch (keybind)
            {
                case Keybind.Primary:
                    return primaryPressedTime <= window;
                case Keybind.Secondary:
                    return secondaryPressedTime <= window;
            }
            return false;
        }

        public void ConsumePress(Keybind keybind)
        {
            switch (keybind)
            {
                case Keybind.Primary:
                    primaryPressedTime = 999f;
                    break;
                case Keybind.Secondary:
                    secondaryPressedTime = 999f;
                    break;
            }
        }

        /// <summary>
        /// Clears the one-frame requests that states and actions write.
        /// Call once per frame, before sensors and states run.
        /// </summary>
        public void ResetFrame()
        {
            desiredVelocity = Vector3.zero;
            animTrigger = null;
        }
    }
}