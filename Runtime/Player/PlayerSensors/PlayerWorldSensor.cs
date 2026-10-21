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
        /// TODO - this should come from some world setting,
        /// localized or static, that we derive here
        public float gravityValue = -9.81f;

        Vector3 localContactNormal;
        int groundContactCount;

        public void Initialize(PlayerContext _context, PlayerValues _values)
        {
            rb = GetComponent<Rigidbody>();
            values = _values;
        }

        public void CollectData(PlayerContext context){ }

        public void FixedCollectData(PlayerContext context)
        {
            context.stepsSinceLastGrounded += 1;
            context.velocity = rb.linearVelocity;
            context.xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		    context.zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
            context.isGrounded = groundContactCount > 0;
            if (context.isGrounded || SnapToGround(context)) 
            {
                context.stepsSinceLastGrounded = 0;
                context.jumpPhase = 0;
                if (groundContactCount > 1) {
                    localContactNormal.Normalize();
                }
            }
            else 
            {
                localContactNormal = Vector3.up;
            }
            context.contactNormal = localContactNormal;
        }

        public void ClearData(PlayerContext context)
        {
            groundContactCount = 0;
            localContactNormal = Vector3.zero;
        }

        private bool SnapToGround (PlayerContext context) {
            if (context.stepsSinceLastGrounded > 1) {
                return false;
            }
            if (!Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit)) {
			    return false;
		    }
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
            for (int i = 0; i < collision.contactCount; i++) {
                Vector3 normal = collision.GetContact(i).normal;
                if (normal.y >= values.MinGroundDotProduct) {
                    groundContactCount += 1;
                    localContactNormal += normal;
                }
            }
	    }

        Vector3 ProjectOnContactPlane (Vector3 vector) {
		    return vector - localContactNormal * Vector3.Dot(vector, localContactNormal);
	    }
    }
}