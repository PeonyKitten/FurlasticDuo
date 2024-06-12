using System;
using UnityEngine;
using UnityEngine.XR;

namespace Game.Scripts.NPC
{
    public class ScaredLiftBot : MonoBehaviour
    {
        [Serializable]
        private enum BotState
        {
            Idle,
            Walking,
            Fleeing,
            Alert,
            Panic,
        }

        [SerializeField] private BotState state = BotState.Idle;

        private Animator _animator;
        
        private static readonly int AnimHashStartMoving = Animator.StringToHash("StartMoving"); 
        private static readonly int AnimHashStopMoving = Animator.StringToHash("StopMoving"); 
        private static readonly int AnimHashAlert = Animator.StringToHash("Alert"); 
        private static readonly int AnimHashFlee = Animator.StringToHash("Flee"); 

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void ChangeState(BotState newState)
        {
            state = newState;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (state != BotState.Idle) return;
            
            if (other.TryGetComponent(out PlayerController player) && player.IsDog)
            {
                _animator.SetTrigger(AnimHashAlert);
                ChangeState(BotState.Alert);
            } 
        }

        public void StartBarkReaction()
        {
            _animator.SetTrigger(AnimHashFlee);
            ChangeState(BotState.Fleeing);
        }

        public void StopBarkReaction()
        {
        }
    }
}
