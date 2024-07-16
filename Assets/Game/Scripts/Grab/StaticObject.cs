using UnityEngine;
using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine.Events;

namespace Game.Scripts.Grab
{
    public class StaticObject : MonoBehaviour, IGrabbable
    {
        public UnityEvent onGrab;
        public UnityEvent onGrabRelease;

        private static readonly Dictionary<GameObject, FixedJoint> PlayerJoints = new();

        private void Awake()
        {
            if (!TryGetComponent(out Rigidbody rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;
        }

        public void OnGrab(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>().gameObject;
            if (PlayerJoints.ContainsKey(player))
            {
                return;
            }

            var connectionBody = CreateConnectionBody(playerGrabPoint.position);
            AttachJointToPlayer(player, connectionBody);
            onGrab?.Invoke();
        }

        private Rigidbody CreateConnectionBody(Vector3 position)
        {
            var configurableJointObject = new GameObject("Connection Point");
            configurableJointObject.transform.position = position;
            configurableJointObject.transform.SetParent(transform);

            var jointRb = configurableJointObject.AddComponent<Rigidbody>();
            jointRb.isKinematic = true;

            return jointRb;
        }

        private static void AttachJointToPlayer(GameObject player, Rigidbody connectedBody)
        {
            var playerController = player.GetComponent<PlayerController>();
            if (playerController == null) return;
            
            var fixedJoint = player.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = connectedBody;

            fixedJoint.breakForce = float.MaxValue;
            fixedJoint.breakTorque = float.MaxValue;
            fixedJoint.enableCollision = true;
            fixedJoint.enablePreprocessing = false;

            PlayerJoints.Add(player, fixedJoint);
        }

        public void OnRelease(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>().gameObject;

            // Make sure we found a matching joint
            if (PlayerJoints.Remove(player, out var joint))
            {
                DestroyJointAndConnectedBody(joint);
            }

            onGrabRelease?.Invoke();
        }

        private static void DestroyJointAndConnectedBody(Joint joint)
        {
            var connectedBody = joint.connectedBody.gameObject;

            Destroy(joint);
            Destroy(connectedBody);
        }

        public void ReleaseAll()
        {
            foreach (var joint in PlayerJoints.Values)
            {
                DestroyJointAndConnectedBody(joint);
            }
            PlayerJoints.Clear();
        }
    }
}
