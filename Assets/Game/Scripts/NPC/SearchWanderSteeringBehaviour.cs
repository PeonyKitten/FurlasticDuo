using Game.Scripts.SteeringBehaviours;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.NPC
{
    public class SearchWanderSteeringBehaviour: WanderSteeringBehaviour
    {
        [SerializeField] private float searchDuration = 10f;

        [Header("Callbacks")]
        public UnityEvent onStartSearch;
        public UnityEvent onStopSearch;

        private float _searchTimer;
        
        public void Search()
        {
            base.Start();
            _searchTimer = searchDuration;
            Target = transform.position;
            onStartSearch.Invoke();
        }
        
        public override Vector3 CalculateForce()
        {
            var isSearching = _searchTimer > 0;
            _searchTimer -= Time.deltaTime;

            if (_searchTimer > 0) return base.CalculateForce();
            
            if (isSearching)
            {
                onStopSearch?.Invoke();
            }
            return Vector3.zero;
        }

        protected override void OnDrawGizmos()
        {
            if (_searchTimer <= 0) return;
            
            base.OnDrawGizmos();
        }
    }
}