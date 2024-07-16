using FD.Levels.Checkpoints;
using FD.Player;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Levels
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
