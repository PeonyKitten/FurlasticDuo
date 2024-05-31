using System;
using System.Collections.Generic;
using Game.Scripts.Patterns;
using UnityEngine;

namespace Game.Scripts.Levels.Checkpoints
{
    public class CheckpointSystem: Singleton<CheckpointSystem>
    {
        [SerializeField] private Checkpoint spawnPoint;
        [SerializeField] private List<PlayerController> players;
        [SerializeField] private SaveCheckpointIcon savingIcon;
        
        private Checkpoint _lastCheckpoint;
        private bool _shouldSpawn;

        private void Start()
        {
            _lastCheckpoint = spawnPoint;
        }

        public void SaveCheckpoint(Checkpoint savePoint)
        {
            savingIcon.Play();
            _lastCheckpoint = savePoint;
        }

        public void Respawn()
        {
            Debug.Log("Respawning Players");
            _shouldSpawn = true;
        }

        public void FixedUpdate()
        {
            if (!_shouldSpawn) return;
            
            _lastCheckpoint.Spawn(players);
            _shouldSpawn = false;
        }
    }
}