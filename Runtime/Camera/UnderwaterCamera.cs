using UnityEngine;
namespace HerosCode.Toolkit.Cameras
{
    public class UnderwaterCamera : MonoBehaviour
    {
        [SerializeField, Tooltip("Meters of depth over which the effect fades in.")]
        private float transitionDepth = 0.2f;

        private static readonly int AmountID = Shader.PropertyToID("_UnderwaterAmount");

        void OnEnable()  => Shader.SetGlobalFloat(AmountID, 0f);
        void OnDisable() => Shader.SetGlobalFloat(AmountID, 0f);

        void LateUpdate()
        {
            var surfaceY = GetWaterHeight(transform.position);
            var depth = surfaceY - transform.position.y;
            var amount = Mathf.Clamp01(depth / transitionDepth);
            Shader.SetGlobalFloat(AmountID, amount);
        }
        float GetWaterHeight(Vector3 position) => 0f;
    }
}
