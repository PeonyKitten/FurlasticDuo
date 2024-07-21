using FD.AI.SteeringBehaviours;
using UnityEngine;

namespace FD.NPC
{
    public class ChasePlayerSteeringBehaviour : ArriveSteeringBehaviour
    {
        public Transform player;

        public override Vector3 CalculateForce()
        {
            if (player == null) return Vector3.zero;
            Target = player.position;
            return base.CalculateForce();
        }

        public override void Reset()
        {
            player = null;
        }
    }
}