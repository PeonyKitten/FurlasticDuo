using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Misc
{
    public class ResetPlayerSpeedFactor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController controller))
            {
                controller.speedFactor = 1f;
                controller.angularSpeedFactor = 1f;
                controller.accelerationFactor = 1f;
            }
        }
    }
}