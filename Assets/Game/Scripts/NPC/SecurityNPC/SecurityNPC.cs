using UnityEngine;
using Game.Scripts.SteeringBehaviours;
using Game.Scripts.NPC;
using Game.Scripts.Misc;
using Game.Scripts.Barking;

public class SecurityNPC : FSM
{
    [SerializeField] private Animator visualAnimator;
    public SteeringAgent steeringAgent;
    public FollowPathSteeringBehaviour followPathBehaviour;
    public BarkAttractSB barkAttractBehaviour;
    public ChasePlayerSteeringBehaviour chasePlayerBehaviour;
    public DetectPlayer playerDetection;
    public bool IsReacting { get; set; }
    public Vector3 barkOrigin { get; private set; }
    private static readonly int IdleTrigger = Animator.StringToHash("IdleTrigger");
    private static readonly int AlertTrigger = Animator.StringToHash("AlertTrigger");

    public void ReactToBark(Vector3 barkOrigin)
    {
        this.barkOrigin = barkOrigin;
        IsReacting = true;
        ChangeState("Investigate");
    }

    public void PlayAnimation(string animationTrigger)
    {
        if (visualAnimator != null)
        {
            switch (animationTrigger)
            {
                case "Idle":
                    visualAnimator.SetTrigger(IdleTrigger);
                    break;
                case "Alert":
                    visualAnimator.SetTrigger(AlertTrigger);
                    break;
                default:
                    Debug.Log($"Animation: {animationTrigger}");
                    break;
            }
        }
    }
}