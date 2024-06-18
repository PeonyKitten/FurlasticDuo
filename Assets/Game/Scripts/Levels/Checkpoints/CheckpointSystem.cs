using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Patterns;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Levels.Checkpoints
{
    public class CheckpointSystem: Singleton<CheckpointSystem>
    {
        [SerializeField] private List<PlayerController> players;
        [SerializeField] private SaveCheckpointIcon savingIcon;
        
        private Checkpoint _lastCheckpoint;
        private bool _shouldSpawn;

        public void SaveCheckpoint(Checkpoint savePoint)
        {
            ForceGrabValues();
            savingIcon.Play();
            _lastCheckpoint = savePoint;
        }

        public void ForceGrabValues()
        {
            // TODO: Really bad, @alvin fix ASAP
            players = FindObjectsOfType<PlayerController>().ToList();
            savingIcon = FindObjectOfType<SaveCheckpointIcon>();
        }


        public void Respawn()
        {
            ForceGrabValues();
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