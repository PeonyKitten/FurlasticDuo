using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Misc
{
    public class MultipleActivation : MonoBehaviour
    {
        [SerializeField] private int requiredActivations = 2;
        [SerializeField] private bool disallowAdditionalActivations;

        [Header("Callbacks")]
        public UnityEvent onActivated;
        public UnityEvent onDeactivated;
        
        public bool IsActivated { get; private set; }
        public int ActivationCount { get; private set; }
        
        private void Update()
        {
            var wasActivated = IsActivated;
            
            if (disallowAdditionalActivations)
            {
                IsActivated = ActivationCount == requiredActivations;
            }
            else
            {
                IsActivated = ActivationCount >= requiredActivations;
            }

            if (IsActivated == wasActivated) return;
            
            if (IsActivated)
            {
                onActivated.Invoke();
            }
            else
            {
                onDeactivated.Invoke();
            }
        }

        public void AddActivation()
        {
            ActivationCount += 1;
        }

        public void RemoveActivation()
        {
            ActivationCount -= 1;
            if (ActivationCount < 0)
            {
                Debug.LogWarning(name + " has negative activations.");
            }
        }

        public void OnDrawGizmosSelected()
        {
            Handles.color = Color.white;
            Handles.DrawSolidArc(transform.position, transform.forward, transform.right, 360f, 0.2f);
            var percentActivated = Mathf.Min((float) ActivationCount, requiredActivations) / requiredActivations;
            Handles.color = Color.Lerp(Color.red, Color.green, percentActivated);
            if (ActivationCount > requiredActivations)
            {
                Handles.color = disallowAdditionalActivations ? Color.yellow : Color.blue;
            }
            Handles.DrawSolidArc(transform.position, transform.forward, transform.right, percentActivated * 360f, 0.2f);
        }
    }
}
