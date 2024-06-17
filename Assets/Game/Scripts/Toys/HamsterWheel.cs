using System;
using System.Collections.Generic;
using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.Toys
{
    public class HamsterWheel : MonoBehaviour
    {
        private struct PlayerData
        {
            public float SpeedFactor;
            public Jump Jump;
        }

        [Header("Hamster Wheel Settings")]
        [SerializeField] private Transform wheel;
        [SerializeField] private float turnSpeed;
        [SerializeField] private bool useWheelRadius = true;
        [SerializeField] private float wheelRadius = 3.5f;
        [Header("Door Settings")]
        [SerializeField] private Door door;
        [SerializeField] private float doorSpeedMultiplier;
        [SerializeField] private bool clampRotationAtEnds = true;
        [SerializeField] private bool disableRotationAtEnds = true;
        [SerializeField] private float rotationDisableDelay = 4.0f;

        private readonly Dictionary<PlayerController, PlayerData> _enteredPlayers = new();
        private float _rotationDisableTimer;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            
            var playerData = new PlayerData
            {
                SpeedFactor = player.speedFactor,
                Jump = player.GetComponent<Jump>()
            };

            _enteredPlayers.Add(player, playerData);

            player.ignoreGroundVelocity = true;
        }

        private void FixedUpdate()
        {
            _rotationDisableTimer -= Time.deltaTime;
            
            float currentRotation = 0;

            foreach (var (player, playerData) in _enteredPlayers) 
            {
                // Ignore movement if Player is airborne
                if (playerData.Jump is not null && playerData.Jump.IsJumping) continue;
                
                // Ignore movement if Player is outside trigger region
                var localPlayerPosition = wheel.InverseTransformPoint(player.transform.position);
                if (useWheelRadius && !wheel.localPosition.IsWithinCylinder(localPlayerPosition, wheelRadius, UtilsMath.Axis3D.X)) continue;

                var movement = player.Movement;
                var turnSpeedFactor = Vector3.Dot(movement, transform.forward);

                var moveSpeedFactor = Vector3.Dot(movement, transform.right);
                var absMoveSpeedFactor = Mathf.Abs(moveSpeedFactor);
                absMoveSpeedFactor = absMoveSpeedFactor * absMoveSpeedFactor;
                player.speedFactor = playerData.SpeedFactor * absMoveSpeedFactor;

                currentRotation += turnSpeedFactor;
            }

            if (_rotationDisableTimer > 0) return;
            
            // Affect the door if it's not null
            if (door)
            {
                door.IncrementOpenness(currentRotation * doorSpeedMultiplier);
                
                if (clampRotationAtEnds && (door.Openness <= 0 || door.Openness >= 1)) return;
            }
            
            wheel.Rotate(currentRotation * turnSpeed * Time.deltaTime, 0f, 0f);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            
            if (_enteredPlayers.Remove(player, out var playerData))
            {
                player.ignoreGroundVelocity = false;
                player.speedFactor = playerData.SpeedFactor;
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(wheel.position, wheel.forward * wheelRadius);
        }

        private void DisableRotation()
        {
            Debug.Log("Booom");
            if (!disableRotationAtEnds) return;
            _rotationDisableTimer = rotationDisableDelay;
        }

        private void Start()
        {
            Debug.Log("awooga");
            if (!door) return;

            door.scriptOverride = true;

            Debug.Log("awooga 2");
            door.onDoorOpen.AddListener(DisableRotation);
            door.onDoorClose.AddListener(DisableRotation);
        }

        private void OnDestroy()
        {
            if (!door) return;
            
            door.onDoorOpen.RemoveListener(DisableRotation);
            door.onDoorClose.RemoveListener(DisableRotation);
        }
    }
}
