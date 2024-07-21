using System;
using FD.Player;
using UnityEngine;

namespace FD.UI.Input
{
    [Serializable]
    public class FDPlayerActions: ScriptableObject
    {
        [Serializable]
        public enum PlayerInputAction
        {
            Jump,
            Bark,
            Grab,
        }

        public static InputLayout.InputMapping GetMapping(PlayerController player, PlayerInputAction action)
        {
            var combined = player.InputHandler.InputType == PlayerInputHandler.PlayerInputType.Combined;
            var isKeyboard = player.InputHandler.device == PlayerInputDevice.Keyboard;

            var mapping = action switch
            {
                PlayerInputAction.Bark => isKeyboard
                    ? InputLayout.InputMapping.KeyB
                    : InputLayout.InputMapping.ButtonWest,
                PlayerInputAction.Jump => isKeyboard
                    ? InputLayout.InputMapping.KeySpace
                    : InputLayout.InputMapping.ButtonSouth,
                PlayerInputAction.Grab when player.IsCat => isKeyboard
                    ? InputLayout.InputMapping.KeyQ
                    : InputLayout.InputMapping.LeftTrigger,
                PlayerInputAction.Grab when player.IsDog => isKeyboard
                    ? InputLayout.InputMapping.KeyP
                    : InputLayout.InputMapping.RightTrigger,
                _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
            };

            return mapping;
        }
    }
}