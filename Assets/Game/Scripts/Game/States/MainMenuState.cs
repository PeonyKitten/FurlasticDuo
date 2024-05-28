using Game.Scripts.Patterns;
using Game.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Game.States
{
    public class MainMenuState : IState<GameManager>
    {
        public void OnStateEnter(GameManager state)
        {
            state.uiCamera.enabled = true;
            
            for (var index = 1; index < SceneManager.loadedSceneCount; index++)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(index));
            }
            
            if (state.uiCamera.TryGetComponent(out AudioListener al))
            {
                al.enabled = true;
            }
            
            MenuManager.Instance.PushMenu(MenuManager.Instance.mainMenu);
        }

        public void OnStateExit(GameManager state)
        {
            // Disable UI AudioListener
            if (state.uiCamera.TryGetComponent<AudioListener>(out var al))
            {
                al.enabled = false;
            }
        }
    }
}