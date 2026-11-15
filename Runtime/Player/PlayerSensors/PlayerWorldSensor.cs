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
        private Rigidbody rb, connectedBody, previousConnectedBody;
        private PlayerValues values;
        private PlayerContext context;

        // Updated locally when evaluate collision executes - OnCollision events
        private Vector3 localGroundContactNormal, localSteepContactNormal, localClimbContactNormal, lastClimbNormal;
        private Vector3 connectionWorldPosition, connectionLocalPosition;
        private int groundContactCount, steepContactCount, climbContactCount;
        private float submergence;

        public void Initialize(PlayerContext _context, PlayerValues _values)
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            context = _context;
            values = _values;
        }

        public void CollectData(PlayerContext context){ }

        public void FixedCollectData(PlayerContext context)
        {
            context.velocity = rb.linearVelocity;
            
            CollectGroundingData(context);

            if (context.isClimbing)
            {
                context.xAxis = PlayerStateUtil.ProjectOnPlane(Vector3.Cross(context.climbNormal, Vector3.up), context.climbNormal);
                context.zAxis = PlayerStateUtil.ProjectOnPlane(Vector3.up, context.climbNormal);
            }
            else
            {
                context.xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		        context.zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
            }
            

            if (connectedBody)
            {
                if (connectedBody.isKinematic || connectedBody.mass >= rb.mass)
                {
                    UpdateConnectionState(context);
                }
            }
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

            ResolveGrounding(context);

            if (context.onGround) context.stepsSinceLastGrounded = 0;
            context.onSteep = steepContactCount > 0;
            context.steepNormal = context.onSteep ? localSteepContactNormal.normalized : Vector3.zero;
            context.submergence = submergence;
            context.inWater = submergence > 0f;
            context.contactNormal = localGroundContactNormal;
        }

        private void ResolveGrounding(PlayerContext context)
        {
            var sensedClimb = false;
            var sensedSwimming = false;
            var sensedGround = false;
            var sensedSnap = false;
            var sensedCrevice = false;

            // Order is priority: the first branch that matches wins
            if (CheckClimbing(context, out Vector3 climbNormal))
            {
                sensedClimb = true;
                localGroundContactNormal = climbNormal;
            }
            else if (values.waterMovement != PlayerValues.WaterMovement.None && submergence > 0f &&
                    (submergence >= values.swimThreshold ||
                    (context.isSwimming && ProbeWater())))
            {
                sensedSwimming = true;
                localGroundContactNormal = Vector3.up;
            }
            else if (groundContactCount > 0)
            {
                sensedGround = true;
                if (groundContactCount > 1)
                {
                    localGroundContactNormal.Normalize();
                }
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

            // Commit resolved state
            context.isClimbing = sensedClimb;
            context.isSwimming = sensedSwimming;
            context.onGround = sensedGround || sensedSnap || sensedCrevice || sensedClimb;
            context.snapToGround = sensedSnap;
            context.climbNormal = climbNormal;
        }

        /// <summary>
        /// Clear ground and steep values for a fresh check
        /// </summary>
        /// <param name="context"></param>
        public void ClearData(PlayerContext context)
        {
            groundContactCount = steepContactCount = climbContactCount = 0;
            localGroundContactNormal = localSteepContactNormal = localClimbContactNormal = Vector3.zero;
            context.connectionVelocity = Vector3.zero;
            previousConnectedBody = connectedBody;
            connectedBody = null;
            submergence = 0f;
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
            if (context.stepsSinceLastGrounded > 1 || context.stepsSinceImpulse <= 2) {
                return false;
            }
            var speed = context.velocity.magnitude;
            if (speed > values.maxSnapSpeed) {
                return false;
            }
            if (!Physics.Raycast(
                rb.position,
                Vector3.down,
                out RaycastHit hit, 
                values.probeDistance, 
                values.probeMask, 
                QueryTriggerInteraction.Ignore)) 
            {
			    return false;
		    }
            if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer))
            {
                return false;
            }
            normal = hit.normal;
            connectedBody = hit.rigidbody;
            return true;
	    }

        private bool ProbeWater()
        {
            Vector3 origin = rb.position + Vector3.up * values.submergenceOffset;

            if (!Physics.Raycast(
                origin,
                Vector3.down,
                out RaycastHit hit,
                values.submergenceRange + 1f,
                values.waterMask,
                QueryTriggerInteraction.Collide))
            {
                return false;
            }

            // Distance between the player reference point and water surface.
            float surfaceDistance = values.submergenceOffset - hit.distance;

            return Mathf.Abs(surfaceDistance) <= values.waterProbeDistance;
        }

        void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void OnTriggerEnter(Collider other)
        {
            if ((values.waterMask & (1 << other.gameObject.layer)) != 0)
            {
                EvaluateSubmergence(other);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if ((values.waterMask & (1 << other.gameObject.layer)) != 0)
            {
                EvaluateSubmergence(other);
            }
        }

        /// <summary>
        /// Given a collision, determine if it is ground,
        /// and update contact counts and normals accordingly
        /// </summary>
        /// <param name="collision"></param>
        private void EvaluateCollision(Collision collision)
        {
            // TODO - fix this
            // Informal swim check
            if (values.waterMovement != PlayerValues.WaterMovement.None && submergence > 0f && submergence >= values.swimThreshold)
                return;

            var layer = collision.gameObject.layer;
            var minDot = GetMinDot(layer);
            for (int i = 0; i < collision.contactCount; i++) {
                Vector3 normal = collision.GetContact(i).normal;
                // Ground check
                if (normal.y >= minDot) {
                    groundContactCount += 1;
                    localGroundContactNormal += normal;
                    connectedBody = collision.rigidbody;
                }
                else 
                {
                    // Steep check
                    if (normal.y > -0.01f)
                    {
                        steepContactCount +=1;
                        localSteepContactNormal += normal;
                        if (groundContactCount == 0) {
                            connectedBody = collision.rigidbody;
                        }
                    }
                    // Climb check
                    if (normal.y >= values.MinClimbDotProduct &&
                       (values.climbableMask & (1 << layer)) != 0
                       )
                    {
                        climbContactCount += 1;
                        localClimbContactNormal += normal;
                        lastClimbNormal = normal;
                        connectedBody = collision.rigidbody;
                    }
                }
            }
	    }
        /// <summary>
        /// Similar to a ground check, but for water. Also,
        /// we can measure how "in" the ground (water) we are, via
        /// submergence
        /// </summary>
        private void EvaluateSubmergence(Collider collider)
        {
            if (Physics.Raycast(rb.position + Vector3.up * values.submergenceOffset,
                Vector3.down, out RaycastHit hit, values.submergenceRange + 1f,
			    values.waterMask, QueryTriggerInteraction.Collide)
                ) 
            {
			    submergence = 1f - hit.distance / values.submergenceRange;
		    }
            else
            {
                submergence = 1f;
            }
            if (values.waterMovement != PlayerValues.WaterMovement.None && submergence > 0f && submergence >= values.swimThreshold)
            {
                connectedBody = collider.attachedRigidbody;
            }
        }

        /// <summary>
        /// Crevice check, checks "are the steep surfaces I'm touching
        /// holding me up like a flat floor would?"
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        private bool CheckSteepContacts (out Vector3 normal)
        {
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

        private bool CheckClimbing(PlayerContext context, out Vector3 normal)
        {
            normal = Vector3.zero;
            if (climbContactCount == 0 || context.stepsSinceImpulse <= 2) return false;

            normal = localClimbContactNormal.normalized;

            // Several climbable surfaces can sum to a floor-like normal.
            // If so, use the most recent single wall instead.
            if (climbContactCount > 1 && normal.y >= values.MinGroundDotProduct)
                normal = lastClimbNormal;

            return true;
        }

        private Vector3 ProjectOnContactPlane (Vector3 vector)
        {
		    return vector - localGroundContactNormal * Vector3.Dot(vector, localGroundContactNormal);
	    }

        private float GetMinDot (int layer)
        {
            return (values.stairsMask & (1 << layer)) == 0 ?
                values.MinGroundDotProduct : values.MinStairsDotProduct;
        }

        private void UpdateConnectionState(PlayerContext context)
        {
            if (connectedBody == previousConnectedBody)
            {
                var connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
                context.connectionVelocity = connectionMovement / Time.deltaTime;
            }
            connectionWorldPosition = rb.position;
            connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
        }

        public void PreventSnapToGround()
        {
            context.stepsSinceImpulse = -1;
        }
    }
}