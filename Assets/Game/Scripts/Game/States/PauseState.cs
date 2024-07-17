using FD.Patterns;
using FD.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FD.Game.States
{
    public class PauseState : IState<GameManager>
    {
        public void OnStateEnter(GameManager state)
        {
            InputSystem.ResetHaptics();
            Time.timeScale = 0;
            MenuManager.Instance.PushMenu(MenuManager.Instance.pauseMenu);
        }

        public void OnStateExit(GameManager state)
        {
            InputSystem.ResumeHaptics();
            MenuManager.Instance.PopMenu();
            Time.timeScale = 1;
        }
    }
}