using System;
using System.Collections.Generic;
using FD.Game.States;
using FD.Patterns;
using FD.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FD.Game
{
    public class GameManager: Singleton<GameManager>
    {
        public InputAction pauseGameAction;
        
        public Camera uiCamera;
        public PlayerInputManager playerInputManager;
        private readonly Stack<IState<GameManager>> _stateHistory = new();
        private IState<GameManager> _currentState;

        private int _playerIndex;

        public List<PlayerInputHandler> inputHandlers = new();

        private void Start()
        {
            playerInputManager = GetComponent<PlayerInputManager>();
            
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

        private void OnPlayerJoined(PlayerInput playerInput)
        {
            Debug.Assert(_playerIndex <= 1, $"Invalid Player Index {_playerIndex}");

            var inputHandler = playerInput.GetComponent<PlayerInputHandler>();
            var playerType = _playerIndex == 0
                ? PlayerInputHandler.PlayerInputType.Cat
                : PlayerInputHandler.PlayerInputType.Dog;
            inputHandler.SetupPlayer(playerType);
            inputHandler.SetupGrabActions();
            inputHandlers.Add(inputHandler);
            _playerIndex++;

            if (_playerIndex >= 2)
            {
                playerInputManager.DisableJoining();
            }
        }

        public void SpawnDefaultInputHandler()
        {
            var inputHandler = Instantiate(playerInputManager.playerPrefab).GetComponent<PlayerInputHandler>();
            inputHandler.SetupPlayer(PlayerInputHandler.PlayerInputType.Combined);
            inputHandler.SetupGrabActions();
            inputHandlers.Add(inputHandler);
        }

        private void OnPlayerLeft(PlayerInput playerInput)
        {
            _playerIndex = Math.Max(_playerIndex - 1, 0);

            if (_playerIndex < 2)
            {
                playerInputManager.EnableJoining();
            }
        }

        public void ClearInputHandlers()
        {
            foreach (var inputHandler in inputHandlers)
            {
                Destroy(inputHandler.gameObject);
            }
            inputHandlers.Clear();
        }
    }
}