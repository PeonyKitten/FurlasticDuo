// ResetPlayerSpeedFactor.cs
// Alvin Philips
// 2024-06-20
// Hard resets PlayerController values.

using FD.Player;
using UnityEngine;

namespace FD.Misc
{
    public class ResetPlayerSpeedFactor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController controller)) return;
            
            controller.speedFactor = 1f;
            controller.angularSpeedFactor = 1f;
            controller.accelerationFactor = 1f;
        }
    }
}