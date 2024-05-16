using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFlee : MonoBehaviour, INPCReaction
{
    public float fleeSpeedMultiplier = 5f;
    public bool isReacting { get; set; }
    public void ReactToBark(Vector3 barkOrigin)
    {
        isReacting = true;
        Vector3 fleeDirection = (transform.position - barkOrigin).normalized * fleeSpeedMultiplier;
        GetComponent<Rigidbody>().AddForce(fleeDirection, ForceMode.VelocityChange);
        Invoke("StopReacting", 3f); // Assuming reaction lasts 3 seconds
    }

    private void StopReacting()
    {
        isReacting = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero; // Stop the NPC from moving
    }
}
