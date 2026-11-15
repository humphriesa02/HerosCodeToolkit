using UnityEngine;

namespace HerosCode.Toolkit.Misc
{
    /// <summary>
    /// Simple buoyancy script.
    /// Applied to an object with a RigidBody,
    /// they will float in water bodies
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Buoyancy : MonoBehaviour
    {
        [SerializeField, Tooltip("How far above each buoyancy point the water probe starts (meters)")]
        private float submergenceOffset = 0.5f;

        [SerializeField, Min(0.1f), Tooltip("Probe depth over which submergence goes from 0 to 1 (meters)")]
        private float submergenceRange = 1f;

        [SerializeField, Min(0f), Tooltip("Total upward push at full submergence, as a multiple of gravity. 1 = neutral, above 1 floats up. Shared evenly between the points")]
        private float buoyancy = 1f;

        [SerializeField, Tooltip("Buoyancy points in local space. Leave empty for a single point at the pivot. Spread them out for stability, e.g. the lower corners of a crate")]
        private Vector3[] buoyancyOffsets = default;

        [SerializeField, Tooltip("Only count a point as submerged if it is actually inside the water volume. Turn on if the object gets lifted when it only touches the edge of the water")]
        private bool safeFloating = false;

        [SerializeField, Range(0f, 10f), Tooltip("How quickly water slows movement and spin")]
        private float waterDrag = 1f;

        [SerializeField]
        private LayerMask waterMask = 0;

        private Rigidbody body;
        private float[] submergence;

        void Awake()
        {
            body = GetComponent<Rigidbody>();

            // An unassigned array would throw, so fall back to one point at the pivot
            if (buoyancyOffsets == null || buoyancyOffsets.Length == 0)
            {
                buoyancyOffsets = new[] { Vector3.zero };
            }
            submergence = new float[buoyancyOffsets.Length];
        }

        void FixedUpdate()
        {
            int count = buoyancyOffsets.Length;
            float buoyancyFactor = -buoyancy / count;
            float totalSubmergence = 0f;

            for (int i = 0; i < count; i++)
            {
                if (submergence[i] <= 0f) continue;

                // Each point pushes against gravity, scaled by how deep it is
                body.AddForceAtPosition(
                    Physics.gravity * (buoyancyFactor * submergence[i]),
                    transform.TransformPoint(buoyancyOffsets[i]),
                    ForceMode.Acceleration
                );

                totalSubmergence += submergence[i];

                // Reset here, the trigger callbacks refill it during the next physics step
                submergence[i] = 0f;
            }

            if (totalSubmergence <= 0f) return;

            // One drag for the whole body, based on the average submergence.
            // Exponential drag is always between 0 and 1 and behaves the same at any step size
            float drag = Mathf.Exp(-waterDrag * (totalSubmergence / count) * Time.fixedDeltaTime);
            body.linearVelocity *= drag;
            body.angularVelocity *= drag;
        }

        void OnTriggerEnter(Collider other)
        {
            if (IsWater(other)) EvaluateSubmergence();
        }

        void OnTriggerStay(Collider other)
        {
            // A sleeping body stays asleep instead of being woken by buoyancy
            if (!body.IsSleeping() && IsWater(other)) EvaluateSubmergence();
        }

        private bool IsWater(Collider other)
        {
            return (waterMask & (1 << other.gameObject.layer)) != 0;
        }

        /// <summary>
        /// For each point, raycast down from above it to find the water surface.
        /// Hit distance 0 = just touching the surface, hit distance = range = fully submerged.
        /// </summary>
        private void EvaluateSubmergence()
        {
            Vector3 probeOffset = Vector3.up * submergenceOffset;

            for (int i = 0; i < buoyancyOffsets.Length; i++)
            {
                Vector3 p = probeOffset + transform.TransformPoint(buoyancyOffsets[i]);

                if (Physics.Raycast(
                        p, Vector3.down, out RaycastHit hit, submergenceRange + 1f,
                        waterMask, QueryTriggerInteraction.Collide))
                {
                    submergence[i] = Mathf.Clamp01(1f - hit.distance / submergenceRange);
                }
                else if (!safeFloating || Physics.CheckSphere(p, 0.01f, waterMask, QueryTriggerInteraction.Collide))
                {
                    // The probe starts inside the water volume, and a ray doesn't hit a collider it begins inside
                    submergence[i] = 1f;
                }
            }
        }
    }
}