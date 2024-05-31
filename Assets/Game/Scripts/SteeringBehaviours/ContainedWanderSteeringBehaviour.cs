using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public class ContainedWanderSteeringBehaviour : WanderSteeringBehaviour
    {
        [SerializeField] private Jail containedArea;
        [SerializeField] private float ehCloseEnoughDistance = 0.5f;
        [SerializeField] private bool seekUntilReachedTarget = true;
        private bool _hasTarget;

        protected override void Start()
        {
            base.Start();

            // Sometimes our own minds are what imprison us
            if (containedArea == null)
            {
                containedArea = steeringAgent.transform.parent.gameObject.AddComponent<Jail>();
            }

            allRoadsLeadToRome = containedArea.transform;
        }

        public override Vector3 CalculateForce()
        {
            if (containedArea.IsContained(transform.position) && !_hasTarget)
            {
                return base.CalculateForce();
            }
            
            if (!_hasTarget)
            {
                Target = containedArea.transform.position + UtilsMath.RandomInsideCircle(containedArea.radius);
            }

            if (seekUntilReachedTarget)
            {
                _hasTarget = true;
            }
            
            if (_hasTarget && Vector3.Distance(transform.position, Target) < ehCloseEnoughDistance)
            {
                _hasTarget = false;
            }
            
            return CalculateSeekForce();
        }
    }
}
