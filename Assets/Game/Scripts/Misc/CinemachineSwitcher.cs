using System;
using FD.Player;
using FD.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Misc
{
    public class CinemachineSwitcher : MonoBehaviour
    {
        [Header("Callbacks (will be deprecated)")]
        public UnityEvent onEnter;

        [Header("Transitions")]
        [SerializeField] private string cinemachineStateEnter;
        [SerializeField] private string cinemachineStateExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                OnEnter();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnExit();
            }
        }

        private void OnEnter()
        {
            onEnter.Invoke();
            
            if (!string.IsNullOrEmpty(cinemachineStateEnter))
            {
                CameraUtils.StateDrivenCameraAnimator.Play(cinemachineStateEnter);
            }
        }
        
        private void OnExit()
        {
            if (!string.IsNullOrEmpty(cinemachineStateExit))
            {
                CameraUtils.StateDrivenCameraAnimator.Play(cinemachineStateExit);
            }
        }
    }
}
