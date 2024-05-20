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
        }

        private static void LoadPlayground(GameManager state)
        {
            var asyncLoad = SceneManager.LoadSceneAsync("Playground-scene", LoadSceneMode.Additive);

            Debug.Assert(asyncLoad != null, nameof(asyncLoad) + " != null");
            
            asyncLoad.completed += _ =>
            {
                state.uiCamera.enabled = false;
                MenuManager.Instance.PopMenu();
                MenuManager.Instance.HideDummy();
            };
        }

        public void OnUpdate(GameManager state)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                state.ChangeState(new PauseState(), true);
            }
        }

        public void OnStateExit(GameManager state)
        {
            // EventBus<GameStates>.Publish(GameStates.Paused);
        }
    }
}