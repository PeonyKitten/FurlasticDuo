using System;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Misc
{
    public class DistanceCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        [Serializable]
        public enum ConditionType
        {
            LessThan,
            GreaterThan,
            EqualTo,
            NotEqualTo,
            LessThanOrEqualTo,
            GreaterThanOrEqualTo
        }
        [SerializeField] private ConditionType conditionToCheck = ConditionType.LessThan;
        [SerializeField] private float distanceToCheck = 1f;

        public UnityEvent onConditionTrue;
        public UnityEvent onConditionFalse;

        void Update()
        {
            // Shoot a ray from the position of this GameObject up
            if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 100.0f, layerMask))
            {
                // Calculate the distance between the GameObject and the hit point
                float distanceToHit = hit.distance;

                bool result = CheckCondition(distanceToHit, distanceToCheck);
                if (result)
                {
                    onConditionTrue?.Invoke();
                }
                else 
                {
                    onConditionFalse?.Invoke();
                }


                // Draw the ray gizmo up to the hit point
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }

        private bool CheckCondition(float a, float b)
        {
            switch (conditionToCheck)
            {
                case ConditionType.LessThan:
                    return a < b;
                case ConditionType.GreaterThan:
                    return a > b;
                case ConditionType.EqualTo:
                    return a == b;
                case ConditionType.NotEqualTo:
                    return a != b;
                case ConditionType.LessThanOrEqualTo:
                    return a <= b;
                case ConditionType.GreaterThanOrEqualTo:
                    return a >= b;
                default:
                    Debug.LogError("Unknown condition type");
                    return false;
            }
        }
    }
}
