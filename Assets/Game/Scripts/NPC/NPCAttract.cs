using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Barking
{
    public class NPCAttract : MonoBehaviour, INPCReaction
    {
        [SerializeField] private float attractSpeedMultiplier = 0.5f;
        public bool IsReacting { get; set; }

        private NavMeshAgent _agent;
        private Vector3 _barkOrigin;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void ReactToBark(Vector3 barkOrigin)
        {
            IsReacting = true;
            _barkOrigin = barkOrigin;
            _agent.speed *= attractSpeedMultiplier;
            _agent.SetDestination(barkOrigin);
        }

        private void Update()
        {
            if (IsReacting && _agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending)
            {
                StopReacting();
            }
        }

        private void StopReacting()
        {
            IsReacting = false;
            _agent.speed /= attractSpeedMultiplier;
        }
    }
}