using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Player;
using Game.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Levels.Checkpoints
{
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint: MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPositions = new();

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

        public void Spawn(List<PlayerController> players)
        {
            Debug.Log($"Spawning {players.Count} at {name}");
            for (var index = 0; index < players.Count; index++)
            {
                var position = spawnPositions.Count > index ? spawnPositions[index].position : transform.position; 
                players[index].transform.SetPositionAndRotation(position, Quaternion.identity);
            }
        }

        [DrawGizmo(GizmoType.NonSelected)]
        private static void DrawCheckpointZone(Checkpoint checkpoint, GizmoType gizmoType)
        {
            var collider = checkpoint.GetComponent<BoxCollider>();
            Gizmos.color = Color.magenta;
            DebugExtension.DrawLocalCube(checkpoint.transform.localToWorldMatrix, collider.size, Color.cyan);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Game/Scripts/Levels/Checkpoints/PlayerStart Icon", true);
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