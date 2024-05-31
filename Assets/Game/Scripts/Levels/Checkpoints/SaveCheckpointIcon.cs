using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Levels.Checkpoints
{
    [RequireComponent(typeof(Animator))]
    public class SaveCheckpointIcon : MonoBehaviour
    {
        private Animator _animator;
        private Image _image;
        private static readonly int LoadingAnimHash = Animator.StringToHash("Loading");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _image = GetComponent<Image>();
        }

        public void Play()
        {
            _image.enabled = true;
            _animator.enabled = true;
            _animator.Play(LoadingAnimHash);

            StartCoroutine(StopAnimation());
        }

        private IEnumerator StopAnimation()
        {
            yield return new WaitForSecondsRealtime(3f);
            
            _animator.enabled = false;
            _image.enabled = false;
        }
    }
}