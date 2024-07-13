using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMBaseState : StateMachineBehaviour
{
    protected FSM fsm { get; private set; }
    public GameObject owner { get; private set; }

    public virtual void Init(GameObject _owner, FSM _fsm)
    {
        owner = _owner;
        fsm = _fsm;
    }
}
