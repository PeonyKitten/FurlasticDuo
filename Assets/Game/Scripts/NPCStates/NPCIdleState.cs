using UnityEngine;
using UnityEngine.InputSystem.XR;

public class SecurityNPCIdleState : SecurityNPCBaseState
{
    public string GoToPatrolStateName = "Patrolling"; 
    public string GoToChaseStateName = "Chasing";
    public float idleDuration = 1f;
    private float currentTimer = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Security NPC is not doing its job.");
        currentTimer = idleDuration;
        securityNPC.SetSteeringBehaviourWeight(securityNPC.followPathBehaviour, 0);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentTimer -= Time.deltaTime;
        float rotationAngle = Mathf.Sin(Time.time * 2f) * 45f;
        securityNPC.transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);

        if (currentTimer <= 0)
        {
            fsm.ChangeState(GoToPatrolStateName);
        }
        else if (securityNPC.IsPlayerInFOV())
        {
            fsm.ChangeState(GoToChaseStateName);
        }
    }
}