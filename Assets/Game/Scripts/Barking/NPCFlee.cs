using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCFlee : MonoBehaviour, INPCReaction
{
    public float fleeSpeedMultiplier = 2f;
    public bool isReacting { get; set; }

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void ReactToBark(Vector3 barkOrigin)
    {
        isReacting = true;
        agent.speed *= fleeSpeedMultiplier;

        Vector3 fleeDirection = (transform.position - barkOrigin).normalized;
        Vector3 fleePosition = transform.position + fleeDirection * fleeSpeedMultiplier;
        agent.SetDestination(fleePosition);

        Invoke("StopReacting", 3f); // Assuming reaction lasts 3 seconds
    }

    private void StopReacting()
    {
        isReacting = false;
        agent.speed /= fleeSpeedMultiplier;
    }
}
