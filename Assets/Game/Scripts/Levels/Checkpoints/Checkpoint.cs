using System.Collections.Generic;
using FD.Player;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Levels.Checkpoints
{
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint: MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPositions = new();
        [SerializeField] private bool initialSavePoint = false;
        [SerializeField] private bool resetPlayers = true;

        [Header("Callbacks")]
        public UnityEvent onRespawn;
        
        private BoxCollider _trigger;
        
        private void Start()
        {
            if (TryGetComponent(out _trigger))
            {
                _trigger.isTrigger = true;
            }

            if (initialSavePoint)
            {
                CheckpointSystem.Instance.SaveCheckpoint(this);
            }
        }

        private void OnValidate()
        {
            _trigger = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController _))
            {
                CheckpointSystem.Instance.SaveCheckpoint(this);
            }
        }

        public void Spawn(List<PlayerController> players)
        {
            Debug.Log($"Spawning {players.Count} at {name}");
            for (var index = 0; index < players.Count; index++)
            {
                players[index].transform.SetPositionAndRotation(spawnPositions[index].position, Quaternion.identity);
                if (resetPlayers)
                {
                    players[index].Reset();
                }
            }
            onRespawn?.Invoke();
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            
            foreach (var spawnPosition in spawnPositions)
            {
                Gizmos.DrawWireSphere(spawnPosition.position, 0.04f);
            }
        }
    }
}