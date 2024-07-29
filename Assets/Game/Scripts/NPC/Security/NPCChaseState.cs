using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class SecurityNPCChaseState : SecurityNPCBaseState
    {
        [FormerlySerializedAs("GoToPatrolStateName")] public string goToPatrolStateName = "Patrolling";
        private ChasePlayerSteeringBehaviour _chasePlayerBehaviour;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Security NPC is chasing.");
            _chasePlayerBehaviour = Security.steeringAgent.GetBehaviour<ChasePlayerSteeringBehaviour>();
            if (_chasePlayerBehaviour != null)
            {
                _chasePlayerBehaviour.Weight = 1;
            }
            Security.SetSpeed(Security.GetSpeedForState("Chasing"));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Security.playerDetection.IsAnyPlayerInRange())
            {
                fsm.ChangeState(goToPatrolStateName);
            }
            else
            {
                var closestPlayer = Security.playerDetection.GetClosestPlayer();
                if (_chasePlayerBehaviour != null)
                {
                    if (_chasePlayerBehaviour.player != null && closestPlayer == null)
                    {
                        Debug.Log("Lost Players :(");
                    }
                    _chasePlayerBehaviour.player = closestPlayer;
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_chasePlayerBehaviour != null)
            {
                _chasePlayerBehaviour.Weight = 0;
                _chasePlayerBehaviour.player = null;
            }
            if (!Security.playerDetection.IsAnyPlayerInRange())
            {
                Security.PlayAnimation("Alert");
            }
        }
    }
}