using FD.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Toys
{
    public class PressurePad : MonoBehaviour
    {
        [SerializeField] private float activationMass = 10f;

        [Header("Piston Settings")]
        [SerializeField] private Transform piston;
        [SerializeField] private Vector3 pistonMoveAxis = Vector3.down;
        [SerializeField] private float pistonMoveDistance = 0.1f;
        [SerializeField] private float pistonDownTime = 0.5f;
        
        [Header("Callbacks")]
        public UnityEvent onPress;
        public UnityEvent onRelease;
        
        public bool IsPressed { get; private set; }

        private float _currentMass;
        private Vector3 _initialPistonPosition = Vector3.zero;
        private Vector3 _targetPistonPosition = Vector3.zero;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Rigidbody rb))
            {
                _currentMass += rb.mass;
            } else if (other.TryGetComponent(out DummyRigidbody dummyRb))
            {
                _currentMass += dummyRb.Mass;
            }

            CheckPressStatus();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Rigidbody rb))
            {
                _currentMass -= rb.mass;
            } else if (other.TryGetComponent(out DummyRigidbody dummyRb))
            {
                _currentMass -= dummyRb.Mass;
            }
            
            CheckPressStatus();
        }

        private void CheckPressStatus()
        {
            var wasPressed = IsPressed;
            IsPressed = _currentMass >= activationMass;

            if (IsPressed == wasPressed) return;

            if (IsPressed)
            {
                OnPress();
            }
            else
            { 
                OnRelease();
            }
        }

        //Kai added this function to set the value of isPressed from outside, temporary use
        public void ForceRelease()
        {
            IsPressed = false;
            OnRelease();
        }

        private void OnPress()
        {
            _targetPistonPosition = _initialPistonPosition + pistonMoveAxis * pistonMoveDistance;
            onPress.Invoke();
        }
        
        private void OnRelease()
        {
            _targetPistonPosition = _initialPistonPosition;
            onRelease.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsPressed ? Color.green : Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        private void Update()
        {
            if (!piston) return;
            
            var delta = pistonMoveDistance / pistonDownTime * Time.deltaTime;
            piston.localPosition = Vector3.MoveTowards(piston.localPosition, _targetPistonPosition, delta);
        }

        private void Start()
        {
            if (piston)
            {
                _initialPistonPosition = piston.localPosition;
                _targetPistonPosition = _initialPistonPosition;
            }
        }
    }
}
