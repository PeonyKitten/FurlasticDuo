using Game.Scripts.Patterns;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Game.States
{
    public class PauseState : IState<GameManager>
    {
        public void OnStateEnter(GameManager state)
        {
            Time.timeScale = 0;
            MenuManager.Instance.PushMenu(MenuManager.Instance.pauseMenu);
        }

        public void OnStateExit(GameManager state)
        {
            MenuManager.Instance.PopMenu();
            Time.timeScale = 1;
        }
    }
}