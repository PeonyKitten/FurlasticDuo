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
            
            Debug.Log("woof woof");
            if (barkSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(barkSound); 
            }

            if (barkEffect)
            {
                Instantiate(barkEffect);
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, barkRadius);
            foreach (var hitCollider in hitColliders)
            {
                var npcReaction = hitCollider.GetComponent<INPCReaction>();
                if (npcReaction != null)
                {
                    npcReaction.ReactToBark(transform.position);
                }
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
