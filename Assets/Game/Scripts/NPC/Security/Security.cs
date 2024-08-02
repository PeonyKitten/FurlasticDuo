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

        private BarkAttractSB _barkAttractSB;
        private Vector3 _lastBarkOrigin;

        [Header("Speed Settings")]
        public float patrolSpeed = 1f;
        public float investigateSpeed = 1.5f;
        public float chaseSpeed = 2f;

        [Header("Behavior Settings")]
        public bool stayInPlace = false;

        public UnityEvent<Vector3> onBarkReaction = new UnityEvent<Vector3>();
        public Vector3 GetLastBarkOrigin() => _lastBarkOrigin;

        protected virtual void Start()
        {
            if (steeringAgent.TryGetBehaviour(out _barkAttractSB))
            {
                _barkAttractSB.onBarkReaction.AddListener(ReactToBark);
            }
        }
        public void SetSpeed(float speed)
        {
            steeringAgent.maxSpeed = speed;
            visualAnimator.SetFloat("Speed", speed);
        }

        public void PlayAlert()
        {
            visualAnimator.SetTrigger("Alert");
        }

        public void ReactToBark(Vector3 barkOrigin)
        {
            if (_barkAttractSB != null)
            {
                _lastBarkOrigin = barkOrigin;
            }
            ChangeState("Investigate");
        }

        private void OnDestroy()
        {
            if (_barkAttractSB != null)
            {
                _barkAttractSB.onBarkReaction.RemoveListener(ReactToBark);
            }
        }

        public void Reset()
        {
            steeringAgent.Reset();
            playerDetection.Reset();
        }
    }
}