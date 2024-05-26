using Game.Scripts;
using UnityEngine;

public class DynamicElasticForce : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private float baseForce;
    [SerializeField] private float maxDistance; 
    [SerializeField] private float maxForce; 
    [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private float forceApplied;
    [SerializeField] private float snapbackThreshold;
    [SerializeField] private float snapbackForceMultiplier = 10f;
    [Range(0, 1)][SerializeField] private float midpointAdjustment = 0.5f;

    private Rigidbody rb1;
    private Rigidbody rb2;
    private PlayerController controller1;
    private PlayerController controller2;
    private LineRenderer lineRenderer;

    private void Start()
    {
        rb1 = player1.GetComponent<Rigidbody>();
        rb2 = player2.GetComponent<Rigidbody>();
        controller1 = player1.GetComponent<PlayerController>();
        controller2 = player2.GetComponent<PlayerController>();


        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 3; 
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
    }

    private void FixedUpdate()
    {
        if (player1 is null || player2 is null || controller1 is null || controller2 is null) return;

        Vector3 direction = player2.position - player1.position;
        float totalDistance = direction.magnitude;

        if (totalDistance > maxDistance)
        {
            direction = direction.normalized * maxDistance;
            player2.position = player1.position + direction;
        }

        Vector3 midpoint = Vector3.Lerp(player1.position, player2.position, midpointAdjustment);

        float distance1 = (player1.position - midpoint).magnitude;
        float distance2 = (player2.position - midpoint).magnitude;

        float normalizedDistance1 = distance1 / (maxDistance * midpointAdjustment);
        float normalizedDistance2 = distance2 / (maxDistance * (1 - midpointAdjustment));

        float forceMultiplier1 = forceCurve.Evaluate(normalizedDistance1);
        float forceMultiplier2 = forceCurve.Evaluate(normalizedDistance2);

        float forceMagnitude1 = Mathf.Min(forceMultiplier1 * baseForce, maxForce);
        float forceMagnitude2 = Mathf.Min(forceMultiplier2 * baseForce, maxForce);


        if (normalizedDistance1 > forceApplied)
        {
            Vector3 force1 = (midpoint - player1.position).normalized * forceMagnitude1;
            rb1.AddForce(force1);
        }

        if (normalizedDistance2 > forceApplied)
        {
            Vector3 force2 = (midpoint - player2.position).normalized * forceMagnitude2;
            rb2.AddForce(force2);
        }

        lineRenderer.SetPosition(0, player1.position);
        lineRenderer.SetPosition(1, midpoint);
        lineRenderer.SetPosition(2, player2.position);

        // Debug log for verification
        Debug.Log($"Player1 - Distance: {distance1}, Normalized Distance: {normalizedDistance1}, Force Magnitude: {forceMagnitude1}");
        Debug.Log($"Player2 - Distance: {distance2}, Normalized Distance: {normalizedDistance2}, Force Magnitude: {forceMagnitude2}");

        ApplySnapbackForce(player1, rb1, controller1, normalizedDistance1, midpoint);
        ApplySnapbackForce(player2, rb2, controller2, normalizedDistance2, midpoint);
    }

    private void ApplySnapbackForce(Transform player, Rigidbody rb, PlayerController controller, float normalizedDistance, Vector3 midpoint)
    {
        if (normalizedDistance >= snapbackThreshold && normalizedDistance <= 1f)
        {
            Debug.Log($"{player.name} is in the snapback range");
            if (controller.Movement == Vector3.zero)
            {
                Vector3 snapbackForce = (midpoint - player.position).normalized * snapbackForceMultiplier * maxForce;
                rb.AddForce(snapbackForce, ForceMode.Impulse);
                Debug.Log($"Snapback Force Applied to {player.name}: {snapbackForce}");
            }
        }
    }
}
