using FD.Toys;
using UnityEngine;

namespace FD.Misc
{
    public class CurvedHallwayTrigger : MonoBehaviour
    {
        [SerializeField] private Door hamsterWheelDoor;
        [SerializeField] private Material curvedHallwayMaterial;
        [SerializeField] private float maxSkew = 1f;
        [SerializeField] private bool reverseOpenness;
        
        private static readonly int ShaderHashSkewMultiplier = Shader.PropertyToID("_Skew_Multiplier");

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                UpdateShaderSkew();
            }
        }

        private void UpdateShaderSkew()
        {
            var openness = reverseOpenness ? 1 - hamsterWheelDoor.Openness: hamsterWheelDoor.Openness;
            var skew = openness * maxSkew;

            curvedHallwayMaterial.SetFloat(ShaderHashSkewMultiplier, skew);
        }
    }
}
