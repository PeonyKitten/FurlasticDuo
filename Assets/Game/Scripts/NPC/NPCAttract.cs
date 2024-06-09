using UnityEngine;
using Game.Scripts.Barking;
using Game.Scripts.SteeringBehaviours;
using UnityEngine.AI;
using System.Collections;

namespace Game.Scripts.NPC
{
    public class NPCAttract : MonoBehaviour, IBarkReaction
    {
        [SerializeField] private float attractSpeedMultiplier = 1f;
        public bool IsReacting { get; set; }

        private NavMeshAgent _agent;
        private SteeringAgent _steeringAgent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _steeringAgent = GetComponent<SteeringAgent>();
        }

        public void React(Bark bark)
        {
            Debug.Log("React Triggered");
            IsReacting = true;
            _agent.speed *= attractSpeedMultiplier;
            Vector3 targetPosition = bark.transform.position;

            Debug.Log($"Running to {targetPosition}");
            _steeringAgent.OverrideSteering(targetPosition);
            StartCoroutine(StopReactingAfterTime(3f));
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
            _steeringAgent.ClearOverride();
        }

        private IEnumerator StopReactingAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            StopReacting();
        }
    }
}
