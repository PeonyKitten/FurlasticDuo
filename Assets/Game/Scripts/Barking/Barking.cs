using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Barking : MonoBehaviour
    {
        public float barkRadius = 5f;
        public AudioClip barkSound; 

        private AudioSource audioSource; 

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();  
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
            if (barkSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(barkSound); 
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

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, barkRadius);
        }
    }
}
