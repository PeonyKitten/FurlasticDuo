using System;
using FD.Utils;
using UnityEngine;

namespace FD.Misc
{
    public class Jail : MonoBehaviour
    {
        [Serializable]
        public enum BoundsShape
        {
            Cylindrical,
            Spherical
        }

        [SerializeField] protected BoundsShape boundsShape = BoundsShape.Cylindrical;
        [SerializeField] protected bool useCylinderHeight = true;
        [SerializeField] protected float cylinderHeight = 1f;
        public float radius = 10f;
        
        public virtual bool IsContained(Vector3 targetPosition) {
            if (boundsShape == BoundsShape.Spherical)
            {
                return (transform.position - targetPosition).IsWithinSphere(radius);
            }

            if (useCylinderHeight)
            {
                var height = targetPosition.y - transform.position.y;

                if (height > cylinderHeight || height < 0) return false;
            }
            
            // Fun fact: there is no global standard for how high of an altitude a nation's airspace extends to
            var target2D = targetPosition.Flatten();
            var position2D = transform.position.Flatten();

            return (position2D - target2D).IsWithinCircle(radius);
        }
        
        protected virtual void OnDrawGizmos()
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
    }
}
