using UnityEngine;
using UnityEngine.Events;

public class SecurityNPCPatrolState : SecurityNPCBaseState
{
    public string GoToChaseStateName = "Chasing";
    public string GoToInvestigateStateName = "Investigate";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Security NPC is patrolling.");
        securityNPC.SetSteeringBehaviourWeight(securityNPC.followPathBehaviour, 1);
        securityNPC.followPathBehaviour.onReachWaypoint.AddListener(OnReachWaypoint);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (securityNPC.IsPlayerInFOV())
        {
            fsm.ChangeState(GoToChaseStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        securityNPC.SetSteeringBehaviourWeight(securityNPC.followPathBehaviour, 0);
        securityNPC.followPathBehaviour.onReachWaypoint.RemoveListener(OnReachWaypoint);
    }

    private void OnReachWaypoint(Vector3 waypoint)
    {
        securityNPC.PlayAnimation("Idle");
    }
}