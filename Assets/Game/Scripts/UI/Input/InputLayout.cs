using System;
using UnityEngine;

namespace FD.UI.Input
{
    [Serializable]
    public abstract class InputLayout : ScriptableObject
    {
        [Serializable]
        public enum InputMapping
        {
            ButtonNorth,
            ButtonEast,
            ButtonSouth,
            ButtonWest,
            DPadUp,
            DPadRight,
            DPadDown,
            DPadLeft,
            LeftTrigger,
            RightTrigger,
            LeftButton,
            RightButton,
            StartButton,
        }
        
        public virtual Sprite GetSprite(InputMapping mapping)
        {
            return null;
        }
    }
}