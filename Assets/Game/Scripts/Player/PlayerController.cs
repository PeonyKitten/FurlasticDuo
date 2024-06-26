using System;
using Game.Scripts.Game.States;
using Game.Scripts.Grab;
using Game.Scripts.Misc;
using Game.Scripts.Patterns;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    [Serializable]
    public enum Player
    {
        Cat,
        Dog,
    }

    [Serializable]
    public enum PlayerInteraction
    {
        CatOnly,
        DogOnly,
        Either,
        Both
    }
    
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private bool useCameraRelativeMovement = true;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float acceleration = 200f;
        [SerializeField] private AnimationCurve accelerationFactorDot;
        [SerializeField] private float maxAcceleration = 150f;
        [SerializeField] private AnimationCurve maxAccelerationFactorDot;
        [SerializeField] private Vector3 forceScale = new(1, 0, 1);

        [Header("Character Controller")]
        [SerializeField] private float rideHeight = 0.5f;
        [SerializeField] private float groundCheckLength = 1f;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField, Range(0, 90)] private float maxSlopeAngleDeg = 45;
        [SerializeField] private Spring rideSpring = new() { strength = 100, damping = 10 };
        [SerializeField] private Spring uprightJointSpring = new() { strength = 100, damping = 10 };
        [SerializeField] private Camera primaryCamera;
        [SerializeField] private bool disableSteepSlopeMovement = true;

        public Player playerType = Player.Cat;
        public Vector3 gravityMultiplier = Vector3.one;
        public float speedFactor = 1f;
        public float accelerationFactor = 1f;
        public float angularSpeedFactor = 1f;
        public bool ignoreGroundVelocity;

        private Rigidbody _rb;
        private Vector2 _movement;
        private Vector3 _goalVel;
        private float _movementControlDisabledTimer;
        private float _groundCheckDisabledTimer;

        private Grabbing _grabbing;

        /// Camera to be used for camera-relative movement
        public Camera PrimaryCamera { get => primaryCamera; set => primaryCamera = value; }
        public float Mass => _rb.mass;
        public Quaternion TargetRotation { get; private set; } = Quaternion.identity;
        public float GroundCheckLength => groundCheckLength;
        public float MaximumSpeed => maxSpeed * speedFactor;
        public Vector3 Movement => _movement.Bulk();
        public bool IsGrabbing => _grabbing.IsGrabbing;
        public bool IsDog => playerType == Player.Dog;
        public bool IsCat => playerType == Player.Cat;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _grabbing = GetComponent<Grabbing>();

            if (!primaryCamera)
            {
                primaryCamera = Camera.main;
            }
        }

        public void OnMovement(InputValue value)
        {
            var input = value.Get<Vector2>();

            if (useCameraRelativeMovement)
            {
                input = primaryCamera.CalculateRelativeMovement(input);
            }

            _movementControlDisabledTimer -= Time.deltaTime;

            if (_movementControlDisabledTimer > 0)
            {
                input = Vector2.zero;
            }

            input = Vector2.ClampMagnitude(input, 1f);

            if (input != Vector2.zero)
            {
                TargetRotation = Quaternion.LookRotation(input.Bulk(), Vector3.up);
            }

            _movement = input;
        }

        private void FixedUpdate()
        {
            _groundCheckDisabledTimer -= Time.deltaTime;

            var hitGround = FloatPlayerAboveGround(out var hitInfo, out var groundVel);

            HoldPlayerUpright(Time.deltaTime);
            
            var groundAngle = Vector3.Angle(Vector3.up, hitInfo.normal);
            
            // TODO: speed factor bad. fix @alvin
            if (speedFactor > 1)
            {
                Debug.LogWarning("SpeedFactor > 1. Clamping to 1");
                speedFactor = 1;
            }
            
            HandleMovement(groundVel, groundAngle);

            var effectiveGravity = Vector3.Scale(Physics.gravity, gravityMultiplier);

            // Slopes
            if (hitGround)
            {
                HandleStickingToSlopes(groundAngle, effectiveGravity, hitInfo.normal);
            }
            
            // Apply gravity
            _rb.AddForce(effectiveGravity, ForceMode.Acceleration);
        }

        private bool FloatPlayerAboveGround(out RaycastHit hitInfo, out Vector3 groundVel)
        {
            groundVel = Vector3.zero;
            
            var groundRay = new Ray(transform.position, Vector3.down);
            
            var hitGround = Physics.Raycast(groundRay, out hitInfo, groundCheckLength, groundLayerMask.value, QueryTriggerInteraction.Ignore);

            if (!hitGround || _groundCheckDisabledTimer > 0) return false;
            
            var velocity = _rb.velocity;
            var rayDir = -hitInfo.normal;

            var otherVelocity = Vector3.zero;
            var hitBody = hitInfo.rigidbody;

            if (hitBody)
            {
                otherVelocity = hitBody.velocity;
            }

            var rayDirVelocity = Vector3.Dot(rayDir, velocity);
            var otherDirVelocity = Vector3.Dot(rayDir, otherVelocity);
            var relativeVelocity = rayDirVelocity - otherDirVelocity;

            var displacement = hitInfo.distance - rideHeight;

            var springForce = rideSpring.CalculateSpringForce(displacement, relativeVelocity);

            _rb.AddForce(rayDir * springForce);

            // Apply force to the Rigidbody we're standing on
            if (hitBody)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, hitInfo.point);
                groundVel = hitBody.GetPointVelocity(hitInfo.point);
                Debug.DrawRay(transform.position, groundVel, Color.yellow);
            }

            return true;
        }

        private void HandleMovement(Vector3 groundVel, float groundAngle)
        {
            // Do not move if we're on too steep of a slope
            if (disableSteepSlopeMovement && groundAngle > maxSlopeAngleDeg) return;

            var movement = _movement.Bulk();
            var velDot = Vector3.Dot(movement, _goalVel.normalized);
            
            var accel = acceleration * accelerationFactorDot.Evaluate(velDot);

            var goalVel = movement * MaximumSpeed;

            var totalVel = goalVel;
            
            if (!ignoreGroundVelocity)
            {
                totalVel += groundVel;
            }

            _goalVel = Vector3.MoveTowards(_goalVel,
                totalVel,
                accel * Time.deltaTime);

            var neededAccel = (_goalVel - _rb.velocity) / Time.deltaTime * accelerationFactor;
            var maxAccel = maxAcceleration * maxAccelerationFactorDot.Evaluate(velDot);

            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            _rb.AddForce(Vector3.Scale(neededAccel * _rb.mass, forceScale));
        }

        private void HandleStickingToSlopes(float groundAngle, Vector3 gravity, Vector3 groundNormal)
        {
            if (groundAngle > maxSlopeAngleDeg) return;
            
            var gravityComponent = Vector3.ProjectOnPlane(gravity, groundNormal);
            var counterForce = -gravityComponent;
            
            _rb.AddForce(counterForce, ForceMode.Acceleration);
            
            Debug.DrawRay(transform.position, counterForce.normalized);
        }
        
        private void HoldPlayerUpright(float elapsedTime)
        {
            var currentRotation = transform.rotation;
            var goalRotation = TargetRotation.ShortestRotation(currentRotation);

            goalRotation.ToAngleAxis(out var rotDegrees, out var rotAxis);
            rotAxis.Normalize();

            var rotRadians = rotDegrees * Mathf.Deg2Rad;
        
            _rb.AddTorque((rotAxis * (rotRadians * uprightJointSpring.strength * angularSpeedFactor) - _rb.angularVelocity * (uprightJointSpring.damping / angularSpeedFactor)) * elapsedTime);
        }
        
        public void DisableGroundCheckForSeconds(float delay)
        {
            _groundCheckDisabledTimer = delay;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, _movement.Bulk());
        }
    }
}