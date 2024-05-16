using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Barking : MonoBehaviour
    {
        public float barkRadius = 5f;
        // public GameObject barkEffectPrefab;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))  // Assuming 'B' is the bark key
            {
                OnBark();
            }
        }
        private void OnBark()
        {
            Debug.Log("woof woof");
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

        //void ShowBarkEffect()
        //{
        //    if (barkEffectPrefab != null)
        //    {
        //        GameObject effect = Instantiate(barkEffectPrefab, transform.position, Quaternion.identity);
        //        Destroy(effect, 1.5f);
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Make sure Bark prefab is assigned in the inspector");
        //    }
        //}

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, barkRadius);
        }
    }
}
