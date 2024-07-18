using System.Linq;
using UnityEngine;

namespace FD.AI.FSM
{
    public abstract class FSM : MonoBehaviour
    {
        public RuntimeAnimatorController FSMController;
        public Animator fsmAnimator { get; private set; }
        // public Animator visualAnimator { get; private set; }

        private void Awake()
        {
            GameObject FSMGo = new GameObject("FSM", typeof(Animator));
            FSMGo.transform.parent = transform;

            fsmAnimator = FSMGo.GetComponent<Animator>();
            fsmAnimator.runtimeAnimatorController = FSMController;

            //  FSMGo.hideFlags = HideFlags.HideInInspector;

            //visualAnimator = GetComponent<Animator>();
            //if (visualAnimator == null)
            //{
            //    Debug.LogWarning("No Animator component found for visual animations on " + gameObject.name);
            //}

            FSMBaseState[] behaviours = fsmAnimator.GetBehaviours<FSMBaseState>();
            foreach (FSMBaseState state in behaviours)
            {
                state.Init(gameObject, this);
            }
        }

        public bool ChangeState(string stateName)
        {
            Debug.Log($"State name: {stateName}");
            return ChangeState(Animator.StringToHash(stateName));
        }

        private bool ChangeState(int hashStateName, string stateName = null)
        {
            bool hasState = fsmAnimator.HasState(0, hashStateName);
            if (!hasState)
            {
                if (string.IsNullOrEmpty(stateName))
                {
                    stateName = FSMController.animationClips
                        .FirstOrDefault(clip => Animator.StringToHash(clip.name) == hashStateName)?.name ?? "Unknown";
                }
                Debug.LogError($"State '{stateName}' (hash: {hashStateName}) not found in Animator!");
                return false;
            }

            fsmAnimator.CrossFade(hashStateName, 0.0f, 0);
            return true;
        }

    }
}