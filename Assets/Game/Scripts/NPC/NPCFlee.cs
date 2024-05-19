using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Barking
{
    public class NPCFlee : MonoBehaviour, INPCReaction
    {
        public float fleeSpeedMultiplier = 2f;
        public bool IsReacting { get; set; }

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void ReactToBark(Vector3 barkOrigin)
        {
            IsReacting = true;
            _agent.speed *= fleeSpeedMultiplier;

            var fleeDirection = (transform.position - barkOrigin).normalized;
            var fleePosition = transform.position + fleeDirection * fleeSpeedMultiplier;
            _agent.SetDestination(fleePosition);

            // TODO: clean up
            Invoke("StopReacting", 3f); // Assuming reaction lasts 3 seconds
        }

        private void StopReacting()
        {
            IsReacting = false;
            _agent.speed /= fleeSpeedMultiplier;
        }
    }
}
