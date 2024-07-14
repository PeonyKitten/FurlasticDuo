using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInvestigateState : SecurityNPCBaseState
{
    public string GoToPatrolStateName = "Patrolling";
    public string GoToChaseStateName = "Chasing";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Security NPC is investigating a sound.");
        securityNPC.PlayAnimation("Alert");
        securityNPC.barkAttractBehaviour.Weight = 1;
        securityNPC.barkAttractBehaviour.Target = securityNPC.barkOrigin;
        securityNPC.barkAttractBehaviour.IsReacting = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(securityNPC.transform.position, securityNPC.barkOrigin) < 0.1f)
        {
            securityNPC.PlayAnimation("Alert");
            fsm.ChangeState(GoToPatrolStateName);
        }
        else
        {
            Transform detectedPlayer;
            if (securityNPC.playerDetection.IsPlayerInFOV(out detectedPlayer))
            {
                securityNPC.chasePlayerBehaviour.player = detectedPlayer;
                fsm.ChangeState(GoToChaseStateName);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        securityNPC.barkAttractBehaviour.Weight = 0;
        securityNPC.IsReacting = false;
    }
}