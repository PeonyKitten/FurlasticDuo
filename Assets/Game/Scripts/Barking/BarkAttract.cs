using UnityEngine;
using UnityEngine.AI;

namespace FD.Barking
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BarkAttract : MonoBehaviour, IBarkReaction
    {
        [SerializeField] private float attractSpeedMultiplier = 0.5f;
        public bool IsReacting { get; set; }

        private NavMeshAgent _agent;
        private Vector3 _barkOrigin;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void React(Bark bark)
        {
            IsReacting = true;
            _barkOrigin = bark.transform.position;
            _agent.speed *= attractSpeedMultiplier;
            _agent.SetDestination(bark.transform.position);
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
