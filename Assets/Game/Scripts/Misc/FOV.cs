using System;
using System.Collections;
using System.Collections.Generic;
using FD.Utils;
using UnityEngine;

namespace FD.Misc {
    public class FOV : Jail
    {
        [Header("FOV Settings")]
        [Tooltip("Half-angle for the FOV, in degrees.")]
        [SerializeField, Range(0, 180)] private float viewAngle = 45f;
        public float viewRadius;

        [Header("Mesh Settings")]
        public float meshResolution = 1;
        public int edgeResolveIterations = 4;
        public float edgeDstThreshold = 0.5f;

        [Header("Target Detection")]
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        [Header("Visualization")]
        public MeshFilter viewMeshFilter;
        private Mesh viewMesh;

        [HideInInspector]
        public List<Transform> visibleTargets = new List<Transform>();

        [Header("Debug Settings")]
        [SerializeField] private Transform debugTarget;
        [SerializeField] private Color defaultColor = Color.green;
        [SerializeField] private Color targetInsideColor = Color.red;

        // Currently we have isContained and visibleTargets list as well, we do not need both. 
        void Start()
        {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;
            StartCoroutine("FindTargetsWithDelay", .2f);
        }

        void LateUpdate()
        {
            DrawFieldOfView();
        }

        IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.pointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointA);
                        }
                        if (edge.pointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointB);
                        }
                    }
                }

                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
            }
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public override bool IsContained(Vector3 targetPosition)
        {
            if (!base.IsContained(targetPosition)) return false;

            var toTarget = targetPosition - transform.position;
            return boundsShape switch
            {
                BoundsShape.Cylindrical => Vector2.Angle(transform.forward.Flatten(), toTarget.Flatten()) <= viewAngle,
                BoundsShape.Spherical => Vector3.Angle(transform.forward, toTarget) <= viewAngle,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void DrawFOV()
        {
            DrawFOV(Color.yellow);
        }

        public void DrawFOV(Color color)
        {
            var startDirection =  Quaternion.AngleAxis(-viewAngle, transform.up) * transform.forward;
            var endDirection = Quaternion.AngleAxis(viewAngle, transform.up) * transform.forward;
            DebugExtension.DebugArc(transform.position, transform.up, transform.forward, viewAngle * 2, color, radius);
            if (boundsShape == BoundsShape.Cylindrical)
            {
                if (!useCylinderHeight) return;
                
                var upDirection = transform.up * cylinderHeight;
                var position = transform.position + upDirection;
                DebugExtension.DebugArc(position, transform.up, transform.forward, viewAngle * 2, color, radius);
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
                var circleRadius = Mathf.Sin(viewAngle * Mathf.Deg2Rad) * radius;
                
                var smallCircleCenter = viewAngle <= 90 ? transform.position + circleForward * 0.5f: transform.position;
                var smallCircleRadius = viewAngle <= 90 ? circleRadius * 0.5f: radius;
                
                DebugExtension.DebugCircle(circleCenter, transform.forward, color, circleRadius);
                DebugExtension.DebugCircle(smallCircleCenter, transform.forward, color, smallCircleRadius);
                DebugExtension.DebugArc(transform.position, transform.right, transform.forward, viewAngle * 2, color, radius);
            }
        }

        protected override void OnDrawGizmos()
        {
            if (debugTarget)
            {
                Gizmos.color = IsContained(debugTarget.position) ? targetInsideColor : defaultColor;
            }
            
            var startDirection =  Quaternion.AngleAxis(-viewAngle, transform.up) * transform.forward;
            var endDirection = Quaternion.AngleAxis(viewAngle, transform.up) * transform.forward;
            DebugExtension.DrawArc(transform.position, transform.up, transform.forward, viewAngle * 2, Gizmos.color, radius);
            if (boundsShape == BoundsShape.Cylindrical)
            {
                if (!useCylinderHeight) return;
                
                var upDirection = transform.up * cylinderHeight;
                var position = transform.position + upDirection;
                DebugExtension.DrawArc(position, transform.up, transform.forward, viewAngle * 2, Gizmos.color, radius);
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
                var circleRadius = Mathf.Sin(viewAngle * Mathf.Deg2Rad) * radius;
                
                var smallCircleCenter = viewAngle <= 90 ? transform.position + circleForward * 0.5f: transform.position;
                var smallCircleRadius = viewAngle <= 90 ? circleRadius * 0.5f: radius;
                
                DebugExtension.DrawCircle(circleCenter, transform.forward, Gizmos.color, circleRadius);
                DebugExtension.DrawCircle(smallCircleCenter, transform.forward, Gizmos.color, smallCircleRadius);
                DebugExtension.DrawArc(transform.position, transform.right, transform.forward, viewAngle * 2, Gizmos.color, radius);
            }
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float dst;
            public float angle;

            public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
            {
                hit = _hit;
                point = _point;
                dst = _dst;
                angle = _angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
            {
                pointA = _pointA;
                pointB = _pointB;
            }
        }

    }
}