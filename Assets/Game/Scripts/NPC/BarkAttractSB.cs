using UnityEngine;
using Game.Scripts.Barking;
using Game.Scripts.SteeringBehaviours;
using System.Collections;
using System;
using Game.Scripts.Utils;

namespace Game.Scripts.NPC
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

        public bool IsReacting { get; set; }
        private Coroutine _barkCoroutine;

        public void React(Bark bark)
        {
            IsReacting = true;
            Target = bark.transform.position;
            steeringAgent.reachedGoal = false;

            if (attractStrategy == AttractStrategy.AttractByDistance) return;

            if (_barkCoroutine != null)
            {
                StopCoroutine(_barkCoroutine);
            }

            _barkCoroutine = StartCoroutine(StopReactingAfterTime(attractTime));
        }

        public override Vector3 CalculateForce()
        {
            if (!IsReacting)
            {
                return Vector3.zero;
            }

            var attractForce = CalculateArriveForce();
            
            if (attractStrategy == AttractStrategy.AttractByDistance && attractForce == Vector3.zero) {
                StopReacting();
            }

            return attractForce;
        }

        private void StopReacting()
        {
            IsReacting = false;
            IsReacting = false;
            if (steeringAgent.SteeringBehaviourCount == 1 && !shouldRunAwayForever)
            {
                steeringAgent.reachedGoal = true;
            }
        }

        private IEnumerator StopReactingAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            StopReacting();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (IsReacting)
            {
                DebugExtension.DrawCircle(transform.position, Vector3.up, Color.blue);
            }
        }
    }
}