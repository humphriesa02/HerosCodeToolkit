using UnityEngine;
namespace HerosCode.Toolkit.Cameras
{
    /// <summary>
    /// Updates values on the underwater shader material (should be attached to
    /// Fullscreen Pass Renderer Feature) on the URP render asset
    /// </summary>
    public class UnderwaterCamera : MonoBehaviour
    {
        [SerializeField, Tooltip("Meters of depth over which the effect fades in.")]
        private float transitionDepth = 0.2f;

        private static readonly int AmountID = Shader.PropertyToID("_UnderwaterAmount");

        private void OnEnable() => Shader.SetGlobalFloat(AmountID, 0f);
        private void OnDisable() => Shader.SetGlobalFloat(AmountID, 0f);

        private void LateUpdate()
        {
            var surfaceY = GetWaterHeight(transform.position);
            var depth = surfaceY - transform.position.y;
            var amount = Mathf.Clamp01(depth / transitionDepth);
            Shader.SetGlobalFloat(AmountID, amount);
        }
        private float GetWaterHeight(Vector3 position) => 0f;
    }
}
