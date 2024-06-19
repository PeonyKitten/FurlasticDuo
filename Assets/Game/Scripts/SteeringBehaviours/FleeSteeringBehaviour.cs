using System;
using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public class FleeSteeringBehaviour: SeekSteeringBehaviour
    {
        [Header("Flee Behaviour")]
        [SerializeField] private Transform enemy;
        [SerializeField] protected float fleeDistance = 5f;
        
        public Transform Enemy { get => enemy; set => enemy = value; }

        private bool _showGizmoArrows;

        public float FleeDistance => fleeDistance;

        public override Vector3 CalculateForce()
        {
            CalculateSeekForce();
            
            if (enemy)
            {
                Target = enemy.position;
                useMouseInput = false;
            }
            
            if (Vector3.Distance(transform.position, Target) > fleeDistance)
            {
                _showGizmoArrows = false;
                return Vector3.zero;
            }
            
            _showGizmoArrows = true;
            
            // Reverse the direction because want to get away
            DesiredVelocity = -DesiredVelocity;
            
            return (DesiredVelocity - steeringAgent.Velocity);
        }

        protected override void OnDrawGizmos()
        {
            if (!_showGizmoArrows) return;

            if (steeringAgent != null)
            {
                var position = transform.position;
                
                DebugExtension.DebugArrow(position, DesiredVelocity, Color.red);
                DebugExtension.DebugArrow(position, steeringAgent.Velocity, Color.blue);
            }
            
            DebugExtension.DrawCircle(Target, Vector3.up, Color.green, fleeDistance);
        }
    }
}