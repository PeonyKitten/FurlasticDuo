using System;
using System.Collections;
using FD.AI.SteeringBehaviours;
using FD.Barking;
using FD.Utils;
using UnityEngine;

namespace FD.NPC
{
    public class BarkFleeSB : FleeSteeringBehaviour, IBarkReaction
    {
        [Serializable]
        public enum FleeStrategy
        {
            FleeByDistance,
            FleeForDuration,
        }

        [Header("Bark Flee Settings")]
        [SerializeField] private FleeStrategy fleeStrategy = FleeStrategy.FleeForDuration;
        [SerializeField] private float fleeTime = 3f;
        [Tooltip("When it is the only active SteeringBehaviour, should we keep running after we stop reacting?")]
        [SerializeField] private bool shouldRunAwayForever = true;

        public bool IsReacting { get; set; }

        private Coroutine _barkCoroutine;

        public void React(Bark bark)
        {
            IsReacting = true;
            Target = bark.transform.position;
            steeringAgent.reachedGoal = false;

            if (fleeStrategy == FleeStrategy.FleeByDistance) return;

            if (_barkCoroutine != null)
            {
                StopCoroutine(_barkCoroutine);
            }

            _barkCoroutine = StartCoroutine(StopReactingAfterTime(fleeTime));
        }

        public override Vector3 CalculateForce()
        {
            if (!IsReacting)
            {
                return Vector3.zero;
            }

            if (fleeStrategy == FleeStrategy.FleeByDistance)
            {
                // If we've gotten out of range, stop reacting
                var fleeForce = base.CalculateForce();
                if (fleeForce == Vector3.zero)
                {
                    StopReacting();
                }

                return fleeForce;
            }

            CalculateSeekForce();

            DesiredVelocity = -DesiredVelocity;

            return DesiredVelocity;
        }

        private void StopReacting()
        {
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
                DebugExtension.DrawCircle(transform.position, Vector3.up, Color.red);
            }
        }
    }
}
