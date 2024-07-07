using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Barking
{
    public class Bark : MonoBehaviour
    {
        public float barkRadius = 5f;
        public AudioClip barkSound;
        [SerializeField] private float barkDelay = 0.5f;
        [SerializeField] private GameObject barkEffect;

        private AudioSource _audioSource;
        private float _barkTimer = 0f;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnBark()
        {
            if (_barkTimer > 0) return;
            
            if (barkSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(barkSound); 
            }

            if (barkEffect)
            {
                var effect = Instantiate(barkEffect, transform.position, Quaternion.identity);
                effect.transform.localScale = Vector3.one * barkRadius * 2;
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, barkRadius);
            foreach (var hitCollider in hitColliders)
            {
                var npcReaction = hitCollider.GetComponentInChildren<IBarkReaction>();
                npcReaction?.React(this);
            }

            _barkTimer = barkDelay;
        }

        private void Update()
        {
            _barkTimer -= Time.deltaTime;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, barkRadius);
        }
    }
}
