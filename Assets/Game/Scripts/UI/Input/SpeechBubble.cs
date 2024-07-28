using System;
using FD.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace FD.UI.Input
{
    public class SpeechBubble: MonoBehaviour
    {
        [SerializeField] private Image bubble;
        [SerializeField] private Image bubbleContents;

        private void Start()
        {
            if (bubble is null)
            {
                Debug.LogWarning("Speech Bubble not set. Disabling. Input Hints will not work.");
                enabled = false;
                return;
            }
            Hide();
        }

        private void Update()
        {
            if (!bubble.enabled) return;
            
            bubble.transform.position = CameraUtils.Main.WorldToScreenPoint(transform.position);
        }

        public void Show()
        {
            bubble.gameObject.SetActive(true);
        }

        public void Say(Sprite sprite)
        {
            bubbleContents.sprite = sprite;
        }

        public void Hide()
        {
            bubble.gameObject.SetActive(false);
        }
    }
}