using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Barking
{
    public class Bark : MonoBehaviour
    {
        public float barkRadius = 5f;
        public AudioClip barkSound; 

        private AudioSource _audioSource; 

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();  
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OnBark();
            }
        }

        private void OnBark()
        {
            Debug.Log("woof woof");
            if (barkSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(barkSound); 
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
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, barkRadius);
        }
    }
}
