using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : MonoBehaviour
{
    public RuntimeAnimatorController FSMController;

    public Animator fsmAnimator { get; private set; }

    private void Awake()
    {
        GameObject FSMGo = new GameObject("FSM", typeof(Animator));
        FSMGo.transform.parent = transform;

        fsmAnimator = FSMGo.GetComponent<Animator>();
        fsmAnimator.runtimeAnimatorController = FSMController;

        FSMGo.hideFlags = HideFlags.HideInInspector;

        FSMBaseState[] behaviours = fsmAnimator.GetBehaviours<FSMBaseState>();
        foreach (FSMBaseState state in behaviours)
        {
            state.Init(gameObject, this);
        }
    }

    public bool ChangeState(string stateName)
    {
        return ChangeState(Animator.StringToHash(stateName));
    }

    public bool ChangeState(int hashStateName)
    {
        bool hasState = fsmAnimator.HasState(0, hashStateName);

        if (!hasState)
        {
            Debug.LogError($"State {hashStateName} not found in Animator!");
            return false;
        }

        fsmAnimator.CrossFade(hashStateName, 0.0f, 0);
        return hasState;
    }

}
