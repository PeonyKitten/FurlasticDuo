using Game.Scripts.SteeringBehaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementOff : MonoBehaviour
{
    private SteeringAgent steeringAgent;

    private void Awake()
    {
        steeringAgent = GetComponent<SteeringAgent>();
    }

    public void DisableMovement()
    {
        if (steeringAgent != null)
        {
            steeringAgent.enabled = false;
        }
    }   

    public void EnableMovement()
    {
        if (steeringAgent != null)
        {
            steeringAgent.enabled = true;
        }
    }
}