using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.SteeringBehaviours
{
    public class NavMeshFollowPathSteeringBehaviour : FollowPathSteeringBehaviour
    {
        private int _waypointIndex;
        private NavMeshPath _path;

        protected void Awake()
        {
            QueuedPoints = new();
            _path = new NavMeshPath();
        }

        protected override void RefreshQueue()
        {
            // Get the next waypoint index to use
            _waypointIndex = (_waypointIndex + 1) % waypoints.Count;

            NavMesh.CalculatePath(transform.position, waypoints[_waypointIndex].position, NavMesh.AllAreas, _path);

            QueuedPoints = new Queue<Vector3>(_path.corners);
        }
    }
}