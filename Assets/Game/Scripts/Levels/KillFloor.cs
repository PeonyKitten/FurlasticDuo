using System;
using Game.Scripts.Levels.Checkpoints;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Levels
{
    public class KillFloor : MonoBehaviour
    {
        public UnityEvent onKill;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController _))
            {
                CheckpointSystem.Instance.Respawn();
                onKill?.Invoke();
            }
        }
    }
}
