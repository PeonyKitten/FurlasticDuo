// ElasticForce.cs
// Dhruv Saikia, Alvin Philips
// 2024-06-30
// Controls the elastic force between the players, along with snapping back. 

using Game.Scripts;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Game.Scripts.Elastic
{
    public class ElasticForce : MonoBehaviour
    {

        [System.Serializable]
        public enum SnapbackMode
        {
            Duration,
            Distance
        }


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
        [SerializeField] private SnapbackMode snapbackMode = SnapbackMode.Duration;
        [SerializeField, Range(0, 1)] private float snapbackThreshold = 0.9f;
        [SerializeField] private float snapbackForceMagnitude = 150;
        [SerializeField] private float snapbackDelay = 0.5f;
        [SerializeField] private float snapbackDuration = 0.5f;

        [Header("Rumble Settings")]
        [SerializeField] private bool useRumble = true;
        [SerializeField, Range(0, 1)] private float rumbleAmount = 0.5f;
        [SerializeField] private AnimationCurve rumbleForceCurve;

        private Rigidbody _rb1;
        private Rigidbody _rb2;
        private PlayerController _controller1;
        private PlayerController _controller2;
        private float _snapbackTimer;
        private float _snapbackDurationTimer;
        private bool _isApplyingSnapback;

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

            ClampVelocityAtMaxDistance(_rb1, player1.position, player2.position, normalizedDistance1);
            ClampVelocityAtMaxDistance(_rb2, player2.position, player1.position, normalizedDistance2);

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

            if (_snapbackTimer > 0) return;

            var snapbackPlayer1 = ShouldApplySnapback(_controller1, normalizedDistance1);
            var snapbackPlayer2 = ShouldApplySnapback(_controller2, normalizedDistance2);

            // The OR here is used to prevent lazy evaluation of the ApplySnapbackForce
            if (snapbackPlayer1 || snapbackPlayer2)
            {
                _snapbackTimer = snapbackDelay;
                _snapbackDurationTimer = snapbackDuration;
                _isApplyingSnapback = true;
            }

            if (_isApplyingSnapback)
            {
                ContinueSnapbackForce(snapbackPlayer1, snapbackPlayer2, normalizedDistance1, normalizedDistance2);
            }
        }

        private bool CalculateBindingForce(float normalizedDistance, Transform player, Vector3 midpoint, float forceMagnitude, out Vector3 force, float forceAppliedNormalizedDistance)
        {
            if (normalizedDistance <= forceApplied)
            {
                force = Vector3.zero;
                InputSystem.ResetHaptics();
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

        private bool ShouldApplySnapback(PlayerController controller, float normalizedDistance)
        {
            if (normalizedDistance < snapbackThreshold) return false;
            if (controller.IsGrabbing || controller.Movement != Vector3.zero) return false;

            return true;
        }

        private void ContinueSnapbackForce(bool applyToPlayer1, bool applyToPlayer2, float normalizedDistance1, float normalizedDistance2)
        {
            if (snapbackMode == SnapbackMode.Duration)
            {
                _snapbackDurationTimer -= Time.fixedDeltaTime;
                if (_snapbackDurationTimer <= 0)
                {
                    _isApplyingSnapback = false;
                    return;
                }
            }

            if (applyToPlayer1) ApplySnapbackForceToPlayer(_rb1, player1.position, normalizedDistance1);
            if (applyToPlayer2) ApplySnapbackForceToPlayer(_rb2, player2.position, normalizedDistance2);
        }

        private void ApplySnapbackForceToPlayer(Rigidbody rb, Vector3 playerPosition, float normalizedDistance)
        {
            Vector3 midpoint = Vector3.Lerp(player1.position, player2.position, midpointAdjustment);
            Vector3 toMidpoint = midpoint - playerPosition;

            bool shouldApplyForce = snapbackMode == SnapbackMode.Duration ||
                                    (snapbackMode == SnapbackMode.Distance && normalizedDistance > snapbackThreshold);

            if (shouldApplyForce)
            {
                Vector3 snapbackForce = toMidpoint.normalized * snapbackForceMagnitude;
                rb.AddForce(snapbackForce, ForceMode.Force);
            }
            else if (snapbackMode == SnapbackMode.Distance)
            {
                _isApplyingSnapback = false;
            }
        }

        // TODO: Re-check for redundancy & efficiency
        private void ClampVelocityAtMaxDistance(Rigidbody rb, Vector3 playerPosition, Vector3 otherPlayerPosition, float normalizedDistance)
        {
            if (normalizedDistance > 0.90f) // We start restricting the movement of the player once we reach 90% of the max distance
            {
                Vector3 toOtherPlayer = otherPlayerPosition - playerPosition;
                Vector3 directionToOtherPlayer = toOtherPlayer.normalized;

                Vector3 velocityTowardsOtherPlayer = Vector3.Project(rb.velocity, directionToOtherPlayer);

                // Debug.Log($"normalizedDistance: {normalizedDistance}");

                if (Vector3.Dot(velocityTowardsOtherPlayer, directionToOtherPlayer) < 0)
                {
                    velocityTowardsOtherPlayer = Vector3.zero;
                }

                Vector3 perpendicularVelocity = rb.velocity - velocityTowardsOtherPlayer;

                // Here the max perpendicular speed is gradually reduced as we move towards the max distance
                float stretchFactor = Mathf.Clamp01((normalizedDistance - 0.90f) / 0.10f);
                float maxPerpendicularSpeed = Mathf.Lerp(rb.velocity.magnitude, rb.velocity.magnitude * 0.2f, stretchFactor);

                if (perpendicularVelocity.magnitude > maxPerpendicularSpeed)
                {
                    perpendicularVelocity = perpendicularVelocity.normalized * maxPerpendicularSpeed;
                }

                rb.velocity = velocityTowardsOtherPlayer + perpendicularVelocity;
            }
        }
    }
}


