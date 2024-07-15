using Game.Scripts.SteeringBehaviours;
using UnityEngine;

namespace Game.Scripts.NPC
{
    public class ChasePlayerSteeringBehaviour: ArriveSteeringBehaviour
    {
        private Transform _player;

        public void SetPlayer(Transform player)
        {
            _player = player;
        }

        public void ResetPlayer()
        {
            _player = null;
        }

        public override Vector3 CalculateForce()
        {
            if (_player is null) return Vector3.zero;

            Target = _player.position;

            return base.CalculateForce();
        }
    }
}