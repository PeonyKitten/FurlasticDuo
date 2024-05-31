using System;
using Game.Scripts.Levels.Checkpoints;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class KillFloor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController _))
            {
                CheckpointSystem.Instance.Respawn();
            }
        }
    }
}
