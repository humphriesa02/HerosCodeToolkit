using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Values to be loaded into <see cref="PlayerController"/>
    /// 
    /// Dictates both how the Player controls, and the values
    /// that go with it.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerValue", menuName = "PlayerValues/CreateNewPlayerValues", order = 1)]
    public class PlayerValues : ScriptableObject
    {
        [Header("Input")]
        [Tooltip("Ensures input isn't dropped immediately when a frame ends. Extends input signals over a few frames instead.")]
        public float inputBuffer = 0.12f;

        [Header("Movement")]
        [Range(0f, 100f)]
        public float maxSpeed = 10f;
        [Range(0f, 100f)]
        public float maxAcceleration = 10f, maxAirAcceleration = 1f;

        /// <summary>
        /// Ground snapping
        /// </summary>
        [Range(0, 90), Tooltip("Max angle of a slope we can still climb")]
        public float maxGroundAngle = 25f, maxStairsAngle = 50f;
        [Range(0f, 100f), Tooltip("If we exceed this speed on a slope we won't stick to it")]
        public float maxSnapSpeed = 100f;
        [Min(0f), Tooltip("Distance our snap to ground will check downwards")]
        public float probeDistance = 1f;
        [Tooltip("Mask used for layers we can snap to.")]
        public LayerMask probeMask = -1, stairsMask = -1;

        /// <summary>Cached by the world sensor or read directly; derived from maxGroundAngle.</summary>
        public float MinGroundDotProduct => Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        public float MinStairsDotProduct => Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);

        [Tooltip("How fast the player turns toward its look direction (degrees/sec).")]
        public float turnSpeed = 720f;
        [Range(0, 1), Tooltip("Animation speed damp time.")]
        public float speedDampTime = 0.1f;

        [Header("Jump")]
        [Range(0f, 10f)]
        public float jumpHeight = 2f;
        [Range(0, 5)]
        public int maxAirJumps = 0;

        [Header("Triple Jump")]
        [Tooltip("Seconds after landing in which another jump continues the chain.")]
        public float chainWindow = 0.3f;
        [Tooltip("Height multiplier per chain step.")]
        public float[] chainHeightsMultipliers = { 1.0f, 1.25f, 1.5f, 2.0f };

        [Header("Dive")]
        [Tooltip("Horizontal burst speed at the start of a dive (units/sec).")]
        public float diveSpeed = 10f;
        [Tooltip("Upward velocity given by a dive. Compared against current vertical speed with Max, so it never cuts a rise short.")]
        public float diveHop = 3f;
    }
}