using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Toys
{
    public class HamsterWheel : MonoBehaviour
    {
        public struct PlayerData
        {
            public float speedFactor;
            public Jump jump;
        }

        [SerializeField] private Transform wheel;
        [SerializeField] private float turnSpeed;

        private Dictionary<PlayerController, PlayerData> enteredPlayers = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                PlayerData playerData = new PlayerData();
                playerData.speedFactor = player.speedFactor;
                playerData.jump = player.GetComponent<Jump>();

                enteredPlayers.Add(player, playerData);

                player.ignoreGroundVelocity = true;
            }
        }

        private void FixedUpdate()
        {
            float currentRotation = 0;

            foreach (var player in enteredPlayers) 
            {
                if (player.Value.jump && player.Value.jump.IsJumping) continue;

                Vector3 _movement = player.Key.Movement;
                float turnSpeedFactor = Vector3.Dot(_movement, transform.forward);

                float moveSpeedFactor = Vector3.Dot(_movement, transform.right);
                float absMoveSpeedFactor = Mathf.Abs(moveSpeedFactor);
                player.Key.speedFactor = player.Value.speedFactor * absMoveSpeedFactor;

                currentRotation += turnSpeedFactor;
            }

            wheel.Rotate(transform.right, currentRotation * turnSpeed * Time.deltaTime);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                if (enteredPlayers.Remove(player, out var playerData))
                {
                    player.ignoreGroundVelocity = false;
                    player.speedFactor = playerData.speedFactor;
                }
            }
        }
    }
}
