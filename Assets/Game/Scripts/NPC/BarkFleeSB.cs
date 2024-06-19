using UnityEngine;
using Game.Scripts.Barking;
using Game.Scripts.SteeringBehaviours;
using UnityEngine.AI;
using System.Collections;
using Game.Scripts.Utils;

namespace Game.Scripts.NPC
{
    public class BarkFleeSB : FleeSteeringBehaviour, IBarkReaction
    {
        [SerializeField] private bool fleeBasedOnDistance = true;
        [SerializeField] private float fleeTime = 3f;

        public bool IsReacting { get; set; }

        private Coroutine _barkCoroutine;

        public void React(Bark bark)
        {
            IsReacting = true;
            Target = bark.transform.position;
            steeringAgent.reachedGoal = false;

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

            if (fleeBasedOnDistance)
            {
                return base.CalculateForce();
            }

            CalculateSeekForce();

            DesiredVelocity = -DesiredVelocity;

            return DesiredVelocity;
        }

        private void StopReacting()
        {
            IsReacting = false;
            steeringAgent.reachedGoal = true;
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
