using System.Collections;
using FD.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace FD.UI.Menus
{
    public class OptionsMenu: Menu
    {
        [SerializeField] private InputAction returnToPreviousMenuAction;
        private VisualElement _paw;
        private Button _backButton;
        
        protected override IEnumerator Generate()
        {
            yield return null;
            var root = Document.rootVisualElement;
            root.styleSheets.Add(style);

            _paw = root.Q("PawIndicator");

            _backButton = root.Q<Button>("BackButton");
            _backButton.RegisterCallback<ClickEvent>(_ => ReturnToPreviousMenu());
            _backButton.RegisterCallback<NavigationSubmitEvent>(_ => ReturnToPreviousMenu());
            
            if (preventOverflowNavigation)
            {
                _backButton.DisableNavigationInDirections(NavigationMoveEvent.Direction.Down);
            }
        }

        public override void Show()
        {
            returnToPreviousMenuAction.Enable();
            returnToPreviousMenuAction.performed += SelectBackOrReturnToPrevious;
            base.Show();
        }
        
        public override void Hide()
        {
            returnToPreviousMenuAction.performed -= SelectBackOrReturnToPrevious;
            base.Hide();
        }

        private void SelectBackOrReturnToPrevious(InputAction.CallbackContext obj)
        {
            if (_backButton is not null && _backButton.HasFocus())
            {
                ReturnToPreviousMenu();
            } else
            {
                _backButton?.Focus();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            MenuManager.Instance.optionsMenu = this;
            Hide();
        }
    }
}