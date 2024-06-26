using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Levels.Checkpoints;
using Game.Scripts.Patterns;
using Game.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace Game.Scripts.Game.States
{
    public class PlayState : IState<GameManager>
    {
        [Serializable]
        public enum PlayMode
        {
            SinglePlayer,
            LocalCoop,
        }

        private PlayMode _mode;

        public PlayState(PlayMode mode = PlayMode.SinglePlayer)
        {
            _mode = mode;
        }
        
        public void OnStateEnter(GameManager state)
        {
            // EventBus<GameStates>.Publish(GameStates.Running);

            GameManager.Instance.StartCoroutine(LoadPlaygroundScene(state));
        }

        private  IEnumerator LoadPlaygroundScene(GameManager state)
        {
            var asyncLoad = SceneManager.LoadSceneAsync("Playground-core", LoadSceneMode.Additive);

            if (asyncLoad == null) yield break;
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            state.uiCamera.enabled = false;
            MenuManager.Instance.PopMenu();
            MenuManager.Instance.HideDummy();
            
            state.pauseGameAction.Enable();
            state.pauseGameAction.performed += OpenPauseGameMenu;

            CheckpointSystem.Instance.ForceGrabValues();

            if (_mode == PlayMode.LocalCoop)
            {
                state.playerInputManager.EnableJoining();
            }
            else
            {
                state.SpawnDefaultInputHandler();
            }
        }

        public void OnStateResume(GameManager state)
        {
            state.pauseGameAction.Enable();
            state.pauseGameAction.performed += OpenPauseGameMenu;
            EventBus<GameEvents>.Publish(GameEvents.Unpaused);
        }

        private static void OpenPauseGameMenu(InputAction.CallbackContext obj)
        {
            var state = GameManager.Instance;
            
            state.ChangeState(new PauseState(), true);
            state.pauseGameAction.performed -= OpenPauseGameMenu;
            state.pauseGameAction.Disable();
            EventBus<GameEvents>.Publish(GameEvents.Paused);
        }

        public void OnStateExit(GameManager state)
        {
            state.pauseGameAction.performed -= OpenPauseGameMenu;
            state.pauseGameAction.Disable();
            state.ClearInputHandlers();
            InputSystem.ResetHaptics();
            state.playerInputManager.DisableJoining();
            // EventBus<GameStates>.Publish(GameStates.Paused);
        }
    }
}