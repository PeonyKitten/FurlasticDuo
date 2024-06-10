﻿using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public class FollowPathSteeringBehaviour: ArriveSteeringBehaviour
    {
        [Header("Follow Path Behaviour")]
        [SerializeField] protected float ehCloseEnoughDistance = 0.5f;
        [SerializeField] private bool generateWaypointsFromChildren = true;
        [SerializeField] private Transform waypointsParent; 
        [SerializeField] protected List<Transform> waypoints = new();
        
        protected Queue<Vector3> QueuedPoints = new();

        protected virtual void Start()
        {
            if (!generateWaypointsFromChildren) return;
            
            // If no waypointsParent was provided, use the current Transform
            if (waypointsParent == null)
            {
                waypointsParent = transform;
            }
            
            waypoints.Clear();
            for (var i = 0; i < waypointsParent.childCount; i++)
            {
                waypoints.Add(waypointsParent.GetChild(i));
            }
        }

        public override Vector3 CalculateForce()
        {
            if (!QueuedPoints.TryPeek(out var currentTarget))
            {
                RefreshQueue();
                return Vector3.zero;
            }
            
            if (Vector3.Distance(currentTarget, transform.position) < ehCloseEnoughDistance)
            {
                QueuedPoints.Dequeue();

                if (!QueuedPoints.TryPeek(out currentTarget))
                {
                    RefreshQueue();
                    return Vector3.zero;
                }
            }

            Target = currentTarget;

            return CalculateArriveForce();
        }

        protected virtual void RefreshQueue()
        {
            foreach (var waypoint in waypoints)
            {
                QueuedPoints.Enqueue(waypoint.position);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            var startPoint = transform.position;

            if (QueuedPoints == null)
            {
                return;
            }
            
            foreach (var point in QueuedPoints)
            {
                Debug.DrawLine(startPoint, point, Color.black);
                startPoint = point;
            }
        }
    }
}