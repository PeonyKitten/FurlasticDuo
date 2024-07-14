using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public class SeekSteeringBehaviour: SteeringBehaviour
    {
        protected Vector3 DesiredVelocity = Vector3.zero;
        
        public override Vector3 CalculateForce()
        {
            CheckMouseInput();

            return CalculateSeekForce();
        }

        protected Vector3 CalculateSeekForce()
        {
            DesiredVelocity = (Target - transform.position).normalized * steeringAgent.maxSpeed;

            return (DesiredVelocity - steeringAgent.Velocity);
        }

        protected virtual void OnDrawGizmos()
        {
            DebugExtension.DrawCircle(Target, Vector3.up, Color.green);
        }
    }
}