using FD.AI.FSM;
using FD.AI.SteeringBehaviours;
using UnityEngine;

namespace FD.NPC.Security
{
    public class Security : FSM
    {
        [SerializeField] private Animator visualAnimator;
        public SteeringAgent steeringAgent;
        public FollowPathSteeringBehaviour followPathBehaviour;
        public BarkAttractSB barkAttractBehaviour;
        public ChasePlayerSteeringBehaviour chasePlayerBehaviour;
        public DetectPlayer playerDetection;
        public bool IsReacting { get; set; }
        public Vector3 barkOrigin { get; private set; }

        [Header("Speed Settings")]
        public float patrolSpeed;
        public float investigateSpeed;
        public float chaseSpeed;

        private static readonly int IdleTrigger = Animator.StringToHash("IdleTrigger");
        private static readonly int AlertTrigger = Animator.StringToHash("AlertTrigger");

        public void ReactToBark(Vector3 barkOrigin)
        {
            this.barkOrigin = barkOrigin;
            IsReacting = true;
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

        public void SetSpeedForState(string stateName)
        {
            switch (stateName)
            {
                case "Patrolling":
                    steeringAgent.maxSpeed = patrolSpeed;
                    break;
                case "Investigate":
                    steeringAgent.maxSpeed = investigateSpeed;
                    break;
                case "Chasing":
                    steeringAgent.maxSpeed = chaseSpeed;
                    break;
                default:
                    Debug.LogWarning($"Unkniown State: {stateName}. Speed is not set");
                    break;

            }
        }
    }
}