using UnityEngine;
using System.Collections.Generic;
using Game.Scripts.Player;

namespace Game.Scripts.Grab
{
    public class StaticObject : MonoBehaviour, IGrabbable
    {
        private Rigidbody _rb;
        private static Dictionary<GameObject, ConfigurableJoint> playerJoints = new Dictionary<GameObject, ConfigurableJoint>();

        private void Awake()
        {
            if (!TryGetComponent<Rigidbody>(out _rb))
            {
                _rb = gameObject.AddComponent<Rigidbody>();
            }
            _rb.isKinematic = true;
        }

        public void OnGrab(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>().gameObject;
            if (playerJoints.ContainsKey(player))
            {
                return;
            }

            var configurableJointObject = CreateConfigurableJointObject(playerGrabPoint.position);
            AttachConfigurableJoint(player, configurableJointObject.GetComponent<Rigidbody>());
        }

        private GameObject CreateConfigurableJointObject(Vector3 position)
        {
            var configurableJointObject = new GameObject("ConfigurableJointObject");
            configurableJointObject.transform.position = position;
            configurableJointObject.transform.SetParent(transform);

            var jointRb = configurableJointObject.AddComponent<Rigidbody>();
            jointRb.isKinematic = true;

            return configurableJointObject;
        }

        private void AttachConfigurableJoint(GameObject player, Rigidbody connectedBody)
        {
            var playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                var configurableJoint = player.AddComponent<ConfigurableJoint>();
                configurableJoint.connectedBody = connectedBody;

                configurableJoint.xMotion = ConfigurableJointMotion.Locked;
                configurableJoint.yMotion = ConfigurableJointMotion.Locked;
                configurableJoint.zMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;

                playerJoints[player] = configurableJoint;
            }
        }

        public void OnRelease(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>().gameObject;
            if (playerJoints.TryGetValue(player, out var joint))
            {
                DestroyJointAndConnectedBody(player, joint);
            }
        }

        private void DestroyJointAndConnectedBody(GameObject player, ConfigurableJoint joint)
        {
            var connectedBody = joint.connectedBody.gameObject;

            Destroy(joint);
            playerJoints.Remove(player);

            Destroy(connectedBody);
        }

        public void ReleaseAll()
        {
            foreach (var kvp in playerJoints)
            {
                var joint = kvp.Value;
                var connectedBody = joint.connectedBody.gameObject;

                Destroy(joint);
                Destroy(connectedBody);
            }
            playerJoints.Clear();
        }
    }
}
