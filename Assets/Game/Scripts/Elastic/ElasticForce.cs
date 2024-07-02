// ElasticForce.cs
// Dhruv Saikia, Alvin Philips
// 2024-06-30
// Controls the elastic force between the players, along with snapping back. 

using System;
using System.Collections;
using Game.Scripts;
using Game.Scripts.Player;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Game.Scripts.Elastic
{
    public class ElasticForce : MonoBehaviour
    {
        [Serializable]
        public enum SnapbackMode
        {
            Impulse,
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
        [SerializeField] private float snapbackDuration = 0.1f;
        [SerializeField] private float snapbackDelay = 0.5f;
        [SerializeField] private float stopSnapbackDistance = 0.5f;

        [Header("Snap-Jump Settings")]
        [SerializeField] private float snapJumpHeight = 5f;
        [SerializeField] private float snapJumpDistance = 10f;
        [SerializeField] private float snapJumpDuration = 0.5f;
        [SerializeField, Range(0, 1)] private float snapJumpTriggerThreshold = 0.1f;

        [Header("Gizmo Settings")]
        [SerializeField] private bool showSnapJumpCurve = true;
        [SerializeField] private Color snapJumpCurveColor = Color.yellow;
        [SerializeField] private int curveResolution = 20;

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
        private bool _applySnapbackPlayer1;
        private bool _applySnapbackPlayer2;

        private bool _isSnapping = false;
        private bool _canSnapJump = false;
        private float _snapJumpTimer = 0f;
        private const float SnapJumpTimeout = 1f;
        private bool _player1Snapped = false;
        private bool _player2Snapped = false;
        private Vector3 _snapbackDirection;

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
            _snapJumpTimer -= Time.deltaTime;

            var player1Position = player1.position;
            var player2Position = player2.position;
            
            Vector3 midpoint = Vector3.Lerp(player1Position, player2Position, midpointAdjustment);

            var forceDirection1 = (midpoint - player1Position).NormalizedWithMagnitude(out var distance1);
            var forceDirection2 = (midpoint - player2Position).NormalizedWithMagnitude(out var distance2);
            
            float normalizedDistance1 = distance1 / (maxDistance * midpointAdjustment);
            float normalizedDistance2 = distance2 / (maxDistance * (1 - midpointAdjustment));

            ClampVelocityAtMaxDistance(_rb1, forceDirection1, normalizedDistance1);
            ClampVelocityAtMaxDistance(_rb2, forceDirection2, normalizedDistance2);

            const float forceAppliedAdjustmentFactor = 0.05f;
            var adjustedForceApplied = forceApplied - forceAppliedAdjustmentFactor;

            var forceAppliedDistance1 = (normalizedDistance1 - adjustedForceApplied) / (1f - adjustedForceApplied);
            var forceAppliedDistance2 = (normalizedDistance2 - adjustedForceApplied) / (1f - adjustedForceApplied);

            float forceMultiplier1 = forceCurve.Evaluate(forceAppliedDistance1);
            float forceMultiplier2 = forceCurve.Evaluate(forceAppliedDistance2);

            float forceMagnitude1 = Mathf.Min(forceMultiplier1 * baseForce, maxForce);
            float forceMagnitude2 = Mathf.Min(forceMultiplier2 * baseForce, maxForce);

            if (CalculateBindingForce(normalizedDistance1, forceDirection1, forceMagnitude1, out var force1, forceAppliedDistance1))
            {
                _rb1.AddForce(force1);
            }

            if (CalculateBindingForce(normalizedDistance2, forceDirection2, forceMagnitude2, out var force2, forceAppliedDistance2))
            {
                _rb2.AddForce(force2);
            }

            var snapbackPlayer1 = ShouldApplySnapback(_controller1, normalizedDistance1);
            var snapbackPlayer2 = ShouldApplySnapback(_controller2, normalizedDistance2);

            // The OR here is used to prevent lazy evaluation of the ApplySnapbackForce
            if (snapbackPlayer1 || snapbackPlayer2)
            {
                _snapbackDurationTimer = snapbackDuration;
                _applySnapbackPlayer1 = snapbackPlayer1;
                _applySnapbackPlayer2 = snapbackPlayer2;
                _isApplyingSnapback = true;
                _canSnapJump = true;
                _snapJumpTimer = SnapJumpTimeout;

                _player1Snapped = snapbackPlayer1;
                _player2Snapped = snapbackPlayer2;

                if (_player1Snapped)
                {
                    _snapbackDirection = (player2.position - player1.position).normalized;
                }
                else if (_player2Snapped)
                {
                    _snapbackDirection = (player1.position - player2.position).normalized;
                }

                // Use good ol' Impulses
                if (snapbackMode == SnapbackMode.Impulse)
                {
                    ApplySnapbackImpulse(snapbackPlayer1, snapbackPlayer2, forceDirection1, forceDirection2);
                }
            }

            if (_isApplyingSnapback)
            {
                ContinueSnapbackForce(_applySnapbackPlayer1, _applySnapbackPlayer2, forceDirection1, forceDirection2);
            }

            if (_canSnapJump)
            {
                CheckForSnapJumpTrigger(normalizedDistance1, normalizedDistance2);
            }
        }

        private void CheckForSnapJumpTrigger(float normalizedDistance1, float normalizedDistance2)
        {
            float averageDistance = (normalizedDistance1 + normalizedDistance2) / 2;
            if (averageDistance <= snapJumpTriggerThreshold && !_isSnapping && _snapJumpTimer > 0)
            {
                StartCoroutine(PerformSnapJump());
            }
            else if (_snapJumpTimer <= 0)
            {
                _canSnapJump = false;
            }
        }

        private IEnumerator PerformSnapJump()
        {
            _isSnapping = true;
            _canSnapJump = false;
            _isApplyingSnapback = false;

            Vector3 startPos1 = player1.position;
            Vector3 startPos2 = player2.position;
            Vector3 jumpDirection = _snapbackDirection;

            for (float t = 0; t < snapJumpDuration; t += Time.fixedDeltaTime)
            {
                float normalizedTime = t / snapJumpDuration;
                Vector3 newPos1 = CalculateSnapJumpPoint(startPos1, jumpDirection, normalizedTime);
                Vector3 newPos2 = CalculateSnapJumpPoint(startPos2, jumpDirection, normalizedTime);

                if (_player1Snapped)
                {
                    player1.position = newPos1;
                    player2.position = startPos2 + (newPos1 - startPos1);
                }
                else if (_player2Snapped)
                {
                    player2.position = newPos2;
                    player1.position = startPos1 + (newPos2 - startPos2);
                }

                yield return new WaitForFixedUpdate();
            }

            _isSnapping = false;
            _player1Snapped = false;
            _player2Snapped = false;
        }

        private void ApplySnapbackImpulse(bool snapbackPlayer1, bool snapbackPlayer2, Vector3 forceDirection1, Vector3 forceDirection2)
        {
            if (_snapbackTimer > 0) return;
            
            _isApplyingSnapback = false;
                    
            if (snapbackPlayer1)
            {
                _rb1.AddForce(forceDirection1 * snapbackForceMagnitude, ForceMode.Impulse);
            }

            if (snapbackPlayer2)
            {
                _rb2.AddForce(forceDirection2 * snapbackForceMagnitude, ForceMode.Impulse);
            }
                    
            _snapbackTimer = snapbackDelay;
        }

        private bool CalculateBindingForce(float normalizedDistance, Vector3 forceDirection, float forceMagnitude, out Vector3 force, float forceAppliedNormalizedDistance)
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

            force = forceDirection * forceMagnitude;
            var yFactor = Mathf.Abs(Vector3.Dot(force.normalized, Vector3.up));
            force.y *= yAxisForceFactorDot.Evaluate(yFactor);

            return true;
        }

        private bool ShouldApplySnapback(PlayerController controller, float normalizedDistance)
        {
            if (normalizedDistance < snapbackThreshold) return false;
            return !controller.IsGrabbing && controller.Movement == Vector3.zero;
        }

        private bool ShouldStopApplyingSnapbackForce()
        {
            switch (snapbackMode)
            {
                case SnapbackMode.Duration when _snapbackDurationTimer <= 0:
                case SnapbackMode.Distance when
                    (player1.position - player2.position).magnitude <= stopSnapbackDistance * maxDistance:
                case SnapbackMode.Impulse:
                    return true;
                default:
                    return false;
            }
        }

        private void ContinueSnapbackForce(bool applyToPlayer1, bool applyToPlayer2, Vector3 forceDirection1, Vector3 forceDirection2)
        {
            _snapbackDurationTimer -= Time.deltaTime;
            
            if (ShouldStopApplyingSnapbackForce()) {
                _isApplyingSnapback = false;
                return;
            }

            if (applyToPlayer1) ApplySnapbackForceToPlayer(_rb1, forceDirection1);
            if (applyToPlayer2) ApplySnapbackForceToPlayer(_rb2, forceDirection2);
        }

        private void ApplySnapbackForceToPlayer(Rigidbody rb, Vector3 forceDirection)
        {
            Vector3 snapbackForce = forceDirection * snapbackForceMagnitude;
            rb.AddForce(snapbackForce, ForceMode.Acceleration);
        }

        // TODO: Re-check for redundancy & efficiency
        private void ClampVelocityAtMaxDistance(Rigidbody rb, Vector3 directionToOtherPlayer, float normalizedDistance)
        {
            // We start restricting the movement of the player once we exceed 90% of the max distance
            if (normalizedDistance <= 0.90f) return;

            Vector3 velocityTowardsOtherPlayer = Vector3.Project(rb.velocity, directionToOtherPlayer);

            // Ignore force if we are moving away from the other player
            if (Vector3.Dot(velocityTowardsOtherPlayer, directionToOtherPlayer) < 0)
            {
                velocityTowardsOtherPlayer = Vector3.zero;
            }

            // NOTE: perpendicular velocity includes the velocity in the opposite direction, i.e. away from the other player as well
            Vector3 perpendicularVelocity = rb.velocity - velocityTowardsOtherPlayer;

            // Here the max perpendicular speed is gradually reduced as we move towards the max distance
            float stretchFactor = (normalizedDistance - 0.90f) * 10f;
            var adjustedPerpendicularVelocity = perpendicularVelocity * Mathf.Lerp(1f, 0.2f, stretchFactor);
            
            rb.velocity = velocityTowardsOtherPlayer + adjustedPerpendicularVelocity;
        }

        private void OnDrawGizmos()
        {
            if (!showSnapJumpCurve || player1 == null || player2 == null) return;

            Vector3 startPos;
            Vector3 jumpDirection;

            if (Application.isPlaying)
            {
                if (_player1Snapped)
                {
                    startPos = player1.position;
                    jumpDirection = _snapbackDirection;
                }
                else if (_player2Snapped)
                {
                    startPos = player2.position;
                    jumpDirection = _snapbackDirection;
                }
                else
                {
                    return; 
                }
            }
            else
            {
                startPos = player1.position;
                jumpDirection = (player2.position - player1.position).normalized;
            }

            Gizmos.color = snapJumpCurveColor;

            Vector3 previousPoint = startPos;
            for (int i = 1; i <= curveResolution; i++)
            {
                float t = i / (float)curveResolution;
                Vector3 point = CalculateSnapJumpPoint(startPos, jumpDirection, t);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }

        private Vector3 CalculateSnapJumpPoint(Vector3 startPos, Vector3 jumpDirection, float t)
        {
            float heightOffset = Mathf.Sin(t * Mathf.PI) * snapJumpHeight;
            Vector3 offset = jumpDirection * (t * snapJumpDistance) + Vector3.up * heightOffset;
            return startPos + offset;
        }
    }
}


