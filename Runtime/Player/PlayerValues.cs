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
        public enum LocomotionStyle
        {
            CharacterController,
            RigidBody
        }
        // Default to CharacterController
        [Header("Locomotion Style")]
        public LocomotionStyle locomotionStyle = LocomotionStyle.CharacterController;

        [Header("CharacterController Locomotion")]
        [Tooltip("Base max move speed of the player")]
        public float moveSpeed = 6.0f;
        [Tooltip("Base max jump height of the player")]
        public float jumpHeight = 0.8f;
        [Tooltip("Base max roll speed of the player")]
        public float rollSpeed = 10.0f;
        [Tooltip("Gravity multiplier, increase or decrease to affect gravity")]
        public float gravityMultiplier = 1f;
        [Range(0, 1), Tooltip("Animation speed damp time.")]
        public float speedDampTime = 0.1f;
        [Range(0, 1), Tooltip("The rate at which our velocity falls off. Increase for slidey movement.")]
        public float velocityDampTime = 0.9f;
        [Range(0, 1), Tooltip("The rate at which we fully rotate the player.")]
        public float rotationDampTime = 0.2f;
        [Range(0, 1), Tooltip("The amount of control we have over the player in the air.")]
        public float airControl = 0.5f;
        [Tooltip("Ensures input isn't dropped immediately when a frame ends. Extends input signals over a few frames instead.")]
        public float inputBuffer = 0.12f;

        [Header("Rigidbody Locomotion")]
        [Range(0f, 100f)]
        public float maxSpeed = 10f;
        [SerializeField, Range(0f, 100f)]
        public float maxAcceleration = 10f, maxAirAcceleration = 1f;
        [SerializeField, Range(0f, 10f)]
        public float dynamicJumpHeight = 2f;
        [SerializeField, Range(0, 5)]
        public int maxAirJumps = 0;
        [SerializeField, Range(0, 90)]
        public float maxGroundAngle = 25f;

        [Header("Dive")]
        [Tooltip("Horizontal burst speed at the start of a dive (units/sec).")]
        public float diveSpeed = 10f;
        [Tooltip("Upward velocity given by a dive. Compared against the current vertical speed with Max, so it never cuts a rise short.")]
        public float diveHop = 3f;
        [Header("Impulse Drag")]
        [Tooltip("How fast the impulse burst decays while grounded (units/sec²).")]
        public float impulseGroundDrag = 20f;
        [Tooltip("How fast the impulse burst decays in the air (units/sec²). Lower means the dive carries further.")]
        public float impulseAirDrag = 6f;

        [Header("Triple Jump")]
        [Tooltip("How long since landed you have to chain another jump")]
        public float chainWindow = 0.15f;
        public float[] chainHeightsMultipliers = {1.0f, 1.25f, 1.5f, 2.0f};
    }
}