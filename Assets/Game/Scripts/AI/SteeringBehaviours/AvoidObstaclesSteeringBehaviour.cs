using System;
using System.Collections.Generic;
using FD.Utils;
using UnityEngine;

namespace FD.AI.SteeringBehaviours
{
    public class AvoidObstaclesSteeringBehaviour : SteeringBehaviour
    {
        [Serializable]
        public class Feeler
        {
            public float distance;
            public Vector3 offset;
            public bool IsTriggered { set; get; } = false;
        }

        public List<Feeler> feelers = new();
        public LayerMask obstacleLayer;
        public float avoidWeight = 1f;

        public override Vector3 CalculateForce()
        {
            var position = transform.position;
            var forward = transform.forward;
            
            foreach (var feeler in feelers)
            {
                var feelerPos = transform.ProjectOffset(feeler.offset);
                var ray = new Ray(feelerPos, forward);
                
                feeler.IsTriggered = false;
                
                if (!Physics.Raycast(ray, out var hit, feeler.distance, obstacleLayer.value)) continue;

                var forceDir = Vector3.Project(hit.point - position, forward);
                var multiplier = 1 + (feeler.distance - forceDir.magnitude) / feeler.distance;

                var forcePos = forceDir + position;
                forceDir = (forcePos - hit.point).normalized * (multiplier * (1.0f / avoidWeight));

                feeler.IsTriggered = true;
                return forceDir;
            }

            return Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            var forward = transform.forward;
            
            foreach (var feeler in feelers)
            {
                var feelerPos = transform.ProjectOffset(feeler.offset);
                Debug.DrawLine(feelerPos, forward * feeler.distance + feelerPos, feeler.IsTriggered ? Color.red : Color.blue);
            }
        }
    }
}
