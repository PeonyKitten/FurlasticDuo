using System;
using Game.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Misc {
    public class FOV : Jail
    {
        [SerializeField, Range(0, 180)] private float angleDeg = 45f;
        [SerializeField] private Transform debugTarget;

        public override bool IsContained(Vector3 targetPosition)
        { 
            if (!base.IsContained(targetPosition)) return false;

            var toTarget = targetPosition - transform.position;
            return boundsShape switch
            {
                BoundsShape.Cylindrical => Vector2.Angle(transform.forward.Flatten(), toTarget.Flatten()) <= angleDeg,
                BoundsShape.Spherical => Vector3.Angle(transform.forward, toTarget) <= angleDeg,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            if (debugTarget)
            {
                Gizmos.color = IsContained(debugTarget.position) ? Color.red : Color.green;
            }
            
            var startDirection =  Quaternion.AngleAxis(-angleDeg, transform.up) * transform.forward;
            var endDirection = Quaternion.AngleAxis(angleDeg, transform.up) * transform.forward;
            Handles.color = Gizmos.color;
            Handles.DrawWireArc(transform.position, transform.up, startDirection, angleDeg * 2, radius);
            Gizmos.DrawRay(transform.position, startDirection * radius);
            Gizmos.DrawRay(transform.position, endDirection * radius);
            if (boundsShape == BoundsShape.Cylindrical)
            {
                if (!useCylinderHeight) return;
                
                var position = transform.position + transform.up * cylinderHeight;
                Handles.DrawWireArc(position, transform.up, startDirection, angleDeg * 2, radius);
                Gizmos.DrawRay(position, startDirection * radius);
                Gizmos.DrawRay(position, endDirection * radius);
                Gizmos.DrawLine(transform.position, position);
                Gizmos.DrawRay(transform.position + startDirection * radius, transform.up * cylinderHeight);
                Gizmos.DrawRay(transform.position + endDirection * radius, transform.up * cylinderHeight);
            }
            else
            {
                var upDirection =  Quaternion.AngleAxis(-angleDeg, transform.right) * transform.forward;
                var downDirection = Quaternion.AngleAxis(angleDeg, transform.right) * transform.forward;
                Gizmos.DrawRay(transform.position, upDirection * radius);
                Gizmos.DrawRay(transform.position, downDirection * radius);
                Handles.DrawWireArc(transform.position, transform.forward, startDirection, 360, radius);
                Handles.DrawWireArc(transform.position, transform.right, upDirection, angleDeg * 2, radius);
            }
            #endif
        }
    }
}