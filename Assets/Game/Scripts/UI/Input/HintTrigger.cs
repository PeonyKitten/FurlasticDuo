using System;
using FD.Game;
using FD.Player;
using UnityEngine;

namespace FD.UI.Input
{
    public class HintTrigger: MonoBehaviour
    {
        [Serializable]
        public enum HintForPlayer
        {
            Dog,
            Cat,
            Either
        }

        [Serializable]
        public enum HintType
        {
            InputMapping,
            InputAction,
            Sprite,
        }

        [SerializeField] private HintType hintType = HintType.InputAction;
        [SerializeField] private Sprite hint;
        [SerializeField] private InputLayout.InputMapping hintMapping;
        [SerializeField] private FDPlayerActions.PlayerInputAction hintAction;
        [SerializeField] private HintForPlayer hintFor = HintForPlayer.Either;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;

            var hintSprite = hintType switch
            {
                HintType.InputAction when PlayManager.HasInstance() => PlayManager.Instance.GetInputSpriteFromAction(
                    player, hintAction),
                HintType.InputMapping when PlayManager.HasInstance() => PlayManager.Instance.GetInputSpriteFromMapping(
                    player, hintMapping),
                _ => hint
            };

            switch (hintFor)
            {
                case HintForPlayer.Cat when !player.IsCat:
                case HintForPlayer.Dog when !player.IsDog:
                    return; 
                case HintForPlayer.Either:
                default:
                    player.Speak(hintSprite);
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            switch (hintFor)
            {
                case HintForPlayer.Cat when !player.IsCat:
                case HintForPlayer.Dog when !player.IsDog:
                    return;
                case HintForPlayer.Either:
                default:
                    player.StopSpeaking();
                    break;
            }
        }
    }
}