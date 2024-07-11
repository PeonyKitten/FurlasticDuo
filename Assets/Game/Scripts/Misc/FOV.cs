using System;
using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.Misc {
    public class FOV : Jail
    {
        [Header("FOV Settings")]
        [Tooltip("Half-angle for the FOV, in degrees.")]
        [SerializeField, Range(0, 180)] private float angleDeg = 45f;
        
        [Header("Debug Settings")]
        [SerializeField] private Transform debugTarget;
        [SerializeField] private Color defaultColor = Color.green;
        [SerializeField] private Color targetInsideColor = Color.red;

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

        public void DrawFOV()
        {
            DrawFOV(Color.yellow);
        }

        public void DrawFOV(Color color)
        {
            var startDirection =  Quaternion.AngleAxis(-angleDeg, transform.up) * transform.forward;
            var endDirection = Quaternion.AngleAxis(angleDeg, transform.up) * transform.forward;
            DebugExtension.DebugArc(transform.position, transform.up, transform.forward, angleDeg * 2, color, radius);
            if (boundsShape == BoundsShape.Cylindrical)
            {
                if (!useCylinderHeight) return;
                
                var upDirection = transform.up * cylinderHeight;
                var position = transform.position + upDirection;
                DebugExtension.DebugArc(position, transform.up, transform.forward, angleDeg * 2, color, radius);
                Debug.DrawRay(position, startDirection * radius, color);
                Debug.DrawRay(position, endDirection * radius, color);
                Debug.DrawLine(transform.position, position, color);
                Debug.DrawRay(transform.position + startDirection * radius, upDirection, color);
                Debug.DrawRay(transform.position + endDirection * radius, upDirection, color);
            }
            else
            {
                var circleForward = Vector3.Project(startDirection, transform.forward) * radius;
                
                var circleCenter = transform.position + circleForward;
                var circleRadius = Mathf.Sin(angleDeg * Mathf.Deg2Rad) * radius;
                
                var smallCircleCenter = angleDeg <= 90 ? transform.position + circleForward * 0.5f: transform.position;
                var smallCircleRadius = angleDeg <= 90 ? circleRadius * 0.5f: radius;
                
                DebugExtension.DebugCircle(circleCenter, transform.forward, color, circleRadius);
                DebugExtension.DebugCircle(smallCircleCenter, transform.forward, color, smallCircleRadius);
                DebugExtension.DebugArc(transform.position, transform.right, transform.forward, angleDeg * 2, color, radius);
            }
        }

        protected override void OnDrawGizmos()
        {
            if (debugTarget)
            {
                Gizmos.color = IsContained(debugTarget.position) ? targetInsideColor : defaultColor;
            }
            
            var startDirection =  Quaternion.AngleAxis(-angleDeg, transform.up) * transform.forward;
            var endDirection = Quaternion.AngleAxis(angleDeg, transform.up) * transform.forward;
            DebugExtension.DrawArc(transform.position, transform.up, transform.forward, angleDeg * 2, Gizmos.color, radius);
            if (boundsShape == BoundsShape.Cylindrical)
            {
                if (!useCylinderHeight) return;
                
                var upDirection = transform.up * cylinderHeight;
                var position = transform.position + upDirection;
                DebugExtension.DrawArc(position, transform.up, transform.forward, angleDeg * 2, Gizmos.color, radius);
                Gizmos.DrawRay(position, startDirection * radius);
                Gizmos.DrawRay(position, endDirection * radius);
                Gizmos.DrawLine(transform.position, position);
                Gizmos.DrawRay(transform.position + startDirection * radius, upDirection);
                Gizmos.DrawRay(transform.position + endDirection * radius, upDirection);
            }
            else
            {
                var circleForward = Vector3.Project(startDirection, transform.forward) * radius;
                
                var circleCenter = transform.position + circleForward;
                var circleRadius = Mathf.Sin(angleDeg * Mathf.Deg2Rad) * radius;
                
                var smallCircleCenter = angleDeg <= 90 ? transform.position + circleForward * 0.5f: transform.position;
                var smallCircleRadius = angleDeg <= 90 ? circleRadius * 0.5f: radius;
                
                DebugExtension.DrawCircle(circleCenter, transform.forward, Gizmos.color, circleRadius);
                DebugExtension.DrawCircle(smallCircleCenter, transform.forward, Gizmos.color, smallCircleRadius);
                DebugExtension.DrawArc(transform.position, transform.right, transform.forward, angleDeg * 2, Gizmos.color, radius);
            }
        }
    }
}