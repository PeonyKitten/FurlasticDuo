using FD.AI.FSM;
using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class NPCInvestigateState : SecurityNPCBaseState
    {
        [SerializeField] private string goToPatrolStateName = "Patrolling";
        [SerializeField] private string goToChaseStateName = "Chasing";
        [SerializeField] private float investigationDuration = 5f;
        private float _investigationTimer;
        private Vector3 _barkOrigin;
        private BarkAttractSB _barkAttractBehaviour;

        public override void Init(GameObject owner, FSM fsm)
        {
            base.Init(owner, fsm);
            CachedSpeed = SecurityNPC.investigateSpeed;
        }

        private void SetBarkOrigin(Vector3 origin)
        {
            _barkOrigin = origin;
            _investigationTimer = investigationDuration;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Debug.Log("Security NPC is investigating a sound.");
            SecurityNPC.PlayAnimation("Alert");

            SetBarkOrigin(SecurityNPC.GetLastBarkOrigin());

            if (SteeringAgent.TryGetBehaviour(out _barkAttractBehaviour))
            {
                _barkAttractBehaviour.Weight = 1;
                _barkAttractBehaviour.Target = _barkOrigin;
                _barkAttractBehaviour.IsReacting = true;
            }
            _investigationTimer = investigationDuration;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _investigationTimer -= Time.deltaTime;
            if (_investigationTimer <= 0)
            {
                fsm.ChangeState(goToPatrolStateName);
                return;
            }
            if (Vector3.Distance(SecurityNPC.transform.position, _barkOrigin) < 0.1f)
            {
                SecurityNPC.PlayAnimation("Alert");
                fsm.ChangeState(goToPatrolStateName);
            }
            else if (PlayerDetection.CanSeePlayer(true))
            {
                fsm.ChangeState(goToChaseStateName);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_barkAttractBehaviour != null)
            {
                _barkAttractBehaviour.Weight = 0;
                _barkAttractBehaviour.IsReacting = false;
            }
        }
    }
}