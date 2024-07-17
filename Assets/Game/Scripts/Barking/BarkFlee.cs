using UnityEngine;
using UnityEngine.AI;

namespace FD.Barking
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BarkFlee : MonoBehaviour, IBarkReaction
    {
        public float fleeSpeedMultiplier = 2f;
        public bool IsReacting { get; set; }

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        void IBarkReaction.React(Bark bark)
        {
            IsReacting = true;
            _agent.speed *= fleeSpeedMultiplier;

            var fleeDirection = (transform.position - bark.transform.position).normalized;
            var fleePosition = transform.position + fleeDirection * fleeSpeedMultiplier;
            if (!_agent.SetDestination(fleePosition))
            {
                Debug.Log("Couldn't get NavMesh destination");
            }

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
