using FD.AI.FSM;
using FD.AI.SteeringBehaviours;
using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class SecurityNPCPatrolState : SecurityNPCBaseState
    {
        [SerializeField] private string goToChaseStateName = "Chasing";
        private FollowPathSteeringBehaviour _followPathBehaviour;

        public override void Init(GameObject owner, FSM fsm)
        {
            base.Init(owner, fsm);
            CachedSpeed = SecurityNPC.patrolSpeed;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Debug.Log("Security NPC is patrolling.");
            if (SteeringAgent.TryGetBehaviour(out _followPathBehaviour))
            {
                _followPathBehaviour.Weight = 1;
                _followPathBehaviour.onReachWaypoint.AddListener(OnReachWaypoint);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (PlayerDetection.CanSeePlayer(true))
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
            SecurityNPC.PlayAnimation("Idle");
        }
    }
}