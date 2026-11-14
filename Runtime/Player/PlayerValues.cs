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

        [Tooltip("How fast the player turns toward its look direction (degrees/sec).")]
        public float turnSpeed = 720f;
        [Range(0, 1), Tooltip("Animation speed damp time.")]
        public float speedDampTime = 0.1f;

        [Header("Climb")]
        [SerializeField, Range(90, 180), Tooltip("The max angle a wall can be at that allows us to climb it.")]
        public float maxClimbAngle = 140f;
        [Tooltip("Layers the player can climb. Zelda-style: tick most world layers. Mario-style: tick only the Climbable layer.")]
        public LayerMask climbableMask;
        [Range(0f, 100f)]
        public float maxClimbSpeed = 2f;
        [Range(0f, 100f)]
        public float maxClimbAcceleration = 20f;

        [Header("Water")]
        // Sensing
        [Tooltip("Layers the player can swim on.")]
        public LayerMask waterMask = 0;
        [Tooltip("Vertical offset from the central point of the player to extend submergenceRange downwards")]
        public float submergenceOffset = 0.5f;
        [Min(0.1f), Tooltip("Starting from submergenceOffset, extend this far down to check submergence")]
        public float submergenceRange = 1f;
        [Range(0.01f, 1f), Tooltip("Once submergence passes this threshold, player will start swimming. Does not require full submergence.")]
        public float swimThreshold = 0.5f;
        [Min(0f), Tooltip("Distance our snap to water will check downwards")]
        public float waterProbeDistance = 0.4f;

        // Anything touching the water
        [Range(0f, 10f), Tooltip("Drag on the player while in water - making their move speed slower.")]
        public float waterDrag = 1f;
        [Min(0f), Tooltip("0 buoyancy sinks like a rock, 1 buoyancy negates gravity, maintaining equilibrium, more than 1 and you float to the top.")]
        public float buoyancy = 1.5f;

        // Swimming
        public enum WaterMovement 
        {
            None, // Can't swim, only sinks
            Free, // Can swim omnidirectionally
            Surface // At the surface, can only move horizontally, not vertically.
        }
        public WaterMovement waterMovement = WaterMovement.Surface;
        [Range(0f, 100f)]
        public float maxSwimSpeed = 5f;
        [Range(0f, 100f)]
        public float maxSwimAcceleration = 5f;

        // Diving
        public float diveImpulse = 4f;

        [Header("Jump")]
        [Range(0f, 10f), Tooltip("Force applied when jumping")]
        public float jumpHeight = 2f;
        [Range(0, 5), Tooltip("How many jumps, when in the air, you can do. I.e., 2 would allow 3 total jumps")]
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


        /// <summary>Cached by the world sensor or read directly; derived from maxGroundAngle.</summary>
        public float MinGroundDotProduct => Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        public float MinStairsDotProduct => Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        public float MinClimbDotProduct => Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }
}