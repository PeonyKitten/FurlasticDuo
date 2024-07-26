using System.Collections.Generic;
using System.Linq;
using FD.Game;
using FD.Patterns;
using FD.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FD.Levels.Checkpoints
{
    public class CheckpointSystem: Singleton<CheckpointSystem>
    {
        [SerializeField] private List<PlayerController> players;
        [SerializeField] private SaveCheckpointIcon savingIcon;
        
        private Checkpoint _lastCheckpoint;
        private bool _shouldSpawn;

        private void Update()
        {
#if UNITY_EDITOR
            if (Keyboard.current[Key.R].wasPressedThisFrame)
            {
                Respawn();
            }
#endif
        }

        public void SaveCheckpoint(Checkpoint savePoint)
        {
            ForceGrabValues();
            if (savingIcon)
            {
                savingIcon.Play();
            }
            _lastCheckpoint = savePoint;
        }

        private void ForceGrabValues()
        {
            // TODO: Really bad, @alvin fix ASAP
            players = PlayManager.GetPlayers().ToList();
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