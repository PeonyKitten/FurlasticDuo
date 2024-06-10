using UnityEngine;
using Game.Scripts.Barking;
using Game.Scripts.SteeringBehaviours;
using UnityEngine.AI;
using System.Collections;

namespace Game.Scripts.NPC
{
    public class BarkFlee : FleeSteeringBehaviour, IBarkReaction
    {
        [SerializeField] private bool fleeBasedOnDistance = true;
        [SerializeField] private float fleeSpeedMultiplier = 3f;

        public bool IsReacting { get; set; }

        private NavMeshAgent _agent;
        private SteeringAgent _steeringAgent;
        private Coroutine _barkCoroutine;

        private void Awake()
        {
            //_agent = GetComponent<NavMeshAgent>();
            _steeringAgent = GetComponent<SteeringAgent>();
        }

        public void React(Bark bark)
        {
            IsReacting = true;
           // _agent.speed *= fleeSpeedMultiplier;
            Target = bark.transform.position;
            _steeringAgent.reachedGoal = false;

            if (_barkCoroutine != null)
            {
                StopCoroutine(_barkCoroutine);
            }

            _barkCoroutine = StartCoroutine(StopReactingAfterTime(3f));
        }

        public override Vector3 CalculateForce()
        {
            if (!IsReacting)
            {
                return Vector3.zero;
            }

            if (fleeBasedOnDistance)
            {
                return base.CalculateForce();
            }

            CalculateSeekForce();

            DesiredVelocity = -DesiredVelocity;

            return DesiredVelocity;
        }

        private void StopReacting()
        {
            IsReacting = false;
            // _agent.speed /= fleeSpeedMultiplier;
            _steeringAgent.reachedGoal = true;
        }

        private IEnumerator StopReactingAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            StopReacting();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (IsReacting)
            {
                DebugExtension.DrawCircle(transform.position, Vector3.up, Color.red, 1f);
            }
        }
    }
}
