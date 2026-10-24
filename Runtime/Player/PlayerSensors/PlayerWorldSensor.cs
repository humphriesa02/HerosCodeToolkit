using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Collect information from the world -
    /// populate <see cref="PlayerContext"/>
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerWorldSensor : MonoBehaviour, IPlayerSensor
    {
        // ex;
        // wall collider
        // depth guage
        // etc.
        private Rigidbody rb;
        private PlayerValues values;

        // Updated locally when evaluate collision executes - OnCollision events
        Vector3 localGroundContactNormal, localSteepContactNormal;
        int groundContactCount, steepContactCount;

        public void Initialize(PlayerContext _context, PlayerValues _values)
        {
            rb = GetComponent<Rigidbody>();
            values = _values;
        }

        public void CollectData(PlayerContext context){ }

        public void FixedCollectData(PlayerContext context)
        {
            context.velocity = rb.linearVelocity;
            
            CollectGroundingData(context);

            context.xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		    context.zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        }

        /// <summary>
        /// Populates the context with physics based data -
        /// things like if we're on the ground, and the normal
        /// of the ground for things like slopes
        /// </summary>
        /// <param name="context"></param>
        private void CollectGroundingData(PlayerContext context)
        {
            context.stepsSinceLastGrounded++;
            context.stepsSinceImpulse++;

            var sensedGround = groundContactCount > 0;
            var sensedSnap = false;
            var sensedCrevice = false;

            if (sensedGround) 
            {
                if (groundContactCount > 1) localGroundContactNormal.Normalize();
            }
            else if (ProbeGround(context, out Vector3 probeNormal))
            {
                sensedSnap = true;
                localGroundContactNormal = probeNormal;
            }
            else if (CheckSteepContacts(out Vector3 creviceNormal))
            {
                sensedCrevice = true;
                localGroundContactNormal = creviceNormal;
            }
            else
            {
                localGroundContactNormal = Vector3.up;
            }

            context.onGround = sensedGround || sensedSnap || sensedCrevice;
            if (context.onGround) context.stepsSinceLastGrounded = 0;

            context.snapToGround = sensedSnap;
            context.onSteep = steepContactCount > 0;

            context.contactNormal = localGroundContactNormal;
            context.steepNormal = context.onSteep ? localSteepContactNormal.normalized : Vector3.zero;
        }

        /// <summary>
        /// Clear ground and steep values for a fresh check
        /// </summary>
        /// <param name="context"></param>
        public void ClearData(PlayerContext context)
        {
            groundContactCount = steepContactCount = 0;
            localGroundContactNormal = localSteepContactNormal = Vector3.zero;
        }

        /// <summary>
        /// For ground snapping - raycast downwards to check for viable ground,
        /// if it exists use its normal. Later on, ground state will stick us
        /// to this normal.
        /// </summary>
        /// <returns></returns>
        private bool ProbeGround(PlayerContext context, out Vector3 normal)
        {
            normal = Vector3.up;
            if (context.stepsSinceLastGrounded > 1) {
                return false;
            }
            if (context.stepsSinceImpulse <= 2) return false;
            var speed = context.velocity.magnitude;
            if (speed > values.maxSnapSpeed) {
                return false;
            }
            if (!Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, values.probeDistance, values.probeMask)) {
			    return false;
		    }
            if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer))
            {
                return false;
            }
            normal = hit.normal;
            return true;
	    }

        void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        /// <summary>
        /// Given a collision, determine if it is ground,
        /// and update contact counts and normals accordingly
        /// </summary>
        /// <param name="collision"></param>
        void EvaluateCollision (Collision collision) {
            var minDot = GetMinDot(collision.gameObject.layer);
            for (int i = 0; i < collision.contactCount; i++) {
                Vector3 normal = collision.GetContact(i).normal;
                // Ground check
                if (normal.y >= minDot) {
                    groundContactCount += 1;
                    localGroundContactNormal += normal;
                }
                // Steep check
                else if (normal.y > -0.01f)
                {
                    steepContactCount +=1;
                    localSteepContactNormal += normal;
                }
            }
	    }

        /// <summary>
        /// Crevice check, checks "are the steep surfaces I'm touching
        /// holding me up like a flat floor would?"
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        bool CheckSteepContacts (out Vector3 normal) {
            normal = Vector3.up;
            // just a wall, not a crevice
            if (steepContactCount <= 1) return false;

            // The culmination of all the surrounding steep contacts,
            // if they're less than mingrounddot - we know we're on a "flat floor"
            Vector3 n = localSteepContactNormal.normalized;
            if (n.y < values.MinGroundDotProduct) return false;

            normal = n;
            return true;
        }

        Vector3 ProjectOnContactPlane (Vector3 vector) {
		    return vector - localGroundContactNormal * Vector3.Dot(vector, localGroundContactNormal);
	    }

        float GetMinDot (int layer) {
            return (values.stairsMask & (1 << layer)) == 0 ?
                values.MinGroundDotProduct : values.MinStairsDotProduct;
        }
    }
}