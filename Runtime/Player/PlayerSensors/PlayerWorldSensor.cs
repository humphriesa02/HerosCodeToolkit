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
            context.stepsSinceLastGrounded++;
            context.stepsSinceImpulse++;
            context.velocity = rb.linearVelocity;
            
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

            context.xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		    context.zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        }

        public void ClearData(PlayerContext context)
        {
            groundContactCount = steepContactCount = 0;
            localGroundContactNormal = localSteepContactNormal = Vector3.zero;
        }

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

        void EvaluateCollision (Collision collision) {
            var minDot = GetMinDot(collision.gameObject.layer);
            for (int i = 0; i < collision.contactCount; i++) {
                Vector3 normal = collision.GetContact(i).normal;
                if (normal.y >= minDot) {
                    groundContactCount += 1;
                    localGroundContactNormal += normal;
                }
                else if (normal.y > -0.01f)
                {
                    steepContactCount +=1;
                    localSteepContactNormal += normal;
                }
            }
	    }

        bool CheckSteepContacts (out Vector3 normal) {
            normal = Vector3.up;
            if (steepContactCount <= 1) return false;

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