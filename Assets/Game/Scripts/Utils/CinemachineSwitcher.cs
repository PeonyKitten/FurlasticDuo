using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Utils
{
    public class CinemachineSwitcher : MonoBehaviour
    {
        [Header("Callbacks")]
        public UnityEvent onEnter;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                OnEnter();
            }
        }

        private void OnEnter()
        {
            onEnter.Invoke();
        }

    }
}
