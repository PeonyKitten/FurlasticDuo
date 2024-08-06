using System;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Misc
{
    public class TimedFuse : MonoBehaviour
    {
        [Serializable]
        public enum FuseBehaviour
        {
            RestartTimer,
            AddToTimer,
        }

        [Header("Timer Settings")]
        [SerializeField] private float delay;
        [SerializeField] private bool singleUse;
        [SerializeField] private FuseBehaviour fuseBehaviour = FuseBehaviour.RestartTimer;
        
        [Header("Callbacks")]
        public UnityEvent onTimerDone;

        private float _timer;
        private bool _isEnabled;
        private bool _disposed;

        public void LightFuse()
        {
            LightFuseWithDelay(delay);
        }

        public void LightFuseWithDelay(float customDelay)
        {
            if (_disposed) return;
            
            if (fuseBehaviour == FuseBehaviour.RestartTimer)
            {
                _timer = customDelay;
            }
            else
            {
                _timer += customDelay;
            }
            
            _isEnabled = true;
        }

        private void Update()
        {
            if (!_isEnabled) return;

            _timer -= Time.deltaTime;

            if (_timer > 0) return;
            
            onTimerDone?.Invoke();
            
            if (singleUse)
            {
                _disposed = true;
            }
            
            _isEnabled = false;

        }
    }
}
