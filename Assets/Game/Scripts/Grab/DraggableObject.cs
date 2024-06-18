using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Grab
{
    public class DraggableObject : MonoBehaviour, IGrabbable
    {
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
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
        }
    }
}