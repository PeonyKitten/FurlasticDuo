using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class NPCInvestigateState : SecurityNPCBaseState
    {
        [FormerlySerializedAs("GoToPatrolStateName")] public string goToPatrolStateName = "Patrolling";
        [FormerlySerializedAs("GoToChaseStateName")] public string goToChaseStateName = "Chasing";

        [SerializeField] private float investigationDuration = 5f;
        private float _investigationTimer;
        private Vector3 _barkOrigin;
        private BarkAttractSB _barkAttractBehaviour;

        public void SetBarkOrigin(Vector3 origin)
        {
            _barkOrigin = origin;
            _investigationTimer = investigationDuration;

            Security.PlayAnimation("Alert");

            if (_barkAttractBehaviour != null)
            {
                _barkAttractBehaviour.Target = _barkOrigin;
            }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Security NPC is investigating a sound.");
            //Security.PlayAnimation("Alert");
            Security.SetSpeed(Security.GetSpeedForState("Investigate"));

            _barkAttractBehaviour = Security.steeringAgent.GetBehaviour<BarkAttractSB>();
            if (_barkAttractBehaviour != null)
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

            if (Vector3.Distance(Security.transform.position, _barkOrigin) < 0.1f)
            {
                Security.PlayAnimation("Alert");
                fsm.ChangeState(goToPatrolStateName);
            }
            else if (Security.playerDetection.CanSeePlayer(true))
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