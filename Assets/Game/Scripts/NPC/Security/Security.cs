using FD.AI.FSM;
using FD.AI.SteeringBehaviours;
using FD.Levels.Checkpoints;
using UnityEngine;
using UnityEngine.Events;


namespace FD.NPC.Security
{
    public class Security : FSM, IReset
    {
        [SerializeField] private Animator visualAnimator;
        public SteeringAgent steeringAgent;
        public DetectPlayer playerDetection;

        public bool IsReacting { get; set; }
        public Vector3 barkOrigin { get; private set; }

        [Header("Speed Settings")]
        public float patrolSpeed;
        public float investigateSpeed;
        public float chaseSpeed;

        [Header("Stay In Place Settings")]
        public bool stayInPlace = false;

        private static readonly int IdleTrigger = Animator.StringToHash("IdleTrigger");
        private static readonly int AlertTrigger = Animator.StringToHash("AlertTrigger");

        public UnityEvent<Vector3> OnBarkReaction = new UnityEvent<Vector3>();

        public void ReactToBark(Vector3 barkOrigin)
        {
            var investigateState = fsmAnimator.GetBehaviour<NPCInvestigateState>();
            if (investigateState != null)
            {
                investigateState.SetBarkOrigin(barkOrigin);
            }
            ChangeState("Investigate");
        }

        public void PlayAnimation(string animationTrigger)
        {
            if (!visualAnimator) return;

            switch (animationTrigger)
            {
                case "Idle":
                    visualAnimator.SetTrigger(IdleTrigger);
                    break;
                case "Alert":
                    visualAnimator.SetTrigger(AlertTrigger);
                    break;
                default:
                    Debug.Log($"Animation: {animationTrigger}");
                    break;
            }
        }

        public void SetSpeed(float speed)
        {
            steeringAgent.maxSpeed = speed;
        }

        public float GetSpeedForState(string stateName)
        {
            switch (stateName)
            {
                case "Patrolling": return patrolSpeed;
                case "Investigate": return investigateSpeed;
                case "Chasing": return chaseSpeed;
                default:
                    Debug.LogWarning($"Unknown State: {stateName}. Defaulting to patrol speed.");
                    return patrolSpeed;
            }
        }

        public void Reset()
        {
            steeringAgent.Reset();
            playerDetection.Reset();
        }
    }
}