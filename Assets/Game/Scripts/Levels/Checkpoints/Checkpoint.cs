using System.Collections.Generic;
using FD.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using FD.Utils;

namespace FD.Levels.Checkpoints
{
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint: MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPositions = new();
        [SerializeField] private bool resetPlayers = true;

        [Header("Callbacks")]
        public UnityEvent onRespawn;
        
        private BoxCollider _trigger;
        
        protected virtual void Start()
        {
            if (TryGetComponent(out _trigger))
            {
                _trigger.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController _))
            {
                CheckpointSystem.Instance.SaveCheckpoint(this);
            }
        }

        public void Spawn(PlayerController player, int index)
        {
            var position = spawnPositions.Count > index ? spawnPositions[index].position : transform.position; 
            player.transform.SetPositionAndRotation(position, Quaternion.identity);
            if (resetPlayers)
            {
                player.Reset();
            }
        }

        public void Spawn(List<PlayerController> players)
        {
            Debug.Log($"Spawning {players.Count} at {name}");
            for (var index = 0; index < players.Count; index++)
            {
                Spawn(players[index], index);
            }
            onRespawn?.Invoke();
        }

        #if UNITY_EDITOR
        [DrawGizmo(GizmoType.NonSelected)]
        private static void DrawCheckpointZone(Checkpoint checkpoint, GizmoType gizmoType)
        {
            var collider = checkpoint.GetComponent<BoxCollider>();
            Gizmos.color = Color.magenta;
            DebugExtension.DrawLocalCube(checkpoint.transform.localToWorldMatrix, collider.size, Color.cyan);
        }
        #endif

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Game/Scripts/Levels/Checkpoints/Checkpoint", true);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            
            foreach (var spawnPosition in spawnPositions)
            {
                Gizmos.DrawSphere(spawnPosition.position, 0.04f);
            }
        }
    }
}