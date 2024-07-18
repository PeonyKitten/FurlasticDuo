using FD.AI.SteeringBehaviours;
using UnityEngine;

namespace FD.NPC.Security
{
    public class MovementOff : MonoBehaviour
    {
        private SteeringAgent _steeringAgent;

        private void Awake()
        {
            _steeringAgent = GetComponent<SteeringAgent>();
        }

        public void DisableMovement()
        {
            if (_steeringAgent != null)
            {
                _steeringAgent.enabled = false;
            }
        }   

        public void EnableMovement()
        {
            if (_steeringAgent != null)
            {
                _steeringAgent.enabled = true;
            }
        }
    }
}