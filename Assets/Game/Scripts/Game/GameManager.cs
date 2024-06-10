using System;
using System.Collections.Generic;
using Game.Scripts.Game.States;
using Game.Scripts.Patterns;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Game
{
    public class GameManager: Singleton<GameManager>
    {
        public InputAction pauseGameAction;
        
        public Camera uiCamera;
        private readonly Stack<IState<GameManager>> _stateHistory = new();
        private IState<GameManager> _currentState;

        private void Start()
        {
            ChangeState(new MainMenuState());
        }

        public void ChangeState(IState<GameManager> newState, bool isTemporary = false, bool clearHistory = false)
        {
            if (!isTemporary)
            {
                _currentState?.OnStateExit(this);
                if (clearHistory)
                {
                    foreach (var previousState in _stateHistory)
                    {
                        previousState.OnStateExit(this);
                    }
                }
            }
            else if (_currentState != null)
            {
                _stateHistory.Push(_currentState);
            }
            Debug.Log("Changing States!" + _currentState + " to " + newState);
            _currentState = newState;
            _currentState.OnStateEnter(this);
        }

        private void Update()
        {
            _currentState?.OnUpdate(this);
        }

        /// <summary>
        /// Revert to a previous State, if one exists.
        /// </summary>
        public void RollbackState()
        {
            if (_stateHistory.Count == 0) return;
            _currentState?.OnStateExit(this);
            _currentState = _stateHistory.Pop();
            _currentState.OnStateResume(this);
        }

    }
}