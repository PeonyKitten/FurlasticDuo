using Game.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.SteeringBehaviours
{
    public class WanderSteeringBehaviour: SeekSteeringBehaviour
    {
        [Header("Wander Behaviour")]
        [SerializeField, Tooltip("Target to wander around.")] protected Transform allRoadsLeadToRome;
        [SerializeField] protected float wanderDistance = 2f;
        [SerializeField] protected float wanderRadius = 1f;
        [SerializeField] protected float wanderJitter = 20f;

        protected Vector3 WanderTarget;

        protected virtual void Start()
        {
            if (allRoadsLeadToRome != null)
            {
                Target = allRoadsLeadToRome.position;
            }
            var theta = Random.value * Mathf.PI * 2f;
            WanderTarget = new Vector3(
                wanderRadius * Mathf.Cos(theta),
                0f,
                wanderRadius * Mathf.Sin(theta));
        }
        
        public override Vector3 CalculateForce()
        {
            var verySmallJitter = wanderJitter * Time.deltaTime;
            WanderTarget += new Vector3(
                Random.Range(-1f, 1f) * verySmallJitter,
                0,
                Random.Range(-1f, 1f) * verySmallJitter);
            
            WanderTarget.Normalize();
            WanderTarget *= wanderRadius;

            Target = WanderTarget + new Vector3(0, 0, wanderDistance);

            Target = transform.ProjectOffset(Target);

            return CalculateSeekForce();
        }

        protected override void OnDrawGizmos()
        {
            var position = transform.position;
            
            var circle = transform.ProjectOffset(Vector3.forward * wanderDistance);
            
            DebugExtension.DrawCircle(circle, Color.red, wanderRadius);
            
            if (!Application.isPlaying) return;
            
            Debug.DrawLine(position, circle, Color.yellow);
            Debug.DrawLine(position, Target, Color.blue);
        }
    }
}