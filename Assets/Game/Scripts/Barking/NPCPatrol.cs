using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public int waypointIndex = 0;
    public float stopDistance = 1f;
    private INPCReaction reactionComponent;

    private void Awake()
    {
        reactionComponent = GetComponent<INPCReaction>();
    }

    private void Update()
    {
        if (!reactionComponent.isReacting) 
        {
            MoveToWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        float step = speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, waypoints[waypointIndex].position) < stopDistance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].position, step);
    }
}