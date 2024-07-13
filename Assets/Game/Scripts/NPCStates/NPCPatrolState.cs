using UnityEngine;

public class SecurityNPCPatrolState : SecurityNPCBaseState
{
    public string GoToIdleStateName = "Idle";
    public string GoToChaseStateName = "Chasing";

    [SerializeField] private float velocityThreshold = 0.1f; 
    [SerializeField] private float lowVelocityDuration = 0.1f; 
    private float lowVelocityTimer = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Security NPC is patrolling.");
        securityNPC.SetSteeringBehaviourWeight(securityNPC.followPathBehaviour, 1);
        lowVelocityTimer = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (securityNPC.steeringAgent.Velocity.magnitude < velocityThreshold)
        {
            lowVelocityTimer += Time.deltaTime;
            if (lowVelocityTimer >= lowVelocityDuration)
            {
                fsm.ChangeState(GoToIdleStateName);
                return;
            }
        }
        else
        {
            lowVelocityTimer = 0f;
        }

        if (securityNPC.IsPlayerInFOV())
        {
            fsm.ChangeState(GoToChaseStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        securityNPC.SetSteeringBehaviourWeight(securityNPC.followPathBehaviour, 0);
    }
}