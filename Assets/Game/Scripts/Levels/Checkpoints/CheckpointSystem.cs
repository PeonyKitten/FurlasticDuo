using System.Collections.Generic;
using System.Linq;
using FD.Patterns;
using FD.Player;
using UnityEngine;

namespace FD.Levels.Checkpoints
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
            if (savingIcon)
            {
                savingIcon.Play();
            }
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