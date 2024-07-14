using UnityEngine;

namespace Game.Scripts.Levels.Checkpoints
{
    public class PlayerStart : Checkpoint
    {
        protected override void Start()
        {
            base.Start();
            
            CheckpointSystem.Instance.SaveCheckpoint(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Game/Scripts/Levels/Checkpoints/PlayerStart_Players", true);
        }
    }
}