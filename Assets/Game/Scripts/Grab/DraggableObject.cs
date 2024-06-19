using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Grab
{
    public class DraggableObject : MonoBehaviour, IGrabbable
    {
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material selectedMaterial;
        [SerializeField] private bool requireBothPlayersToMove = false;

        private Renderer[] _renderers;
        private int _grabCount = 0;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _renderers = GetComponentsInChildren<Renderer>();
        }

        public void OnGrab(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                var hitPoint = playerGrabPoint.position;
                    
                var fixedJoint = player.gameObject.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = _rb;
                fixedJoint.breakForce = float.MaxValue;
                fixedJoint.breakTorque = float.MaxValue;
                fixedJoint.enableCollision = false;
                fixedJoint.enablePreprocessing = false;

                fixedJoint.anchor = playerGrabPoint.InverseTransformPoint(hitPoint);
            }

            _grabCount++;
            UpdateMaterial();

            if (requireBothPlayersToMove)
            {
                _rb.isKinematic = _grabCount < 2;
            }
        }

        public void OnRelease(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                var joint = player.GetComponent<FixedJoint>();
                if (joint != null)
                {
                    Destroy(joint);
                }
            }

            _grabCount--;
            UpdateMaterial();

            if (requireBothPlayersToMove)
            {
                _rb.isKinematic = _grabCount < 2;
            }
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
            _rb.isKinematic = true;
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