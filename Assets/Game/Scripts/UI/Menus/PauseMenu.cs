using System.Collections;
using Game.Scripts.Game;
using Game.Scripts.Game.States;
using Game.Scripts.Levels.Checkpoints;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Menus
{
    public class PauseMenu: Menu
    {
        [SerializeField] private InputAction returnToGameAction;
        private Button _resumeButton;

        protected override IEnumerator Generate()
        {
            yield return null;
            
            var root = Document.rootVisualElement;
            root.styleSheets.Add(style);
            
            var options = root.Q("Buttons");
            options.Clear();
            
            _resumeButton = options.Create<Button>("main-button");
            _resumeButton.text = "Resume";
            _resumeButton.Focus();
            _resumeButton.ApplyClickCallbacks(() => GameManager.Instance.RollbackState());
            
            var restartButton = options.Create<Button>("main-button");
            restartButton.text = "Restart from Checkpoint";
            restartButton.ApplyClickCallbacks(() =>
            {
                CheckpointSystem.Instance.Respawn();
                GameManager.Instance.RollbackState();
            });
            
            var optionsButton = options.Create<Button>("main-button");
            optionsButton.text = "Options";
            optionsButton.ApplyClickCallbacks(() => MenuManager.Instance.PushMenu(MenuManager.Instance.optionsMenu));

            var mainMenuButton = options.Create<Button>("main-button");
            mainMenuButton.text = "Main Menu";
            mainMenuButton.ApplyClickCallbacks(() => GameManager.Instance.ChangeState(new MainMenuState()));
            
            if (preventOverflowNavigation)
            {
                _resumeButton.DisableNavigationInDirections(NavigationMoveEvent.Direction.Up);
                mainMenuButton.DisableNavigationInDirections(NavigationMoveEvent.Direction.Down);
            }
        }

        public override void Show()
        {
            returnToGameAction.Enable();
            returnToGameAction.performed += SelectResumeOrReturnToGame;
            base.Show();
        }
        
        public override void Hide()
        {
            returnToGameAction.performed -= SelectResumeOrReturnToGame;
            base.Hide();
        }

        private void SelectResumeOrReturnToGame(InputAction.CallbackContext obj)
        {
            if (_resumeButton is not null && _resumeButton.HasFocus())
            {
                GameManager.Instance.RollbackState();
            } else
            {
                _resumeButton?.Focus();
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            MenuManager.Instance.pauseMenu = this;
            Hide();
        }
    }
}