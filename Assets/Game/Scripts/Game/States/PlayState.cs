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
        private readonly PlayMode _mode;

        public PlayState(PlayMode mode)
        {
            _mode = mode;
        }
        
        public void OnStateEnter(GameManager state)
        {
            // EventBus<GameStates>.Publish(GameStates.Running);

            GameManager.Instance.StartCoroutine(LoadPlaygroundScene(state));
        }

        private IEnumerator LoadPlaygroundScene(GameManager state)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(state.gameScene, LoadSceneMode.Additive);

            if (asyncLoad == null) yield break;
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            state.uiCamera.enabled = false;
            MenuManager.Instance.PopMenu();
            PlayManager.Instance.PlayGame(_mode, SceneManager.GetSceneByName(state.gameScene));
            MenuManager.Instance.HideDummy();
            
            state.pauseGameAction.Enable();
            state.pauseGameAction.performed += OpenPauseGameMenu;

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
            PlayManager.Instance.ResetGame();
            InputSystem.ResetHaptics();
            // EventBus<GameStates>.Publish(GameStates.Paused);
        }
    }
}