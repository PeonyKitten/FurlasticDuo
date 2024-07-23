using System;
using System.Collections.Generic;
using FD.Elastic;
using FD.Levels.Checkpoints;
using FD.Patterns;
using FD.Player;
using FD.UI.Input;
using FD.Utils;
using TMPro;
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
        
        [SerializeField] private InputUISettings inputSettings;

        [SerializeField] private TMP_Text player1Score;
        [SerializeField] private TMP_Text player2Score;
        
        public PlayerInputManager playerInputManager;
        private int _playerIndex;
        
        public PlayerController cat;
        public PlayerController dog;

        public ElasticForce ghost;
        
        public List<PlayerInputHandler> inputHandlers = new();
        
        private PlayerStart _playerStart;

        public PlayMode Mode { get; private set; } = PlayMode.Unassigned;
        public InputUISettings InputSettings => inputSettings;

        private void Start()
        {
            playerInputManager = GetComponent<PlayerInputManager>();
        }
        
        public void PlayGame(PlayMode mode, Scene scene)
        {
            CameraUtils.ResetMainCamera();
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
                player2Score.enabled = true;
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
            inputHandler.onScoreChanged.AddListener(UpdatePlayerScores);
            inputHandler.ResetScore();
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

            player2Score.enabled = false;
            inputHandler.onScoreChanged.AddListener(UpdatePlayerScores);
            inputHandler.ResetScore();
        }

        private void UpdatePlayerScores(int _)
        {
            if (cat?.InputHandler is not null && player1Score is not null)
            {
                player1Score.text = $"{cat.InputHandler.Score}";
            }
            if (dog?.InputHandler is not null && player2Score is not null)
            {
                player2Score.text = $"{dog.InputHandler.Score}";
            }
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
                inputHandler.ResetScore();
                inputHandler.onScoreChanged.RemoveAllListeners();
                Destroy(inputHandler.gameObject);
            }
            inputHandlers.Clear();
        }

        public Sprite GetInputSpriteFromMapping(PlayerController player, InputLayout.InputMapping inputMapping)
        {
            var device = player.InputHandler?.device ?? PlayerInputDevice.GenericGamepad;
            
            return inputSettings.GetSprite(inputMapping, device);
        }

        public Sprite GetInputSpriteFromAction(PlayerController player, FDPlayerActions.PlayerInputAction action)
        {
            var device = player.InputHandler?.device ?? PlayerInputDevice.GenericGamepad;

            var inputMapping = FDPlayerActions.GetMapping(player, action);
            return inputSettings.GetSprite(inputMapping, device);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static PlayerController[] GetPlayers()
        {
            if (!HasInstance())
            {
                return FindObjectsOfType<PlayerController>();
            }
            
            var players = new PlayerController[2];
            players[0] = Instance.cat;
            players[1] = Instance.dog;
            return players;
        }
    }
}
