using Game.Scripts;
using UnityEngine;


namespace Game.Scripts.Elastic
{
    public class ElasticForce : MonoBehaviour
    {
        [SerializeField] private Transform player1;
        [SerializeField] private Transform player2;
        [SerializeField] private float baseForce = 1500;
        [SerializeField] private float maxDistance = 10;
        [SerializeField] private float maxForce = 100000;
        [SerializeField] private AnimationCurve forceCurve;
        [SerializeField, Range(0, 1)] private float forceApplied = 0.5f;
        [SerializeField, Range(0, 1)] private float snapbackThreshold = 0.9f;
        [SerializeField] private float snapbackForceMagnitude = 150;
        [SerializeField, Range(0, 1)] private float midpointAdjustment = 0.5f;
        [SerializeField] private Vector3 forceMultiplier = new(1, 0.1f, 1);

        private Rigidbody _rb1;
        private Rigidbody _rb2;
        private PlayerController _controller1;
        private PlayerController _controller2;

        public Transform Player1 => player1;
        public Transform Player2 => player2;
        public float ForceAppliedDistance => forceApplied * maxDistance;
        public float SnapbackDistance => snapbackThreshold * maxDistance;
        public float MaxDistance => maxDistance;


        private void Start()
        {
            _rb1 = player1.GetComponent<Rigidbody>();
            _rb2 = player2.GetComponent<Rigidbody>();
            _controller1 = player1.GetComponent<PlayerController>();
            _controller2 = player2.GetComponent<PlayerController>();
        }

        private void FixedUpdate()
        {
            if (player1 is null || player2 is null || _controller1 is null || _controller2 is null) return;

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
                Vector3 force1 = Vector3.Scale((midpoint - player1.position).normalized * forceMagnitude1, forceMultiplier);
                _rb1.AddForce(force1);
            }

            if (normalizedDistance2 > forceApplied)
            {
                Vector3 force2 = Vector3.Scale((midpoint - player2.position).normalized * forceMagnitude2, forceMultiplier);
                _rb2.AddForce(force2);
            }
            
            AdjustAccelerationFactor(_controller1, normalizedDistance1);
            AdjustAccelerationFactor(_controller2, normalizedDistance2);

            ApplySnapbackForce(player1, _rb1, _controller1, normalizedDistance1, midpoint);
            ApplySnapbackForce(player2, _rb2, _controller2, normalizedDistance2, midpoint);
        }

        private void ApplySnapbackForce(Transform player, Rigidbody rb, PlayerController controller, float normalizedDistance, Vector3 midpoint)
        {
            if (normalizedDistance >= snapbackThreshold)
            {
                // Debug.Log($"{player.name} is in the snapback range");
                if (controller.Movement == Vector3.zero)
                {
                    Vector3 snapbackForce = (midpoint - player.position).normalized * snapbackForceMagnitude;
                    rb.AddForce(snapbackForce, ForceMode.Impulse);
                    // Debug.Log($"Snapback Force Applied to {player.name}: {snapbackForce}");
                }
            }
        }
        private void AdjustAccelerationFactor(PlayerController controller, float normalizedDistance)
        {
            if (normalizedDistance > 0.99)
            {
                controller.accelerationFactor = 0.5f; 
            }
            else
            {
                controller.accelerationFactor = 1f; 
            }
        }
    }
}
