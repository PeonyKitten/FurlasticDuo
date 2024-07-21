using UnityEngine;
using UnityEngine.Events;

namespace FD.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class Jump : MonoBehaviour
    {
        [Header("Jump")]
        [SerializeField] private float jumpTime = 0.667f;
        [SerializeField] private float jumpSpeed = 15f;
        [SerializeField] private float jumpInputBuffer = 0.33f;
        [SerializeField] private float coyoteTime = 0.33f;
        [SerializeField] private LayerMask groundLayerMask;
        
        [Header("Ground Forces")]
        [SerializeField] private float jumpGroundForce = 10f;
        [SerializeField] private float landGroundForce = 100f;
        
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

        [Header("Ground Indicator")]
        [SerializeField] private GameObject groundIndicatorPrefab;
        [SerializeField] private float longRaycastLength = 100f;

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

        private GameObject _currentGroundIndicator;


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

            var ray = new Ray(transform.position, Vector3.down);
            var hitGround = Physics.Raycast(ray, out var hitInfo, longRaycastLength, groundLayerMask.value, QueryTriggerInteraction.Ignore);
            
            if (hitGround)
            {
                UpdateGroundIndicatorPosition(hitInfo);
            }

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

            if (hitGround && hitInfo.distance <= _playerController.GroundCheckLength)
            {
                _hitInfo = hitInfo;
                
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
                if (_hitInfo.rigidbody)
                {
                    _hitInfo.rigidbody.AddForceAtPosition(Vector3.down * jumpGroundForce, _hitInfo.point, ForceMode.Impulse);
                }
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

            if (groundIndicatorPrefab)
            {
                SpawnGroundIndicator(_hitInfo);
            }
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

            if (_hitInfo.rigidbody)
            {
                _hitInfo.rigidbody.AddForceAtPosition(Vector3.down * landGroundForce, _hitInfo.point, ForceMode.Impulse);
            }
            _playerController.gravityMultiplier = Vector3.one;
            _playerController.speedFactor /= jumpSpeedMultiplier;
            _playerController.angularSpeedFactor /= jumpAngularSpeedMultiplier;
            onPlayerLand.Invoke();

            DestroyGroundIndicator();
        }

        private void SpawnGroundIndicator(RaycastHit hitInfo)
        {
            DestroyGroundIndicator();
            _currentGroundIndicator = Instantiate(groundIndicatorPrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
        }

        private void UpdateGroundIndicatorPosition(RaycastHit hitInfo)
        {
            if (_currentGroundIndicator is null) return;

            _currentGroundIndicator.transform.position = hitInfo.point;
            _currentGroundIndicator.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }

        private void DestroyGroundIndicator()
        {
            if (_currentGroundIndicator)
            {
                Destroy(_currentGroundIndicator);
                _currentGroundIndicator = null;
            }
        }
    }
}
