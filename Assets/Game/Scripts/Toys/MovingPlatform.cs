using System;
using System.Collections.Generic;
using UnityEngine;

namespace FD.Toys
{
    [Serializable]
    public enum PlatformType
    {
        OneShot,
        PingPong,
        Loop
    }

    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private List<Transform> waypoints = new();
        [SerializeField] private float speed = 2f;
        [SerializeField] private PlatformType platformType = PlatformType.PingPong;
        [SerializeField] private float closeEnoughDistance = 0.1f;
        [SerializeField] private bool useRigidbody = true;

        public Vector3 CurrentWaypoint => waypoints[_currentWaypointIndex].position;
        
        private int _currentWaypointIndex;
        private bool _reverse;
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        
        private void Update()
        {
            // Move the platform towards the target position
            if (useRigidbody)
            {
                _rb.MovePosition(Vector3.MoveTowards(transform.position, CurrentWaypoint, speed * Time.deltaTime));
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, CurrentWaypoint, speed * Time.deltaTime);
            }

            // Check if the platform has reached the target position
            if (!(Vector3.Distance(transform.position, CurrentWaypoint) < closeEnoughDistance)) return;
            
            // Switch target position to the other waypoint
            SetNextWaypoint();
        }

        private void SetNextWaypoint()
        {
            if (platformType == PlatformType.OneShot && _currentWaypointIndex == waypoints.Count - 1) return;
            
            if (platformType == PlatformType.PingPong && (_currentWaypointIndex == 0 || _currentWaypointIndex == waypoints.Count - 1))
            {
                _reverse = !_reverse;
            }

            if (_reverse)
            {
                _currentWaypointIndex = Math.Max(_currentWaypointIndex - 1, 0);
            }
            else
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Count;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            
            Transform previousWaypoint = null;
            foreach (var waypoint in waypoints)
            {
                if (previousWaypoint)
                {
                    Gizmos.DrawLine(waypoint.position, previousWaypoint.position);
                }
                Gizmos.DrawSphere(waypoint.position, 0.2f);
                previousWaypoint = waypoint;
            }
        }
    }
}