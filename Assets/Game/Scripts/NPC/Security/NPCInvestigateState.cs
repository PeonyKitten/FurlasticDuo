using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class NPCInvestigateState : SecurityNPCBaseState
    {
        [FormerlySerializedAs("GoToPatrolStateName")] public string goToPatrolStateName = "Patrolling";
        [FormerlySerializedAs("GoToChaseStateName")] public string goToChaseStateName = "Chasing";

        private float _investigationTimer;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Security NPC is investigating a sound.");
            Security.PlayAnimation("Alert");
            Security.SetSpeedForState("Investigate");
            Security.barkAttractBehaviour.Weight = 1;
            Security.barkAttractBehaviour.Target = Security.barkOrigin;
            Security.barkAttractBehaviour.IsReacting = true;
            _investigationTimer = Security.investigationResetTime;

        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _investigationTimer -= Time.deltaTime;

            if (_investigationTimer <= 0)
            {
                fsm.ChangeState(goToPatrolStateName);
                return;
            }

            if (Vector3.Distance(Security.transform.position, Security.barkOrigin) < 0.1f)
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
            Security.barkAttractBehaviour.Weight = 0;
            Security.IsReacting = false;
        }
    }
}