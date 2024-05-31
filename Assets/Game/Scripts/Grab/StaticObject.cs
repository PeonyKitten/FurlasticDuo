using UnityEngine;
using System.Collections.Generic;

namespace Game.Scripts.Grab
{
    public class StaticObject : MonoBehaviour, IGrabbable
    {
        private Rigidbody _rb;
        private GameObject _configurableJointObject;
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

            _configurableJointObject = new GameObject("ConfigurableJointObject");
            _configurableJointObject.transform.position = playerGrabPoint.position;
            _configurableJointObject.transform.SetParent(transform);

            var jointRb = _configurableJointObject.AddComponent<Rigidbody>();
            jointRb.isKinematic = true;

            AttachConfigurableJoint(player, jointRb);

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
                Destroy(joint);
                playerJoints.Remove(player);
            }

            if (_configurableJointObject != null)
            {
                Destroy(_configurableJointObject);
                _configurableJointObject = null;
            }
        }

        public void ReleaseAll()
        {
            foreach (var joint in playerJoints.Values)
            {
                Destroy(joint);
            }
            playerJoints.Clear();

            if (_configurableJointObject != null)
            {
                Destroy(_configurableJointObject);
                _configurableJointObject = null;
            }
        }
    }
}
