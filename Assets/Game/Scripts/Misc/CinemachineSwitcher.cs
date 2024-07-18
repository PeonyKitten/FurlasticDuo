using FD.Player;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Misc
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
