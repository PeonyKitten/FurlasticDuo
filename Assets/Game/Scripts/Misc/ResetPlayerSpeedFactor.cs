// ResetPlayerSpeedFactor.cs
// Alvin Philips
// 2024-06-20
// Hard resets PlayerController values.

using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Misc
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