using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private float rideHeight = 0.5f;
        [SerializeField] private float groundCheckLength = 1f;

        [Header("Movement")]
        [SerializeField] private bool useCameraRelativeMovement = true;
        [SerializeField] private float playerSpeed = 20f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float acceleration = 200f;
        [SerializeField] private AnimationCurve accelerationFactorDot;
        [SerializeField] private float maxAcceleration = 150f;
        [SerializeField] private AnimationCurve maxAccelerationFactorDot;
        
        [Header("Jump")]
        [SerializeField] private bool canJump;
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private float jumpInputBuffer = 0.33f;
        [SerializeField] private float coyoteTime = 0.33f;
        
        [SerializeField] private Spring rideSpring = new() { strength = 100, damping = 10 };
        [SerializeField] private Spring uprightJointSpring = new() { strength = 100, damping = 10 };
        
        private Quaternion _uprightJointTargetRotation = Quaternion.identity;
        private Rigidbody _rb;
        private Vector2 _movement;
        private Transform _camera;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            
            // TODO: we might want to allow easily switching out our Camera
            var mainCamera = Camera.main;
            if (mainCamera) _camera = mainCamera.transform;
        }

        private void OnJump()
        {
            if (canJump)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        private void UpdateUprightForce(float elapsedTime)
        {
            var currentRotation = transform.rotation;
            var goalRotation = _uprightJointTargetRotation.ShortestRotation(currentRotation);

            goalRotation.ToAngleAxis(out var rotDegrees, out var rotAxis);
            rotAxis.Normalize();

            var rotRadians = rotDegrees * Mathf.Deg2Rad;
        
            _rb.AddTorque(rotAxis * (rotRadians * uprightJointSpring.strength) - _rb.angularVelocity * uprightJointSpring.damping);
        }

        private void OnMovement(InputValue value)
        {
            _movement = value.Get<Vector2>();

            if (useCameraRelativeMovement)
            {
                var projected = Quaternion.Inverse(_camera.rotation) * _movement.Bulk();
                _movement = projected.Flatten();
            }
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

            if (hitBody != null)
            {
                otherVelocity = hitBody.velocity;
            }

            var rayDirVelocity = Vector3.Dot(rayDir, velocity);
            var otherDirVelocity = Vector3.Dot(rayDir, otherVelocity);

            var relativeVelocity = rayDirVelocity - otherDirVelocity;

            var displacement = hitInfo.distance - rideHeight;

            var springForce = displacement * rideSpring.strength - relativeVelocity * rideSpring.damping;
                
            _rb.AddForce(rayDir * springForce);

            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, hitInfo.point);
            }
            
            UpdateUprightForce(Time.deltaTime);
            
            _movement.Normalize();

            _rb.AddForce(_movement.Bulk() * playerSpeed);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, _movement.Bulk());
        }
    }
}