using Game.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Game.Scripts.Elastic
{
    public class ElasticForce : MonoBehaviour
    {
        [Header("Anchor settings")]
        [SerializeField] private Transform player1;
        [SerializeField] private Transform player2;
        [SerializeField, Range(0, 1)] private float midpointAdjustment = 0.5f;
        [SerializeField] private float maxDistance = 10;
        
        [Header("Force Settings")]
        [SerializeField] private float baseForce = 1500;
        [SerializeField] private float maxForce = 100000;
        [SerializeField] private AnimationCurve forceCurve;
        [SerializeField, Range(0, 1)] private float forceApplied = 0.5f;
        [SerializeField] private AnimationCurve yAxisForceFactorDot;
        
        [Header("Snapback Settings")]
        [SerializeField, Range(0, 1)] private float snapbackThreshold = 0.9f;
        [SerializeField] private float snapbackForceMagnitude = 150;
        [SerializeField] private float snapbackDelay = 0.5f;
        
        [Header("Rumble Settings")]
        [SerializeField] private bool useRumble = true;
        [SerializeField, Range(0, 1)] private float rumbleAmount = 0.5f;
        [SerializeField] private AnimationCurve rumbleForceCurve;

        private Rigidbody _rb1;
        private Rigidbody _rb2;
        private PlayerController _controller1;
        private PlayerController _controller2;
        private float _snapbackTimer;

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
            _snapbackTimer -= Time.deltaTime;
            
            if (player1 is null || player2 is null || _controller1 is null || _controller2 is null) return;

            Vector3 midpoint = Vector3.Lerp(player1.position, player2.position, midpointAdjustment);

            float distance1 = (player1.position - midpoint).magnitude;
            float distance2 = (player2.position - midpoint).magnitude;

            float normalizedDistance1 = distance1 / (maxDistance * midpointAdjustment);
            float normalizedDistance2 = distance2 / (maxDistance * (1 - midpointAdjustment));
            
            // Use Force Applied factor to calculate forceCurve contribution
            const float forceAppliedAdjustmentFactor = 0.05f;
            var adjustedForceApplied = forceApplied - forceAppliedAdjustmentFactor;
            
            var forceAppliedDistance1 = (normalizedDistance1 - adjustedForceApplied) / (1f - adjustedForceApplied);
            var forceAppliedDistance2 = (normalizedDistance2 - adjustedForceApplied) / (1f - adjustedForceApplied);

            float forceMultiplier1 = forceCurve.Evaluate(forceAppliedDistance1);
            float forceMultiplier2 = forceCurve.Evaluate(forceAppliedDistance2);

            float forceMagnitude1 = Mathf.Min(forceMultiplier1 * baseForce, maxForce);
            float forceMagnitude2 = Mathf.Min(forceMultiplier2 * baseForce, maxForce);
            
            if (CalculateBindingForce(normalizedDistance1, player1, midpoint, forceMagnitude1, out var force1, forceAppliedDistance1))
            {
                _rb1.AddForce(force1);
            }

            if (CalculateBindingForce(normalizedDistance2, player2, midpoint, forceMagnitude2, out var force2, forceAppliedDistance2))
            {
                _rb2.AddForce(force2);
            }
            
            AdjustAccelerationFactor(_controller1, normalizedDistance1);
            AdjustAccelerationFactor(_controller2, normalizedDistance2);

            if (_snapbackTimer > 0) return;

            var snapbackPlayer1 = ApplySnapbackForce(player1, _rb1, _controller1, normalizedDistance1, midpoint);
            var snapbackPlayer2 = ApplySnapbackForce(player2, _rb2, _controller2, normalizedDistance2, midpoint);

            if (snapbackPlayer1 || snapbackPlayer2)
            {
                _snapbackTimer = snapbackDelay;
            }
        }

        private bool CalculateBindingForce(float normalizedDistance, Transform player, Vector3 midpoint, float forceMagnitude, out Vector3 force, float forceAppliedNormalizedDistance)
        {
            if (normalizedDistance <= forceApplied)
            {
                force = Vector3.zero;
                if (Gamepad.current is not null)
                {
                    Gamepad.current.ResetHaptics();
                }
                return false;
            }

            if (useRumble && Gamepad.current is not null)
            {
                var rumble = rumbleForceCurve.Evaluate(forceAppliedNormalizedDistance) * rumbleAmount;
                Gamepad.current.SetMotorSpeeds(rumble, rumble);
            }
            
            force = (midpoint - player.position).normalized * forceMagnitude;
            var yFactor = Mathf.Abs(Vector3.Dot(force.normalized, Vector3.up));
            force.y *= yAxisForceFactorDot.Evaluate(yFactor);
            
            return true;
        }

        private bool ApplySnapbackForce(Transform player, Rigidbody rb, PlayerController controller, float normalizedDistance, Vector3 midpoint)
        {
            if (normalizedDistance < snapbackThreshold) return false;
            
            // Debug.Log($"{player.name} is in the snapback range");
            if (controller.IsGrabbing || controller.Movement != Vector3.zero) return false;
            
            Vector3 snapbackForce = (midpoint - player.position).normalized * snapbackForceMagnitude;
            rb.AddForce(snapbackForce, ForceMode.Impulse);

            return true;
            // Debug.Log($"Snapback Force Applied to {player.name}: {snapbackForce}");
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
