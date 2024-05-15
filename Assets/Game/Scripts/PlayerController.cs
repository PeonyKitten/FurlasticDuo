using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Game.Scripts
{
    [Serializable]
    public struct Spring
    {
        public float strength;
        public float damping;
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
        
        [Header("Jump")]
        [SerializeField] private bool canJump;
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private float jumpInputBuffer = 0.33f;
        [SerializeField] private float coyoteTime = 0.33f;
        
        [Header("Character Controller")]
        [SerializeField] private float rideHeight = 0.5f;
        [SerializeField] private float groundCheckLength = 1f;
        [SerializeField] private Spring rideSpring = new() { strength = 100, damping = 10 };
        [SerializeField] private Spring uprightJointSpring = new() { strength = 100, damping = 10 };
        [SerializeField] private Camera primaryCamera;

        public float speedFactor = 1f;
        
        private Quaternion _uprightJointTargetRotation = Quaternion.identity;
        private Rigidbody _rb;
        private Vector2 _movement;
        private Vector3 _goalVel;

        /// Camera to be used for camera-relative movement
        public Camera PrimaryCamera { get => primaryCamera; set => primaryCamera = value; }

        public float Mass => _rb.mass;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();

            if (!primaryCamera)
            {
                primaryCamera = Camera.main;
            }
        }

        private void OnJump()
        {
            // TODO: turn off raycast and floating force for a bit :)
            // set a velocity directly, no need for a force
            if (canJump)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
            }
        }

        private void UpdateUprightForce(float elapsedTime)
        {
            var currentRotation = transform.rotation;
            var goalRotation = _uprightJointTargetRotation.ShortestRotation(currentRotation);

            goalRotation.ToAngleAxis(out var rotDegrees, out var rotAxis);
            rotAxis.Normalize();

            var rotRadians = rotDegrees * Mathf.Deg2Rad;
        
            _rb.AddTorque((rotAxis * (rotRadians * uprightJointSpring.strength) - _rb.angularVelocity * uprightJointSpring.damping) * elapsedTime);
        }

        private Vector2 CalculateCameraRelativeMovement(Vector2 input)
        {
            var cameraForward = primaryCamera.transform.forward;
            var cameraRight = primaryCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            var forwardRelativeInput = input.y * cameraForward;
            var rightRelativeInput = input.x * cameraRight;

            var cameraRelativeMovement = forwardRelativeInput + rightRelativeInput;
            
            return cameraRelativeMovement.Flatten();
        }

        private void OnMovement(InputValue value)
        {
            var input = value.Get<Vector2>();

            if (useCameraRelativeMovement)
            {
                input = CalculateCameraRelativeMovement(input);
            }

            var _movementControlDisabledTimer = 0f;

            _movementControlDisabledTimer -= Time.deltaTime;

            if (_movementControlDisabledTimer > 0)
            {
                input = Vector2.zero;
            }

            input = Vector2.ClampMagnitude(input, 1f);

            if (input != Vector2.zero)
            {
                _uprightJointTargetRotation = Quaternion.LookRotation(input.Bulk(), Vector3.up);
            }

            _movement = input;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            var hitGround = Physics.Raycast(transform.position, Vector3.down, out var hitInfo, groundCheckLength);

            if (!hitGround) return;
            
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

            var springForce = displacement * rideSpring.strength - relativeVelocity * rideSpring.damping;
                
            _rb.AddForce(rayDir * springForce);

            var groundVel = Vector3.zero;
            if (hitBody)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, hitInfo.point);
                groundVel = hitBody.GetPointVelocity(hitInfo.point);
            }
            
            UpdateUprightForce(Time.deltaTime);
            
            var unitVel = _goalVel.normalized;
            var velDot = Vector3.Dot(_goalVel, unitVel);
            var accel = acceleration * accelerationFactorDot.Evaluate(velDot);
            
            // groundVel = velocity of the object we're standing on / GetPointVelocity()
            // speedFactor = used for special effects, not needed?
            
            var goalVel = _movement.Bulk() * maxSpeed * speedFactor;

            _goalVel = Vector3.MoveTowards(_goalVel,
                goalVel + groundVel,
                accel * Time.deltaTime);

            var neededAccel = (_goalVel - _rb.velocity) / Time.deltaTime;
            var maxAccel = maxAcceleration * maxAccelerationFactorDot.Evaluate(velDot);
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

            _rb.AddForce(Vector3.Scale(neededAccel * _rb.mass, forceScale));
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, _movement.Bulk());
        }
    }
}