using UnityEngine;

public class SecurityNPCChaseState : SecurityNPCBaseState
{
    public string GoToAlertStateName = "Alert";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Security NPC is coming for you.");
        securityNPC.SetSteeringBehaviourWeight(securityNPC.chasePlayerBehaviour, 1);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!securityNPC.IsPlayerInChaseRange())
        {
            fsm.ChangeState(GoToAlertStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        securityNPC.SetSteeringBehaviourWeight(securityNPC.chasePlayerBehaviour, 0);
        securityNPC.chasePlayerBehaviour.ResetPlayer();
    }
}