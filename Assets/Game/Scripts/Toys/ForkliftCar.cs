using System;
using UnityEngine;

namespace Game.Scripts.Toys
{
    public class ForkliftCar : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                player.speedFactor = 0;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                player.speedFactor = 1;
            }
        }
    }
}
