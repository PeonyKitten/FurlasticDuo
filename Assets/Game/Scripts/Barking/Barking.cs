using FD.Player;
using UnityEngine;
using FMODUnity; // Add this line to import FMOD Unity integration

namespace FD.Barking
{
    public class Bark : MonoBehaviour
    {
        public float barkRadius = 5f;
        [SerializeField] private float barkDelay = 0.5f;
        [SerializeField] private GameObject barkEffect;
        [SerializeField] private Transform barkSpawn;
        [SerializeField] private EventReference barkEvent; // Add this line for the FMOD event

        private PlayerController _controller;
        private float _barkTimer = 0f;
        private static readonly int AnimHashBark = Animator.StringToHash("Bark");

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            barkSpawn ??= transform;
        }

        private void OnBark()
        {
            _controller.animator?.ResetTrigger(AnimHashBark);
            if (_barkTimer > 0) return;

            // Play the FMOD event
            RuntimeManager.PlayOneShot(barkEvent);

            _controller.animator?.SetTrigger(AnimHashBark);
            if (barkEffect)
            {
                var maybeForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
                var rotation = Quaternion.LookRotation(maybeForward);
                var effect = Instantiate(barkEffect, barkSpawn.position, rotation);
                effect.transform.localScale = Vector3.one * barkRadius * 2;
            }

            Collider[] hitColliders = Physics.OverlapSphere(barkSpawn.position, barkRadius);
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