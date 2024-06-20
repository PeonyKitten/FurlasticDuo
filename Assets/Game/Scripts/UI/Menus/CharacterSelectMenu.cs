using System.Collections;
using Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Menus
{
    public class CharacterSelectMenu : Menu
    {
        public void OnPlayerJoined(PlayerInput player)
        {
            Debug.Log(player.devices[0].displayName);
        }

        public void OnPlayerLeft(PlayerInput player)
        {
            Debug.Log("Goodbye, " + player.devices[0].displayName);
        }
        
        protected override IEnumerator Generate()
        {
            yield return null;
            
            var root = Document.rootVisualElement;
            root.styleSheets.Add(style);

            var deviceList = root.Q("InputDeviceList");

            foreach (var device in Gamepad.all)
            {
                deviceList.Create<Label>().text = device.displayName;
            }
        }
    }
}
