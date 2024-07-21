using FD.Player;
using UnityEngine;

namespace FD.UI.Input
{
    [CreateAssetMenu(menuName = "FD/Input UI Settings")]
    public class InputUISettings : ScriptableObject
    {
        [SerializeField] private GamepadLayout genericGamepad;
        [SerializeField] private GamepadLayout playStation4Layout;
        [SerializeField] private GamepadLayout playStation5Layout;
        [SerializeField] private GamepadLayout xboxLayout;
        [SerializeField] private KeyboardLayout keyboardLayout;

        public Sprite GetSprite(InputLayout.InputMapping mapping, PlayerInputDevice device)
        {
            return device switch
            {
                PlayerInputDevice.Xbox => xboxLayout.GetSprite(mapping),
                PlayerInputDevice.PS5 => playStation5Layout.GetSprite(mapping),
                PlayerInputDevice.PS4 => playStation4Layout.GetSprite(mapping),
                PlayerInputDevice.GenericGamepad => genericGamepad.GetSprite(mapping),
                PlayerInputDevice.Keyboard => keyboardLayout.GetSprite(mapping),
                _ => null
            };
        }
    }
}