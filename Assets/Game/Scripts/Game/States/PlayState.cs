using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Patterns;
using Game.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace Game.Scripts.Game.States
{
    public class PlayState : IState<GameManager>
    {
        public void OnStateEnter(GameManager state)
        {
            // EventBus<GameStates>.Publish(GameStates.Running);
            LoadPlayground(state);
            state.pauseGameAction.Enable();
            state.pauseGameAction.performed += OpenPauseGameMenu;
        }

        private static void LoadPlayground(GameManager state)
        {
            var asyncLoad = SceneManager.LoadSceneAsync("Playground-core", LoadSceneMode.Additive);

            Debug.Assert(asyncLoad != null, nameof(asyncLoad) + " != null");
            
            asyncLoad.completed += _ =>
            {
                state.uiCamera.enabled = false;
                MenuManager.Instance.PopMenu();
                MenuManager.Instance.HideDummy();
            };
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
            // EventBus<GameStates>.Publish(GameStates.Paused);
        }
    }
}