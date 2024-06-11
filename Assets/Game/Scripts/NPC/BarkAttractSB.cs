using UnityEngine;
using Game.Scripts.Barking;
using Game.Scripts.SteeringBehaviours;
using System.Collections;
using System;

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
        [SerializeField] private float attractSpeedMultiplier = 0.5f;

        public bool IsReacting { get; set; }
        private Coroutine _barkCoroutine;

        public void React(Bark bark)
        {
            IsReacting = true;
            Target = bark.transform.position;
            steeringAgent.reachedGoal = false;
            steeringAgent.maxSpeed *= attractSpeedMultiplier;

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

            if (attractStrategy == AttractStrategy.AttractByDistance)
            {
                var attractForce = CalculateArriveForce();
                if (attractForce == Vector3.zero)
                {
                    Debug.Log("Attract force = 0");
                    StopReacting();
                }
                return attractForce;
            }

            CalculateArriveForce();

            return DesiredVelocity;
        }

        private void StopReacting()
        {
            IsReacting = false;
            steeringAgent.maxSpeed /= attractSpeedMultiplier;
            //steeringAgent.reachedGoal = true;
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