using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttract : MonoBehaviour, INPCReaction
{
    public float attractSpeedMultiplier = 5f;
    public bool isReacting { get; set; }

    public void ReactToBark(Vector3 barkOrigin)
    {
        isReacting = true;
        Vector3 attractDirection = (barkOrigin - transform.position).normalized * attractSpeedMultiplier;
        GetComponent<Rigidbody>().AddForce(attractDirection, ForceMode.VelocityChange);
        Invoke("StopReacting", 3f); 
    }

    private void StopReacting()
    {
        isReacting = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}