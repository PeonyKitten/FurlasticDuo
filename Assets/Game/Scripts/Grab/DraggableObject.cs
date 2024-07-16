using System;
using System.Collections.Generic;
using Game.Scripts.Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Grab
{
    public class DraggableObject : MonoBehaviour, IGrabbable
    {
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material selectedMaterial;
        [SerializeField] private bool requireBothPlayersToMove = false;
        [SerializeField] private bool pushIfNotGrabbed = true;

        public UnityEvent onGrab;
        public UnityEvent onGrabRelease;

        private Renderer[] _renderers;
        private int _grabCount = 0;
        private Rigidbody _rb;
        private static readonly int AnimHashIsDragging = Animator.StringToHash("IsDragging");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _renderers = GetComponentsInChildren<Renderer>();

            _rb.isKinematic = !pushIfNotGrabbed;
        }

        public void OnGrab(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                AddJoint(player, playerGrabPoint);
            }

            _grabCount++;
            UpdateMaterial();

            if (requireBothPlayersToMove)
            {
                _rb.isKinematic = _grabCount < 2;
            }
            else
            {
                _rb.isKinematic = false;
            }

            onGrab?.Invoke();
        }

        public void OnRelease(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                RemoveJoint(player);
            }

            _grabCount--;
            UpdateMaterial();

            if (requireBothPlayersToMove)
            {
                _rb.isKinematic = _grabCount < 2;
            }

            if (_grabCount <= 0)
            {
                _rb.isKinematic = !pushIfNotGrabbed;
            }

            onGrabRelease?.Invoke();
        }

        //TODO: maybe change?   
        public void ReleaseAll()
        {
            var allPlayers = FindObjectsOfType<PlayerController>();
            foreach (var player in allPlayers)
            {
                var joint = player.GetComponent<FixedJoint>();
                if (joint != null && joint.connectedBody == _rb)
                {
                    Destroy(joint);
                }
            }
            _grabCount = 0;
            UpdateMaterial();
            _rb.isKinematic = !pushIfNotGrabbed;
        }

        private void AddJoint(PlayerController player, Transform playerGrabPoint)
        {
            var hitPoint = playerGrabPoint.position;

            var fixedJoint = player.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = _rb;
            fixedJoint.breakForce = float.MaxValue;
            fixedJoint.breakTorque = float.MaxValue;
            fixedJoint.enableCollision = true;
            fixedJoint.enablePreprocessing = false;

            fixedJoint.anchor = playerGrabPoint.InverseTransformPoint(hitPoint);
        }

        private void RemoveJoint(PlayerController player)
        {
            var joint = player.GetComponent<FixedJoint>();
            if (joint != null)
            {
                Destroy(joint);
            }
        }

        private void UpdateMaterial()
        {
            var material = _grabCount > 0 ? selectedMaterial : normalMaterial;
            foreach (var renderer in _renderers)
            {
                foreach (var rendererMaterial in renderer.sharedMaterials)
                {
                    if (rendererMaterial == normalMaterial || rendererMaterial == selectedMaterial)
                    {
                        renderer.material = material;
                    }
                }
            }
        }
    }
}