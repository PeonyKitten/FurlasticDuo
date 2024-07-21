using FD.AI.FSM;
using UnityEngine;

namespace FD.NPC.Security
{
    public abstract class SecurityNPCBaseState : FSMBaseState
    {
        protected Security Security;

        public override void Init(GameObject owner, FSM fsm)
        {
            base.Init(owner, fsm);
            Security = owner.GetComponent<Security>();
            Debug.Assert(Security != null, $"{owner.name} must have a SecurityNPC Component");
        }
    }
}