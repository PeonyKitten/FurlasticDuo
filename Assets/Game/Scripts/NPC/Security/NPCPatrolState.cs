using FD.AI.SteeringBehaviours;
using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class SecurityNPCPatrolState : SecurityNPCBaseState
    {
        [FormerlySerializedAs("GoToChaseStateName")] public string goToChaseStateName = "Chasing";
        private FollowPathSteeringBehaviour _followPathBehaviour;
        private bool _wasStayingInPlace;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Security NPC is patrolling.");
            _followPathBehaviour = Security.steeringAgent.GetBehaviour<FollowPathSteeringBehaviour>();
            _wasStayingInPlace = Security.stayInPlace;
            UpdateMovementBehavior();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Security.playerDetection.CanSeePlayer(true))
            {
                fsm.ChangeState(goToChaseStateName);
            }

            if (_wasStayingInPlace != Security.stayInPlace)
            {
                _wasStayingInPlace = Security.stayInPlace;
                UpdateMovementBehavior();
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

        private void UpdateMovementBehavior()
        {
            if (_followPathBehaviour != null)
            {
                if (!Security.stayInPlace)
                {
                    _followPathBehaviour.Weight = 1;
                    _followPathBehaviour.onReachWaypoint.AddListener(OnReachWaypoint);
                    Security.SetSpeed(Security.GetSpeedForState("Patrolling"));
                }
                else
                {
                    _followPathBehaviour.Weight = 0;
                    _followPathBehaviour.onReachWaypoint.RemoveListener(OnReachWaypoint);
                    Security.SetSpeed(0);
                }
            }
        }
        private void OnReachWaypoint(Vector3 waypoint)
        {
            Security.PlayAnimation("Idle");
        }
    }
}