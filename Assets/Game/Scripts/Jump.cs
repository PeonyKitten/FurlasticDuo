using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts
{
    [RequireComponent(typeof(PlayerController))]
    public class Jump : MonoBehaviour
    {
        [Header("Jump")] [SerializeField] private float jumpTime = 0.667f;
        [FormerlySerializedAs("jumpForce")] [SerializeField] private float jumpSpeed = 15f;
        [SerializeField] private float jumpInputBuffer = 0.33f;
        [SerializeField] private float coyoteTime = 0.33f;

        private float _coyoteTimeTimer;
        private float _jumpInputBufferTimer;

        private PlayerController _playerController;
        private Rigidbody _rb;

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
            
            
            var hitGround = Physics.Raycast(transform.position, Vector3.down, out var hitInfo, _playerController.GroundCheckLength);

            if (hitGround)
            {
                _coyoteTimeTimer = coyoteTime;
            }
            
            if (_jumpInputBufferTimer > 0 && _coyoteTimeTimer > 0)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, jumpSpeed, _rb.velocity.z);
                
                _playerController.DisableGroundCheckForSeconds(jumpTime);
                
                // Reset Timers
                _jumpInputBufferTimer = 0;
                _coyoteTimeTimer = 0;
            }
        }
    }
}
