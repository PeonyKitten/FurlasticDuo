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

        [SerializeField] private Transform wheel;
        [SerializeField] private float turnSpeed;
        [SerializeField] private bool useWheelRadius = true;
        [SerializeField] private float wheelRadius = 3.5f;

        [SerializeField] public Door door;
        [SerializeField] private float doorSpeedMultiplier;

        private readonly Dictionary<PlayerController, PlayerData> _enteredPlayers = new();

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

            wheel.Rotate(currentRotation * turnSpeed * Time.deltaTime, 0f, 0f);

            // Affect the door if it's not null
            if (door)
            {
                door.IncrementOpenness(currentRotation * doorSpeedMultiplier);
            }
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
    }
}
