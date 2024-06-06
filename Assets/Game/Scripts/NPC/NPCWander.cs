using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Barking
{
    public class NPCWander : MonoBehaviour
    {
        public float wanderRadius;
        public float wanderTimer;

        private NavMeshAgent _agent;
        private float _timer;
        private IBarkReaction _reactionComponent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _timer = wanderTimer;
            _reactionComponent = GetComponent<IBarkReaction>();
        }

        //private void Update()
        //{
        //    if (_reactionComponent.IsReacting) return;
            
        //    _timer += Time.deltaTime;

        //    if (_timer >= wanderTimer)
        //    {
        //        var newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        //        _agent.SetDestination(newPos);
        //        _timer = 0;
        //    }
        //}

        //private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        //{
        //    var randDirection = Random.insideUnitSphere * dist;
        //    randDirection += origin;

        //    NavMesh.SamplePosition(randDirection, out var navHit, dist, layermask);

        //    return navHit.position;
        //}
    }
}
