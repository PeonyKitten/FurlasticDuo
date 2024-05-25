using UnityEngine;

public class DynamicElasticForce : MonoBehaviour
{
    [SerializeField] private Transform anchor;       
    [SerializeField] private Transform player;
    [SerializeField] private float baseForce = 100f;
    [SerializeField] private float maxDistance = 5f; 
    [SerializeField] private float maxForce = 1000f; 
    [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private float forceApplied = 0.5f;

    private Rigidbody dog;

    private void Start()
    {
        dog = player.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (anchor == null || player == null) return;

        Vector3 direction = player.position - anchor.position;
        float distance = direction.magnitude;

        if (distance > maxDistance)
        {
            direction = direction.normalized.normalized * maxDistance;
            player.position = anchor.position + direction;
            distance = maxDistance; 
        }


        float normalizedDistance = distance / maxDistance;
        float forceMultiplier = forceCurve.Evaluate(normalizedDistance);
        float forceMagnitude = Mathf.Min(Mathf.Pow(forceMultiplier, 2) * baseForce, maxForce);

        if (normalizedDistance > forceApplied)
        {
            Vector3 force = -direction.normalized * forceMagnitude;
            dog.AddForce(force);
            Debug.Log($"Distance: {distance}, Normalized Distance: {normalizedDistance}, Force Magnitude: {forceMagnitude}, Force Applied: {force}");
        }
    }
}
