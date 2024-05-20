using System.Collections;
using Game.Scripts.Game;
using Game.Scripts.Game.States;
using Game.Scripts.Utils;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Menus
{
    public class PauseMenu: Menu
    {
        protected override IEnumerator Generate()
        {
            yield return null;
            
            var root = Document.rootVisualElement;
            root.styleSheets.Add(style);
            
            var options = root.Q("Buttons");
            options.Clear();
            
            var playButton = options.Create<Button>("main-button");
            playButton.text = "Resume";
            playButton.Focus();
            playButton.RegisterCallback<ClickEvent>(_ => GameManager.Instance.RollbackState());
            
            var restartButton = options.Create<Button>("main-button");
            restartButton.text = "Restart";
            
            var optionsButton = options.Create<Button>("main-button");
            optionsButton.text = "Options";
            optionsButton.RegisterCallback<ClickEvent>(_ => MenuManager.Instance.PushMenu(MenuManager.Instance.optionsMenu));

            var mainMenuButton = options.Create<Button>("main-button");
            mainMenuButton.text = "Main Menu";
            mainMenuButton.RegisterCallback<ClickEvent>(_ => GameManager.Instance.ChangeState(new MainMenuState()));
        }

        protected override void Awake()
        {
            base.Awake();
            MenuManager.Instance.pauseMenu = this;
            Hide();
        }
    }
}