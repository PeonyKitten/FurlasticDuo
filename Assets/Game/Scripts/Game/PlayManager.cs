using System;
using System.Collections.Generic;
using FD.Elastic;
using FD.Levels.Checkpoints;
using FD.Patterns;
using FD.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FD.Game
{
    [Serializable]
    public enum PlayMode
    {
        Unassigned,
        SinglePlayer,
        LocalCoop,
    }
    
    public class PlayManager : Singleton<PlayManager>
    {
        [SerializeField] private GameObject defaultPlayerStartPrefab;
        [SerializeField] private GameObject catPrefab;
        [SerializeField] private GameObject dogPrefab;
        [SerializeField] private GameObject ghostPrefab;
        
        public PlayerInputManager playerInputManager;
        private int _playerIndex;
        
        public PlayerController cat;
        public PlayerController dog;

        public ElasticForce ghost;
        
        public List<PlayerInputHandler> inputHandlers = new();
        
        private PlayerStart _playerStart;

        public PlayMode Mode { get; private set; } = PlayMode.Unassigned;

        private void Start()
        {
            playerInputManager = GetComponent<PlayerInputManager>();
        }
        
        public void PlayGame(PlayMode mode, Scene scene)
        {
            Mode = mode;
            _playerStart = FindFirstObjectByType<PlayerStart>(); 
            if (_playerStart is null)
            {
                _playerStart = Instantiate(defaultPlayerStartPrefab).GetComponent<PlayerStart>();
                SceneManager.MoveGameObjectToScene(_playerStart.gameObject, scene);
                _playerStart.transform.position = Vector3.zero;
            }
            
            CheckpointSystem.Instance.SaveCheckpoint(_playerStart);
            
            var players = FindObjectsOfType<PlayerController>();
            foreach (var playerController in players)
            {
                if (playerController.IsCat)
                {
                    cat = playerController;
                }

                if (playerController.IsDog)
                {
                    dog = playerController;
                }
            }

            if (players.Length != 2)
            {
                Debug.LogWarning($"Found {players.Length} player(s). Expected 2!");
            }

            if (cat is null)
            {
                cat = Instantiate(catPrefab).GetComponent<PlayerController>();
                SceneManager.MoveGameObjectToScene(cat.gameObject, scene);
                _playerStart.Spawn(cat, 0);
            }
            
            if (dog is null)
            {
                dog = Instantiate(dogPrefab).GetComponent<PlayerController>();
                SceneManager.MoveGameObjectToScene(dog.gameObject, scene);
                _playerStart.Spawn(dog, 1);
            }

            ghost = FindFirstObjectByType<ElasticForce>();
            if (ghost is null)
            {
                ghost = Instantiate(ghostPrefab).GetComponent<ElasticForce>();
                SceneManager.MoveGameObjectToScene(ghost.gameObject, scene);
            }

            ghost.player1 = cat.transform;
            ghost.player2 = dog.transform;
            
            ghost.GrabValues();
            
            //     set cinemachine target group players
            
            if (mode == PlayMode.LocalCoop)
            {
                playerInputManager.EnableJoining();
            }
            else
            {
                SpawnDefaultInputHandler();
            }
        }

        public void ResetGame()
        {
            ClearInputHandlers();
            playerInputManager.DisableJoining();

            ghost = null;

            cat = null;
            dog = null;
            
            _playerStart = null;
            _playerIndex = 0;

            Mode = PlayMode.Unassigned;
        }
        
        // ReSharper disable once UnusedMember.Local
        private void OnPlayerJoined(PlayerInput playerInput)
        {
            if (Mode != PlayMode.LocalCoop) return;
            
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
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnDefaultInputHandler()
        {
            var inputHandler = Instantiate(playerInputManager.playerPrefab).GetComponent<PlayerInputHandler>();
            inputHandler.SetupPlayer(PlayerInputHandler.PlayerInputType.Combined);
            inputHandler.SetupGrabActions();
            inputHandlers.Add(inputHandler);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnPlayerLeft(PlayerInput _)
        {
            if (Mode != PlayMode.LocalCoop) return;
            
            _playerIndex = Math.Max(_playerIndex - 1, 0);

            if (_playerIndex < 2)
            {
                playerInputManager.EnableJoining();
            }
        }

        private void ClearInputHandlers()
        {
            foreach (var inputHandler in inputHandlers)
            {
                Destroy(inputHandler.gameObject);
            }
            inputHandlers.Clear();
        }
    }
}
