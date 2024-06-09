using UnityEngine;
using Game.Scripts.Barking;
using Game.Scripts.SteeringBehaviours;
using UnityEngine.AI;
using System.Collections;

namespace Game.Scripts.NPC
{
    public class BarkFlee : MonoBehaviour, IBarkReaction
    {
        public float fleeSpeedMultiplier = 2f;
        public bool IsReacting { get; set; }

        private NavMeshAgent _agent;
        private SteeringAgent _steeringAgent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _steeringAgent = GetComponent<SteeringAgent>();
        }

        void IBarkReaction.React(Bark bark)
        {
            IsReacting = true;
            _agent.speed *= fleeSpeedMultiplier;

            var fleeDirection = (transform.position - bark.transform.position).normalized;
            var fleePosition = transform.position + fleeDirection * fleeSpeedMultiplier;
            

            _steeringAgent.OverrideSteering(fleePosition);
            StartCoroutine(StopReactingAfterTime(3f));
        }

        private void StopReacting()
        {
            IsReacting = false;
            _agent.speed /= fleeSpeedMultiplier;
            _steeringAgent.ClearOverride();
        }

        private IEnumerator StopReactingAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            StopReacting();
        }
    }
}
