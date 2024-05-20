using System;
using System.Collections.Generic;
using Game.Scripts.Game.States;
using Game.Scripts.Patterns;
using UnityEngine;

namespace Game.Scripts.Game
{
    public class GameManager: Singleton<GameManager>
    {
        public Camera uiCamera;
        private readonly Stack<IState<GameManager>> _stateHistory = new();
        private IState<GameManager> _currentState;

        private void Start()
        {
            ChangeState(new MainMenuState());
        }

        public void ChangeState(IState<GameManager> newState, bool isTemporary = false)
        {
            if (!isTemporary)
            {
                _currentState?.OnStateExit(this);
            }
            else if (_currentState != null)
            {
                _stateHistory.Push(_currentState);
            }
            Debug.Log("Changing States!");
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