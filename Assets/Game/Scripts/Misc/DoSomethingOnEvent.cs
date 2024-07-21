using System;
using FD.Player;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Misc
{
    public class DoSomethingOnEvent : MonoBehaviour
    {
        [Serializable]
        public enum ListenableEvent
        {
            None,
            ObjectStart,
            ObjectDestroy,
            ObjectUpdate,
            TriggerEnter,
            TriggerExit,
            CollisionEnter,
            CollisionExit,
        }

        [SerializeField] private ListenableEvent performActionOnEvent = ListenableEvent.None;
        [SerializeField] private bool checkIfPlayer = true;

        public UnityEvent onAction;

        private void Start()
        {
            if (performActionOnEvent != ListenableEvent.ObjectStart) return;
            
            onAction?.Invoke();
        }

        private void Update()
        {
            if (performActionOnEvent != ListenableEvent.ObjectUpdate) return;
            
            onAction?.Invoke();
        }

        private void OnDestroy()
        {
            if (performActionOnEvent != ListenableEvent.ObjectDestroy) return;
            
            onAction?.Invoke();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (performActionOnEvent != ListenableEvent.TriggerEnter) return;
            if (checkIfPlayer && !other.gameObject.TryGetComponent(out PlayerController _)) return;
            
            onAction?.Invoke();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (performActionOnEvent != ListenableEvent.TriggerExit) return;
            if (checkIfPlayer && !other.gameObject.TryGetComponent(out PlayerController _)) return;
            
            onAction?.Invoke();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (performActionOnEvent != ListenableEvent.CollisionEnter) return;
            if (checkIfPlayer && !other.gameObject.TryGetComponent(out PlayerController _)) return;
            
            onAction?.Invoke();
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (performActionOnEvent != ListenableEvent.CollisionExit) return;
            if (checkIfPlayer && !other.gameObject.TryGetComponent(out PlayerController _)) return;
            
            onAction?.Invoke();
        }
    }
}
