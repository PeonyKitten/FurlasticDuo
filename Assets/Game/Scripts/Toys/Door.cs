using System;
using UnityEngine;

namespace Game.Scripts.Toys
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

        [SerializeField] private bool scriptOverride;

        public bool IsOpen => _openness > openThreshold;
        public float Openness => _openness;
        
        private float _openness;
        private bool _isOpening;
        private bool _isClosing;
        
        // Start is called before the first frame update
        private void Start()
        {
            if (scriptOverride) return;

            switch (startState)
            {
                case StartState.Open:
                    _openness = 1;
                    ApplyPosition(openPosition);
                    ApplyRotation(openRotationDeg);
                    break;
                case StartState.Closed:
                    _openness = 0;
                    ApplyPosition(closePosition);
                    ApplyRotation(closeRotationDeg);
                    break;
            }
        }

        private void ApplyPosition(Vector3 pos)
        {
            if (!usePosition) return;
            
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

        public void OpenDoor()
        {
            _isOpening = true;
            _isClosing = false;
        }

        public void CloseDoor()
        {
            _isClosing = true;
            _isOpening = false;
        }

        public void ToggleDoor()
        {
             _isClosing = IsOpen;
             _isOpening = !IsOpen;
        }

        public void ApplyOpenness(float openness)
        {
            _openness = Mathf.Clamp01(openness);


            ApplyPosition(Vector3.LerpUnclamped(closePosition, openPosition, _openness));
            ApplyRotation(Mathf.LerpUnclamped(closeRotationDeg, openRotationDeg, _openness));
        }

        public void IncrementOpenness(float increment)
        {
            ApplyOpenness(_openness + increment);
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
            }

            if (_openness < 0)
            {
                _openness = 0;
                _isClosing = false;
            }

            ApplyPosition(Vector3.LerpUnclamped(closePosition, openPosition, _openness));
            ApplyRotation(Mathf.LerpUnclamped(closeRotationDeg, openRotationDeg, _openness));
        }
    }
}
