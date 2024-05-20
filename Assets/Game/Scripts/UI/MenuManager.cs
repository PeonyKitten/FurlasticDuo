using System.Collections.Generic;
using Game.Scripts.Patterns;
using Game.Scripts.UI.Menus;

namespace Game.Scripts.UI
{
    public class MenuManager: Singleton<MenuManager>
    {
        public MainMenu mainMenu;
        public PauseMenu pauseMenu;
        public OptionsMenu optionsMenu;
        public Menu creditsPage;

        private readonly Stack<Menu> _menuStack = new();

        public void PushMenu(Menu menu)
        {
            if (_menuStack.TryPeek(out var currentMenu))
            {
                currentMenu.Hide();
            }
            
            _menuStack.Push(menu);
            
            // Force refresh
            menu.RefreshUI = true;
            menu.Show();
        }
        
        public bool PopMenu()
        {
            if (!_menuStack.TryPop(out var oldMenu))
            {
                return false;
            }

            ShowDummy();
            oldMenu.Hide();
            
            if (_menuStack.TryPeek(out var currentMenu))
            {
                // Force refresh
                currentMenu.RefreshUI = true;
                currentMenu.Show();
            }
            
            HideDummy();
            return true;
        }

        public void HideDummy()
        {
            // throw new System.NotImplementedException();
        }

        private void ShowDummy()
        {
            // throw new System.NotImplementedException();
        }

        private void Start()
        {
            
        }
    }
}