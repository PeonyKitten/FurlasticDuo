using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace FD.Toys
{
    [Serializable]
    public enum StartState
    {
        None,
        Open,
        Closed,
    }
    
    public class Door : MonoBehaviour
    {
        [SerializeField] private Transform gate;
        
        [Header("Translation")]
        [SerializeField] private bool usePosition = true;
        [SerializeField] private bool useRigidbodyPosition;
        [SerializeField] private bool useGlobalPosition;
        [SerializeField] private Vector3 openPosition;
        [SerializeField] private Vector3 closePosition;
        
        [Header("Rotation")]
        [SerializeField] private bool useRotation;
        [SerializeField] private bool useGlobalRotation;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private float openRotationDeg;
        [SerializeField] private float closeRotationDeg;
        
        [Header("Animation Settings")]
        [SerializeField] private float duration = 1f;
        [SerializeField, Range(0, 1)] private float openThreshold = 0.5f;
        [SerializeField] private StartState startState = StartState.None;
        [SerializeField] private Animator animator;
        [SerializeField] private bool reverseAnimation;

        [FormerlySerializedAs("doorOpenSound")]
        [Header("FMOD Events")]
        [SerializeField] private StudioEventEmitter doorStartOpenSound;
        [SerializeField] private StudioEventEmitter doorOpenedSound;
        [SerializeField] private StudioEventEmitter doorStartCloseSound;
        [SerializeField] private StudioEventEmitter doorClosedSound;

        [Header("Script Override")]
        [SerializeField] public bool scriptOverride;
        [SerializeField] public bool scriptOverrideCallbacks = true;

        [Header("Callbacks")]
        public UnityEvent onDoorOpen;
        public UnityEvent onDoorClose;
        public UnityEvent<float> onOpennessUpdate;

        public bool IsOpen => _openness > openThreshold;
        public float Openness => _openness;
        
        private float _openness;
        private bool _isOpening;
        private bool _isClosing;
        private Rigidbody _gateRb;
        
        // Start is called before the first frame update
        private void Start()
        {
            _gateRb = gate.GetComponent<Rigidbody>();

            switch (startState)
            {
                case StartState.Open:
                    _openness = 1;
                    OnUpdateOpenness();
                    ApplyPosition(openPosition);
                    ApplyRotation(openRotationDeg);
                    UpdateAnimation();
                    break;
                case StartState.Closed:
                    _openness = 0;
                    OnUpdateOpenness();
                    ApplyPosition(closePosition);
                    ApplyRotation(closeRotationDeg);
                    UpdateAnimation();
                    break;
            }
        }

        private void ApplyPosition(Vector3 pos)
        {
            if (!usePosition) return;

            // Use Rigidbody movement instead of Transform movement
            if (useRigidbodyPosition && _gateRb)
            {
                if (!useGlobalPosition)
                {
                    pos = gate.TransformPoint(pos);
                }
                _gateRb.MovePosition(pos);
                
                return;
            }
            
            if (useGlobalPosition)
            {
                gate.position = pos;
            }
            else
            {
                gate.localPosition = pos;
            }
        }

        private void ApplyRotation(float angleDeg)
        {
            if (!useRotation) return;
            
            if (useGlobalRotation)
            {
                gate.rotation = Quaternion.AngleAxis(angleDeg, rotationAxis);
            }
            else
            {
                gate.localRotation = Quaternion.AngleAxis(angleDeg, rotationAxis);
            }
        }

        private void OnStartOpen()
        {
            if (doorStartOpenSound)
            {
                doorStartOpenSound.Play();
            }
        }

        private void OnStartClose()
        {
            if (doorStartCloseSound)
            {
                doorStartCloseSound.Play();
            }
        }

        private void OnUpdateOpenness()
        {
            onOpennessUpdate?.Invoke(_openness);
        }

        public void OpenDoor()
        {
            _isOpening = true;
            _isClosing = false;
            OnStartOpen();
        }

        public void CloseDoor()
        {
            _isClosing = true;
            _isOpening = false;
            OnStartClose();
        }

        public void ToggleDoor()
        {
             _isClosing = IsOpen;
             _isOpening = !IsOpen;
             if (_isOpening)
             {
                 OnStartOpen();
             }
             else
             {
                 OnStartClose();
             }
        }

        public void ApplyOpenness(float openness)
        {
            var clampedOpenness = Mathf.Clamp01(openness);
            if (_openness < openness)
            {
                _isOpening = true;
            }

            if (_openness > openness)
            {
                _isClosing = true;
            }

            _openness = clampedOpenness;
            
            ApplyPosition(Vector3.LerpUnclamped(closePosition, openPosition, _openness));
            ApplyRotation(Mathf.LerpUnclamped(closeRotationDeg, openRotationDeg, _openness));
            UpdateAnimation();
            
            // Note: when being overridden by script, these callbacks are called *AFTER* the door's position and rotation are applied
            if (!scriptOverrideCallbacks) return;

            if (_isOpening && openness >= 1)
            {
                _isOpening = false;
                OnDoorOpen();
            }
            
            if (_isClosing && openness <= 0)
            {
                _isClosing = false;
                OnDoorClose();
            }
            
            OnUpdateOpenness();
        }

        public void IncrementOpenness(float increment)
        {
            ApplyOpenness(_openness + increment);
        }

        private void OnDoorOpen()
        {
            if (doorOpenedSound)
            {
                doorOpenedSound.Play();
            }
            onDoorOpen?.Invoke();
        }

        private void OnDoorClose()
        {
            if (doorClosedSound)
            {
                doorClosedSound.Play();
            }
            onDoorClose?.Invoke();
        }

        private void Update()
        {
            if (scriptOverride || Time.timeScale == 0) return;
            
            if (_isOpening)
            {
                _openness += Time.deltaTime / duration;
            }

            if (_isClosing)
            {
                _openness -= Time.deltaTime / duration;
            }

            if (_openness > 1)
            {
                _openness = 1;
                _isOpening = false;
                OnDoorOpen();
            }

            if (_openness < 0)
            {
                _openness = 0;
                _isClosing = false;
                OnDoorClose();
            }
            
            OnUpdateOpenness();
            ApplyPosition(Vector3.LerpUnclamped(closePosition, openPosition, _openness));
            ApplyRotation(Mathf.LerpUnclamped(closeRotationDeg, openRotationDeg, _openness));
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (!animator) return;
            
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            var openness = reverseAnimation ? 1 - _openness : _openness;
            animator.Play(stateInfo.fullPathHash, 0, openness);
            animator.speed = 0;
        }
    }
}