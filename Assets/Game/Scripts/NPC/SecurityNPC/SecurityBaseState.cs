using UnityEngine;

public abstract class SecurityNPCBaseState : FSMBaseState
{
    protected SecurityNPC securityNPC;

    public override void Init(GameObject owner, FSM fsm)
    {
        base.Init(owner, fsm);
        securityNPC = owner.GetComponent<SecurityNPC>();
        Debug.Assert(securityNPC != null, $"{owner.name} must have a SecurityNPC Component");
    }

    public virtual void OnIdleComplete() { }
}