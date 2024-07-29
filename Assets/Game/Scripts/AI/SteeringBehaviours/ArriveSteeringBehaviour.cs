using FD.Utils;
using UnityEngine;

namespace FD.AI.SteeringBehaviours
{
    public class ArriveSteeringBehaviour: SeekSteeringBehaviour
    {
        [Header("Arrive Behaviour")]
        [SerializeField] protected float slowdownDistance = 2f;
        [SerializeField] protected float stoppingDistance = 0.1f;

        public override Vector3 CalculateForce()
        {
            return CalculateArriveForce();
        }

        protected Vector3 CalculateArriveForce()
        {
            var toTarget = Target - transform.position;
            var distance = toTarget.magnitude;
            
            if (distance > slowdownDistance)
            {
                return CalculateSeekForce();
            }

            if (distance < stoppingDistance)
            {
                steeringAgent.reachedGoal = true;
                return Vector3.zero;
            }
            
            toTarget.Normalize();

            var speed = steeringAgent.maxSpeed * (distance / slowdownDistance);
            speed = Mathf.Min(speed, steeringAgent.maxSpeed);

            DesiredVelocity = (speed / distance) * toTarget;

            return DesiredVelocity - steeringAgent.Velocity;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            DebugExtension.DebugCircle(Target, Vector3.up, Color.red, slowdownDistance);
        }
    }
}