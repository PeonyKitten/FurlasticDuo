using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class SecurityNPCChaseState : SecurityNPCBaseState
    {
        [FormerlySerializedAs("GoToPatrolStateName")] public string goToPatrolStateName = "Patrolling";

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Security NPC is chasing.");
            Security.chasePlayerBehaviour.Weight = 1;
            Security.SetSpeedForState("Chasing");
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Security.playerDetection.IsAnyPlayerInRange())
            {
                fsm.ChangeState(goToPatrolStateName);
            }
            else
            {
                var closestPlayer = Security.playerDetection.GetClosestPlayer();
                if (Security.chasePlayerBehaviour.player is not null && closestPlayer is null)
                {
                    Debug.Log("Lost Players :(");
                }
                Security.chasePlayerBehaviour.player = closestPlayer;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Security.chasePlayerBehaviour.Weight = 0;
            Security.chasePlayerBehaviour.player = null;
            Security.PlayAnimation("Alert");
        }
    }
}