using System.Collections;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Menus
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
            returnToPreviousMenuAction.Disable();
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