using System;
using System.Collections;
using FD.AI.SteeringBehaviours;
using FD.Barking;
using FD.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace FD.NPC
{
    public class BarkAttractSB : ArriveSteeringBehaviour, IBarkReaction
    {
        [Serializable]
        public enum AttractStrategy
        {
            AttractByDistance,
            AttractForDuration,
        }

        [Header("Bark Attract Settings")]
        [SerializeField] private AttractStrategy attractStrategy = AttractStrategy.AttractForDuration;
        [SerializeField] private float attractTime = 3f;
        [Tooltip("When it is the only active SteeringBehaviour, should we keep running after we stop reacting?")]
        [SerializeField] private bool shouldRunAwayForever = true;

        [Header("Callbacks")]
        public UnityEvent onBarkReact;
        public UnityEvent onBarkStopReact;
        public UnityEvent<Vector3> onBarkReaction;

        public bool IsReacting { get; set; }
        private Coroutine _barkCoroutine;

        public void React(Bark bark)
        {
            IsReacting = true;
            Target = bark.transform.position.Flatten().Bulk(transform.position.y);
            steeringAgent.reachedGoal = false;
            onBarkReact?.Invoke();
            onBarkReaction?.Invoke(Target);

            if (attractStrategy == AttractStrategy.AttractByDistance) return;
            if (_barkCoroutine != null)
            {
                StopCoroutine(_barkCoroutine);
            }
            _barkCoroutine = StartCoroutine(StopReactingAfterTime(attractTime));
        }

        public void ReactToBarkOrigin(Vector3 barkOrigin)
        {
            Target = barkOrigin;
            IsReacting = true;
        }

        public override Vector3 CalculateForce()
        {
            if (!IsReacting)
            {
                return Vector3.zero;
            }

            var attractForce = CalculateArriveForce();
            
            if (attractStrategy == AttractStrategy.AttractByDistance && steeringAgent.reachedGoal) {
                StopReacting();
            }

            return attractForce;
        }

        private void StopReacting()
        {
            IsReacting = false;
            if (steeringAgent.SteeringBehaviourCount == 1 && !shouldRunAwayForever)
            {
                steeringAgent.reachedGoal = true;
            }

            onBarkStopReact?.Invoke();
        }

        private IEnumerator StopReactingAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            StopReacting();
        }

        protected override void OnDrawGizmos()
        {
            if (!IsReacting) return;
            
            base.OnDrawGizmos();
            DebugExtension.DrawCircle(transform.position, Vector3.up, Color.blue);
            Gizmos.DrawLine(transform.position, Target);
        }

        public override void Reset()
        {
            if (_barkCoroutine != null)
            {
                StopCoroutine(_barkCoroutine);
            }
            
            StopReacting();
        }
    }
}