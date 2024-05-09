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

        [SerializeField] private bool canJump = false;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private Spring rideSpring = new() { strength = 100, damping = 10 };
        [SerializeField] private Spring uprightJointSpring = new() { strength = 100, damping = 10 };
        
        private Quaternion _uprightJointTargetRotation = Quaternion.identity;
        private Rigidbody _rb;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
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
            var movement = value.Get<Vector2>();
            movement.Normalize();
            
            _rb.AddForce(movement.Bulk() * 10f);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            var hitGround = Physics.Raycast(transform.position, Vector3.down, out var hitInfo, groundCheckLength);

            if (!hitGround) return;
            
            var velocity = _rb.velocity;
            var rayDir = transform.TransformDirection(Vector3.down);

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
        }
    }
}