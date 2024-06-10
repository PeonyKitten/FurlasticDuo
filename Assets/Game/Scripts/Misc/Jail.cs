using System;
using Game.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts
{
    public class Jail : MonoBehaviour
    {
        [Serializable]
        public enum BoundsShape
        {
            Cylindrical,
            Spherical
        }

        [SerializeField] private BoundsShape boundsShape = BoundsShape.Cylindrical;
        public float radius = 10f;
        
        public void OnDrawGizmos()
        {
            if (boundsShape == BoundsShape.Cylindrical)
            {
                DebugExtension.DebugCircle(transform.position, Color.magenta, radius);
            }
            else
            {
                DebugExtension.DebugWireSphere(transform.position, Color.magenta, radius);
            }
        }
        
        public bool IsContained(Vector3 targetPosition) {
            if (boundsShape == BoundsShape.Spherical)
            {
                return (transform.position - targetPosition).IsWithinSphere(radius);
            }
            
            // Fun fact: there is no global standard for how high of an altitude a nation's airspace extends to
            var target2D = targetPosition.Flatten();
            var position2D = transform.position.Flatten();

            return (position2D - target2D).IsWithinCircle(radius);
        }
    }
}
