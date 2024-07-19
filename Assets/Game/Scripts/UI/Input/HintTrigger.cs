using System;
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
        
        [SerializeField] private Sprite hint;
        [SerializeField] private HintForPlayer hintFor = HintForPlayer.Either;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController player)) return;
            
            switch (hintFor)
            {
                case HintForPlayer.Cat when !player.IsCat:
                case HintForPlayer.Dog when !player.IsDog:
                    return;
                case HintForPlayer.Either:
                default:
                    player.Speak(hint);
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