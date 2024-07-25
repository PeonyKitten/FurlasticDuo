using System;
using UnityEngine;

namespace FD.UI.Input
{
    [Serializable]
    [CreateAssetMenu(menuName = "Control Layouts/Keyboard")]
    public class KeyboardLayout : InputLayout
    {
        public Sprite keyA;
        public Sprite keyB;
        public Sprite keyD;
        public Sprite keyI;
        public Sprite keyJ;
        public Sprite keyK;
        public Sprite keyL;
        public Sprite keyP;
        public Sprite keyQ;
        public Sprite keyS;
        public Sprite keyW;
        public Sprite keySpace;
        public Sprite keyEscape;
        
        public override Sprite GetSprite(InputMapping mapping)
        {
            return mapping switch
            {
                InputMapping.KeyA when keyA is not null => keyA,
                InputMapping.KeyB when keyB is not null => keyB,
                InputMapping.KeyD when keyD is not null => keyD,
                InputMapping.KeyI when keyI is not null => keyI,
                InputMapping.KeyJ when keyJ is not null => keyJ,
                InputMapping.KeyK when keyK is not null => keyK,
                InputMapping.KeyL when keyL is not null => keyL,
                InputMapping.KeyP when keyP is not null => keyP,
                InputMapping.KeyQ when keyQ is not null => keyQ,
                InputMapping.KeyS when keyS is not null => keyS,
                InputMapping.KeyW when keyW is not null => keyW,
                InputMapping.KeySpace when keySpace is not null => keySpace,
                InputMapping.KeyEscape when keyEscape is not null => keyEscape,
                _ => null
            };
        }
    }
}