using Game.Scripts.Misc;
using Game.Scripts.NPC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public FOV fov;
    private Dictionary<int, Transform> playersInRange = new Dictionary<int, Transform>();
    private SphereCollider detectionCollider;

    private void Awake()
    {
        detectionCollider = GetComponent<SphereCollider>();
        if (detectionCollider == null)
        {
            Debug.LogError("No SphereCollider found on PlayerDetection object!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int id = other.gameObject.GetInstanceID();
            playersInRange[id] = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int id = other.gameObject.GetInstanceID();
            playersInRange.Remove(id);
        }
    }

    public bool IsPlayerInFOV(out Transform detectedPlayer)
    {
        detectedPlayer = null;
        foreach (var player in playersInRange.Values)
        {
            if (fov.IsContained(player.position))
            {
                detectedPlayer = player;
                return true;
            }
        }
        return false;
    }

    public bool IsAnyPlayerInRange()
    {
        return playersInRange.Count > 0;
    }

    public Transform GetClosestPlayer()
    {
        return playersInRange.Values
            .OrderBy(p => Vector3.Distance(transform.position, p.position))
            .FirstOrDefault();
    }
}