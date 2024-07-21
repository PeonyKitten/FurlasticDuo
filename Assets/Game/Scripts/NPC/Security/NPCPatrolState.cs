using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class SecurityNPCPatrolState : SecurityNPCBaseState
    {
        [FormerlySerializedAs("GoToChaseStateName")] public string goToChaseStateName = "Chasing";

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Security NPC is patrolling.");
            Security.followPathBehaviour.Weight = 1;
            Security.SetSpeedForState("Patrolling");
            Security.followPathBehaviour.onReachWaypoint.AddListener(OnReachWaypoint);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Security.playerDetection.GetClosestPlayer(true))
            {
                fsm.ChangeState(goToChaseStateName);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Security.followPathBehaviour.Weight = 0;
            Security.followPathBehaviour.onReachWaypoint.RemoveListener(OnReachWaypoint);
        }

        private void OnReachWaypoint(Vector3 waypoint)
        {
            Security.PlayAnimation("Idle");
        }
    }
}