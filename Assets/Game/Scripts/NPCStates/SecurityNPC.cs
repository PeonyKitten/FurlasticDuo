using UnityEngine;
using Game.Scripts.SteeringBehaviours;
using Game.Scripts.NPC;
using Game.Scripts.Misc;

public class SecurityNPC : MonoBehaviour
{
    public SteeringAgent steeringAgent;
    public FollowPathSteeringBehaviour followPathBehaviour;
    public BarkAttractSB barkAttractBehaviour;
    public ChasePlayerSteeringBehaviour chasePlayerBehaviour;
    public FOV fov;

    public float chaseRadius = 10f;

    public void SetSteeringBehaviourWeight(SteeringBehaviour behaviour, float weight)
    {
        behaviour.Weight = weight;
    }

    public bool IsPlayerInFOV()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, fov.radius, LayerMask.GetMask("Player"));
        foreach (Collider col in colliders)
        {
            if (fov.IsContained(col.transform.position))
            {
                chasePlayerBehaviour.SetPlayer(col.transform);
                return true;
            }
        }
        chasePlayerBehaviour.ResetPlayer();
        return false;
    }

    public bool IsPlayerInChaseRange()
    {
        return chasePlayerBehaviour._player != null &&
               Vector3.Distance(transform.position, chasePlayerBehaviour._player.position) <= chaseRadius;
    }
}