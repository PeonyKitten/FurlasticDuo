using System.Collections;
using FD.Game;
using FD.Game.States;
using FD.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace FD.UI.Menus
{
    public class MainMenu : Menu
    {
        protected override IEnumerator Generate()
        {
            yield return null;
            
            var root = Document.rootVisualElement;
            root.styleSheets.Add(style);

            var options = root.Q("Buttons");
            options.Clear();
            
            var coopPlayButton = options.Create<Button>("main-button");
            coopPlayButton.text = "Local Co-op";
            coopPlayButton.Focus();
            coopPlayButton.ApplyClickCallbacks(() =>
            {
                GameManager.Instance.ChangeState(new PlayState(PlayState.PlayMode.LocalCoop));
                coopPlayButton.SetEnabled(false);
            });
            
            var soloPlayButton = options.Create<Button>("main-button");
            soloPlayButton.text = "Singleplayer";
            soloPlayButton.ApplyClickCallbacks(() =>
            {
                GameManager.Instance.ChangeState(new PlayState(PlayState.PlayMode.SinglePlayer));
                soloPlayButton.SetEnabled(false);
            });
            
            var optionsButton = options.Create<Button>("main-button");
            optionsButton.text = "Options";
            optionsButton.ApplyClickCallbacks(() => MenuManager.Instance.PushMenu(MenuManager.Instance.optionsMenu));
            
            var creditsButton = options.Create<Button>("main-button");
            creditsButton.text = "Credits";
            // creditsButton.RegisterCallback<ClickEvent>(_ => MenuManager.Instance.PushMenu(MenuManager.Instance.creditsPage));

            var quitButton = options.Create<Button>("main-button");
            quitButton.text = "Quit";
            quitButton.ApplyClickCallbacks(Application.Quit);
            
            if (preventOverflowNavigation)
            {
                coopPlayButton.DisableNavigationInDirections(NavigationMoveEvent.Direction.Up);
                quitButton.DisableNavigationInDirections(NavigationMoveEvent.Direction.Down);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            MenuManager.Instance.mainMenu = this;
            Hide();
        }
    }
}
