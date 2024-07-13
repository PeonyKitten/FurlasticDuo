using UnityEngine;

public class SecurityNPCAlertState : SecurityNPCBaseState
{
    public string GoToPatrolStateName = "Patrolling";
    public string GoToChaseStateName = "Chasing";

    private SecurityNPCFSM securityNPCFSM;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("DID I HEAR SOMETHING?");
        securityNPCFSM = fsm as SecurityNPCFSM;
        securityNPC.SetSteeringBehaviourWeight(securityNPC.barkAttractBehaviour, 1);
        securityNPC.barkAttractBehaviour.Target = securityNPCFSM.barkOrigin;
        PlayAlertAnimation();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(securityNPC.transform.position, securityNPC.barkAttractBehaviour.Target) < 0.1f)
        {
            PlayAlertAnimation();
            fsm.ChangeState(GoToPatrolStateName);
        }
        else if (securityNPC.IsPlayerInFOV())
        {
            fsm.ChangeState(GoToChaseStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        securityNPC.SetSteeringBehaviourWeight(securityNPC.barkAttractBehaviour, 0);
    }

    private void PlayAlertAnimation()
    {
        Vector3 startPos = securityNPC.transform.position;
        Vector3 jumpHeight = Vector3.up * 0.5f;
        securityNPC.transform.position = startPos + jumpHeight;
    }
}