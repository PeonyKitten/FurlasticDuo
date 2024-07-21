using System;
using UnityEngine;

namespace FD.UI.Input
{
    [Serializable]
    [CreateAssetMenu(menuName = "Control Layouts/Gamepad")]
    public class GamepadLayout : InputLayout
    {
        public GamepadLayout baseGamepad;
        public Sprite buttonNorth;
        public Sprite buttonEast;
        public Sprite buttonSouth;
        public Sprite buttonWest;
        public Sprite dPadUp;
        public Sprite dPadRight;
        public Sprite dPadDown;
        public Sprite dPadLeft;
        public Sprite leftTrigger;
        public Sprite rightTrigger;
        public Sprite leftButton;
        public Sprite rightButton;
        public Sprite startButton;

        public override Sprite GetSprite(InputMapping mapping)
        {
            return mapping switch
            {
                InputMapping.ButtonNorth when buttonNorth is not null => buttonNorth,
                InputMapping.ButtonEast when buttonEast is not null => buttonNorth,
                InputMapping.ButtonSouth when buttonSouth is not null => buttonSouth,
                InputMapping.ButtonWest when buttonWest is not null => buttonWest,
                InputMapping.DPadUp when dPadUp is not null => dPadUp,
                InputMapping.DPadRight when dPadRight is not null => dPadRight,
                InputMapping.DPadDown when dPadDown is not null => dPadDown,
                InputMapping.DPadLeft when dPadLeft is not null => dPadLeft,
                InputMapping.LeftTrigger when leftTrigger is not null => leftTrigger,
                InputMapping.RightTrigger when rightTrigger is not null => rightTrigger,
                InputMapping.LeftButton when leftButton is not null => leftButton,
                InputMapping.RightButton when rightButton is not null => rightButton,
                InputMapping.StartButton when startButton is not null => startButton,
                _ when baseGamepad is not null => baseGamepad.GetSprite(mapping),
                _ => null
            };
        }
    }
}