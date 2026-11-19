using UnityEngine;


namespace HerosCode.Toolkit.Util
{
    /// <summary>
    /// Moves a Rigidbody between two positions via Interpolate(t), e.g. driven by an automation.
    /// The positions are local to a reference transform, so the whole path follows
    /// wherever that transform is placed in the scene.
    /// </summary>
    public class PositionInterpolator : MonoBehaviour
    {
        [SerializeField, Tooltip("The physics body moved by the interpolator")]
        private Rigidbody rb = default;

        [SerializeField, Tooltip("The space from and to are expressed in. Leave empty to use the body's parent. With no parent they are world positions")]
        private Transform relativeTo = default;

        [SerializeField]
        private Vector3 from = default, to = default;

        private Transform ReferenceFrame => relativeTo != null ? relativeTo : rb.transform.parent;

        public void Interpolate(float t)
        {
            // Lerp in local space, then convert once. TransformPoint is an affine map,
            // so this gives the same result as converting both ends first
            Vector3 position = Vector3.LerpUnclamped(from, to, t);

            Transform frame = ReferenceFrame;
            if (frame != null)
            {
                position = frame.TransformPoint(position);
            }

            rb.MovePosition(position);
        }
    }
}