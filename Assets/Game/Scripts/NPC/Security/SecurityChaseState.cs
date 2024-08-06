using FD.AI.FSM;
using UnityEngine;
using UnityEngine.Serialization;

namespace FD.NPC.Security
{
    public class SecurityChaseState : SecurityBaseState
    {
        [SerializeField] private string goToPatrolStateName = "Patrolling";

        private ChasePlayerSteeringBehaviour _chasePlayerBehaviour;

        public override void Init(GameObject owner, FSM fsm)
        {
            base.Init(owner, fsm);
            CachedSpeed = SecurityNPC.chaseSpeed;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            Debug.Log("Security NPC is chasing.");
            if (SteeringAgent.TryGetBehaviour(out _chasePlayerBehaviour))
            {
                _chasePlayerBehaviour.Weight = 1;
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!PlayerDetection.IsAnyPlayerInRange())
            {
                fsm.ChangeState(goToPatrolStateName);
            }
            else
            {
                var closestPlayer = PlayerDetection.GetClosestPlayer();
                if (_chasePlayerBehaviour != null)
                {
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
            if (!PlayerDetection.IsAnyPlayerInRange())
            {
                SecurityNPC.PlayAnimation("Alert");
            }
        }
    }
}