using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts
{
    [RequireComponent(typeof(PlayerController))]
    public class Jump : MonoBehaviour
    {
        [Header("Jump")]
        [SerializeField] private float jumpTime = 0.667f;
        [SerializeField] private float jumpSpeed = 15f;
        [SerializeField] private float jumpInputBuffer = 0.33f;
        [SerializeField] private float coyoteTime = 0.33f;
        
        [Header("Character Controller Modifiers")]
        [SerializeField] private float jumpSpeedMultiplier = 1f;
        [SerializeField] private float jumpAngularSpeedMultiplier = 1f;
        [SerializeField] private float fallGravityMultiplier = 2f;

        [Header("Jump Effects")]
        [SerializeField] private GameObject jumpStartEffect;
        [SerializeField] private GameObject fallStartEffect;
        [SerializeField] private GameObject jumpEndEffect;
        [SerializeField] private bool ignoreGroundEffectSpawnRotation;
        [SerializeField] private bool globalFallEffect;
        
        [Header("Callbacks")]
        public UnityEvent onPlayerJump;
        public UnityEvent onPlayerFall;
        public UnityEvent onPlayerLand;

        public bool IsJumping { get; private set; }
        public bool IsFalling { get; private set; }
        
        private float _coyoteTimeTimer;
        private float _jumpInputBufferTimer;
        private float _jumpTimer;

        private PlayerController _playerController;
        private Rigidbody _rb;

        private RaycastHit _hitInfo;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _rb = GetComponent<Rigidbody>();
        }

        private void OnJump()
        {
            _jumpInputBufferTimer = jumpInputBuffer;
        }

        private void FixedUpdate()
        {
            _coyoteTimeTimer -= Time.deltaTime;
            _jumpInputBufferTimer -= Time.deltaTime;
            _jumpTimer -= Time.deltaTime;

            // Check if we're falling
            if (IsJumping && _rb.velocity.y < 0)
            {
                // If we weren't previously falling, notify listeners
                if (!IsFalling)
                {
                    OnFallStart();
                }
                
                IsFalling = true;
            }
            
            if (_jumpTimer > 0) return;
            
            var hitGround = Physics.Raycast(transform.position, Vector3.down, out _hitInfo, _playerController.GroundCheckLength);

            if (hitGround)
            {
                // If we were currently in the air and have touched the ground, we've landed
                if (IsJumping)
                {
                    OnJumpOver();
                }
                
                IsFalling = false;
                IsJumping = false;
                _coyoteTimeTimer = coyoteTime;
            }
            
            // Perform Jump
            if (_jumpInputBufferTimer > 0 && _coyoteTimeTimer > 0)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, jumpSpeed, _rb.velocity.z);
                
                _playerController.DisableGroundCheckForSeconds(jumpTime);
                
                // Reset Timers
                _jumpInputBufferTimer = 0;
                _coyoteTimeTimer = 0;
                _jumpTimer = jumpTime;
                IsJumping = true;
                OnJumpStart();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (IsJumping)
            {
                Gizmos.color = Color.blue;
            }
            if (IsFalling)
            {
                Gizmos.color = Color.yellow;
            }
            Gizmos.DrawSphere(transform.position, 0.2f);
        }

        private void OnJumpStart()
        {
            if (jumpStartEffect)
            {
                var rotation = ignoreGroundEffectSpawnRotation ? Quaternion.identity : Quaternion.FromToRotation(Vector3.up, _hitInfo.normal);
                Instantiate(jumpStartEffect, _hitInfo.point, rotation);
            }
            _playerController.speedFactor *= jumpSpeedMultiplier;
            _playerController.angularSpeedFactor *= jumpAngularSpeedMultiplier;
            onPlayerJump.Invoke();
        }
        
        // We've started falling
        private void OnFallStart()
        {
            if (fallStartEffect)
            {
                if (globalFallEffect)
                {
                    Instantiate(fallStartEffect, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(fallStartEffect, transform);
                }
            }
            _playerController.gravityMultiplier = Vector3.one * fallGravityMultiplier;
            onPlayerFall.Invoke();
        }

        // We've touched the ground
        private void OnJumpOver()
        {
            if (jumpEndEffect)
            {
                var rotation = ignoreGroundEffectSpawnRotation ? Quaternion.identity : Quaternion.FromToRotation(Vector3.up, _hitInfo.normal);
                Instantiate(jumpEndEffect, _hitInfo.point, rotation);
            }
            _playerController.gravityMultiplier = Vector3.one;
            _playerController.speedFactor /= jumpSpeedMultiplier;
            _playerController.angularSpeedFactor /= jumpAngularSpeedMultiplier;
            onPlayerLand.Invoke();
        }
    }
}
