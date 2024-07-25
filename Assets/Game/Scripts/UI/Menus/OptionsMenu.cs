using System.Collections;
using FD.Game;
using FD.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace FD.UI.Menus
{
    public class OptionsMenu: Menu
    {
        [SerializeField] private InputAction returnToPreviousMenuAction;
        [SerializeField] private Material worldDistortMaterial;
        private Button _backButton;

        private Toggle _rumbleToggle;
        private Toggle _distortionToggle;
        private static readonly int MatHashEnableSkew = Shader.PropertyToID("_Enable_Skew");

        protected override IEnumerator Generate()
        {
            yield return null;
            var root = Document.rootVisualElement;
            root.styleSheets.Add(style);

            _rumbleToggle = root.Q<Toggle>("RumbleToggle");
            _rumbleToggle.value = PlayManager.Instance.GetRumble();
            _rumbleToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                PlayManager.Instance.SetRumble(evt.newValue);
            });

            _distortionToggle = root.Q<Toggle>("DistortionToggle");
            if (worldDistortMaterial)
            {
                _distortionToggle.value = Mathf.Approximately(worldDistortMaterial.GetFloat(MatHashEnableSkew), 1);
            }
            _distortionToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                if (worldDistortMaterial)
                {
                    worldDistortMaterial.SetFloat(MatHashEnableSkew, evt.newValue ? 1 : 0);
                }
            });

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