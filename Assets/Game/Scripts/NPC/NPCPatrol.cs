using UnityEngine;

namespace Game.Scripts.Barking
{
    public class NPCPatrol : MonoBehaviour
    {
        public Transform[] waypoints;
        public float speed = 2f;
        public int waypointIndex = 0;
        public float stopDistance = 1f;
        private INPCReaction _reactionComponent;

        private void Awake()
        {
            _reactionComponent = GetComponent<INPCReaction>();
        }

        private void Update()
        {
            if (!_reactionComponent.IsReacting) 
            {
                MoveToWaypoint();
            }
        }

        private void MoveToWaypoint()
        {
            var step = speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, waypoints[waypointIndex].position) < stopDistance)
            {
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
            }
            transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].position, step);
        }
    }
}