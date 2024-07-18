using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Misc
{
    public class MultipleActivation : MonoBehaviour
    {
        [SerializeField] private int requiredActivations = 2;
        [SerializeField] private bool disallowAdditionalActivations;

        [Header("Callbacks")] 
        public UnityEvent onAddActivation;
        public UnityEvent onRemoveActivation;
        public UnityEvent onActivated;
        public UnityEvent onDeactivated;
        public UnityEvent onNoActivations;
        
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
            onAddActivation?.Invoke();
        }

        public void RemoveActivation()
        {
            ActivationCount -= 1;
            if (ActivationCount < 0)
            {
                Debug.LogWarning(name + " has negative activations.");
            }
            onRemoveActivation?.Invoke();
            if (ActivationCount == 0)
            {
                onNoActivations?.Invoke();
            }
        }

        public void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            Handles.color = Color.white;
            Handles.DrawSolidArc(transform.position, transform.forward, transform.right, 360f, 0.2f);
            var percentActivated = Mathf.Min((float) ActivationCount, requiredActivations) / requiredActivations;
            Handles.color = Color.Lerp(Color.red, Color.green, percentActivated);
            if (ActivationCount > requiredActivations)
            {
                Handles.color = disallowAdditionalActivations ? Color.yellow : Color.blue;
            }
            Handles.DrawSolidArc(transform.position, transform.forward, transform.right, percentActivated * 360f, 0.2f);
            #endif
        }
    }
}
