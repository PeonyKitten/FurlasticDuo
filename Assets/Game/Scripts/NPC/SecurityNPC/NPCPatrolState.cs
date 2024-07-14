using UnityEngine;
using UnityEngine.Events;

public class SecurityNPCPatrolState : SecurityNPCBaseState
{
    public string GoToChaseStateName = "Chasing";
    public string GoToInvestigateStateName = "Investigate";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Security NPC is patrolling.");
        securityNPC.followPathBehaviour.Weight = 1;
        securityNPC.followPathBehaviour.onReachWaypoint.AddListener(OnReachWaypoint);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Transform detectedPlayer;
        if (securityNPC.playerDetection.IsPlayerInFOV(out detectedPlayer))
        {
            securityNPC.chasePlayerBehaviour.player = detectedPlayer;
            fsm.ChangeState(GoToChaseStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        securityNPC.followPathBehaviour.Weight = 0;
        securityNPC.followPathBehaviour.onReachWaypoint.RemoveListener(OnReachWaypoint);
    }

    private void OnReachWaypoint(Vector3 waypoint)
    {
        securityNPC.PlayAnimation("Idle");
    }
}