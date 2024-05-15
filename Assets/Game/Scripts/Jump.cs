using System;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(PlayerController))]
    public class Jump : MonoBehaviour
    {
        [Header("Jump")]
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private float jumpInputBuffer = 0.33f;
        [SerializeField] private float coyoteTime = 0.33f;

        private float _coyoteTimeTimer;
        private float _jumpInputBufferTimer;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnJump()
        {
            _jumpInputBufferTimer = jumpInputBuffer;
        }

        private void FixedUpdate()
        {
            _jumpInputBufferTimer -= Time.deltaTime;
            
            if (_jumpInputBufferTimer > 0)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
                _jumpInputBufferTimer = 0;
            }
        }
    }
}
