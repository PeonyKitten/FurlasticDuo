using System.Collections;
using Game.Scripts.Utils;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Menus
{
    public class OptionsMenu: Menu
    {
        private VisualElement _paw;
        protected override IEnumerator Generate()
        {
            yield return null;
            
            var root = Document.rootVisualElement;
            root.styleSheets.Add(style);

            _paw = root.Q("PawIndicator");

            var back = root.Q<Button>("BackButton");
            back.RegisterCallback<ClickEvent>(_ => ReturnToPreviousMenu());
        }

        protected override void Awake()
        {
            base.Awake();
            MenuManager.Instance.optionsMenu = this;
            Hide();
        }
    }
}