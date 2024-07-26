using FD.AI.SteeringBehaviours;
using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class SecurityNPCPatrolState : SecurityNPCBaseState
    {
        [FormerlySerializedAs("GoToChaseStateName")] public string goToChaseStateName = "Chasing";
        private FollowPathSteeringBehaviour _followPathBehaviour;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Security NPC is patrolling.");
            _followPathBehaviour = Security.steeringAgent.GetBehaviour<FollowPathSteeringBehaviour>();
            if (_followPathBehaviour != null)
            {
                _followPathBehaviour.Weight = 1;
                _followPathBehaviour.onReachWaypoint.AddListener(OnReachWaypoint);
            }
            Security.SetSpeed(Security.GetSpeedForState("Patrolling"));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Security.playerDetection.GetClosestPlayer(true))
            {
                fsm.ChangeState(goToChaseStateName);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_followPathBehaviour != null)
            {
                _followPathBehaviour.Weight = 0;
                _followPathBehaviour.onReachWaypoint.RemoveListener(OnReachWaypoint);
            }
        }

        private void OnReachWaypoint(Vector3 waypoint)
        {
            Security.PlayAnimation("Idle");
        }
    }
}