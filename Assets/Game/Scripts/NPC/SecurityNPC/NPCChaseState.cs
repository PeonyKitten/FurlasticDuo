using UnityEngine;

public class SecurityNPCChaseState : SecurityNPCBaseState
{
    public string GoToPatrolStateName = "Patrolling";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Security NPC is chasing.");
        securityNPC.chasePlayerBehaviour.Weight = 1;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!securityNPC.playerDetection.IsAnyPlayerInRange())
        {
            fsm.ChangeState(GoToPatrolStateName);
        }
        else
        {
            Transform closestPlayer = securityNPC.playerDetection.GetClosestPlayer();
            if (closestPlayer != null)
            {
                securityNPC.chasePlayerBehaviour.player = closestPlayer;
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        securityNPC.chasePlayerBehaviour.Weight = 0;
        securityNPC.chasePlayerBehaviour.player = null;
        securityNPC.PlayAnimation("Alert");
    }
}