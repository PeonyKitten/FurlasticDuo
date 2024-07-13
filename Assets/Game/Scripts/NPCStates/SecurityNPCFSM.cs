using UnityEngine;

[RequireComponent(typeof(SecurityNPC))]
public class SecurityNPCFSM : FSM
{
    public Vector3 barkOrigin { get; private set; }
    public void ReactToBark(Vector3 origin)
    {
        barkOrigin = origin;
        ChangeState("Alert");
    }
}