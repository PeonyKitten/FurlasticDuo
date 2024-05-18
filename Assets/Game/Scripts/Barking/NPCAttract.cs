using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAttract : MonoBehaviour, INPCReaction
{
    public float attractSpeedMultiplier = 0.5f;
    public bool isReacting { get; set; }

    private NavMeshAgent agent;
    private Vector3 barkOrigin;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void ReactToBark(Vector3 barkOrigin)
    {
        isReacting = true;
        this.barkOrigin = barkOrigin;
        agent.speed *= attractSpeedMultiplier;
        agent.SetDestination(barkOrigin);
    }

    private void Update()
    {
        if (isReacting && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            StopReacting();
        }
    }

    private void StopReacting()
    {
        isReacting = false;
        agent.speed /= attractSpeedMultiplier;
    }
}
