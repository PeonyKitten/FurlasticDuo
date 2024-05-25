using Game.Scripts;
using UnityEngine;

public class DynamicElasticForce : MonoBehaviour
{
    [SerializeField] private Transform anchor;       
    [SerializeField] private Transform player;
    [SerializeField] private float baseForce;
    [SerializeField] private float maxDistance; 
    [SerializeField] private float maxForce; 
    [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private float forceApplied;
    [SerializeField] private float snapbackThreshold;
    [SerializeField] private float snapbackForceMultiplier = 10f;

    private Rigidbody dog;
    private PlayerController playerController;

    private void Start()
    {
        dog = player.GetComponent<Rigidbody>();
        playerController = player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (anchor is null || player is null) return;

        Vector3 direction = player.position - anchor.position;
        float distance = direction.magnitude;

        if (distance > maxDistance)
        {
            direction = direction.normalized * maxDistance;
            player.position = anchor.position + direction;
            distance = maxDistance; 
        }


        float normalizedDistance = distance / maxDistance;
        float forceMultiplier = forceCurve.Evaluate(normalizedDistance);
        float forceMagnitude = Mathf.Min(forceMultiplier * baseForce, maxForce);

        if (normalizedDistance > forceApplied)
        {
            Vector3 force = -direction.normalized * forceMagnitude;
            dog.AddForce(force);
            //Debug.Log($"Distance: {distance}, Normalized Distance: {normalizedDistance}, Force Magnitude: {forceMagnitude}, Force Applied: {force}, ForceMultiplier: {forceMultiplier}");
        }

        if (normalizedDistance >= snapbackThreshold && normalizedDistance <= 1f)
        {
            Debug.Log("Player is in the snapback range");
            if (playerController.Movement == Vector3.zero)
            {
                Vector3 snapbackForce = -direction.normalized * snapbackForceMultiplier * forceMagnitude;
                dog.AddForce(snapbackForce, ForceMode.Impulse);
                Debug.Log($"Snapback Force Applied: {snapbackForce}");
            }
        }
    }
}
