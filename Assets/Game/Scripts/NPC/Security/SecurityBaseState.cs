using FD.AI.FSM;
using FD.AI.SteeringBehaviours;
using UnityEngine;

namespace FD.NPC.Security
{
    public abstract class SecurityNPCBaseState : FSMBaseState
    {
        protected Security SecurityNPC;
        protected SteeringAgent SteeringAgent;
        protected DetectPlayer PlayerDetection;

        protected float CachedSpeed { get; set; }

        public override void Init(GameObject owner, FSM fsm)
        {
            base.Init(owner, fsm);
            SecurityNPC = owner.GetComponent<Security>();
            SteeringAgent = SecurityNPC.steeringAgent;
            PlayerDetection = SecurityNPC.playerDetection;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SecurityNPC.SetSpeed(CachedSpeed);
        }
    }
}