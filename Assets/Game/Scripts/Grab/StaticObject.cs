using UnityEngine;

namespace Game.Scripts.Grab
{
    public class StaticObject : MonoBehaviour, IGrabbable
    {
        public GrabbableType Type => GrabbableType.Static;

        private Transform _grabPoint;
        private Rigidbody _rb;
        private GameObject _fixedJointObject;
        private ConfigurableJoint _configurableJoint;

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
            _fixedJointObject = new GameObject("FixedJointObject");
            _fixedJointObject.transform.position = playerGrabPoint.position;
            _fixedJointObject.transform.SetParent(transform);

            var fixedJointRb = _fixedJointObject.AddComponent<Rigidbody>();
            fixedJointRb.isKinematic = true;

            AttachConfigurableJoint(playerGrabPoint, fixedJointRb);

            Debug.Log("Static object grabbed at " + playerGrabPoint.position);
        }

        private void AttachConfigurableJoint(Transform playerGrabPoint, Rigidbody connectedBody)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                _configurableJoint = player.gameObject.AddComponent<ConfigurableJoint>();
                _configurableJoint.connectedBody = connectedBody;

                _configurableJoint.xMotion = ConfigurableJointMotion.Locked;
                _configurableJoint.yMotion = ConfigurableJointMotion.Locked;
                _configurableJoint.zMotion = ConfigurableJointMotion.Locked;
                _configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
                _configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
                _configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;

                Debug.Log("Configurable joint created on player connected to fixed joint object.");
            }
            else
            {
                Debug.LogError("PlayerController not found in parent hierarchy.");
            }
        }

        public void OnRelease(Transform playerGrabPoint)
        {
            if (_configurableJoint != null)
            {
                Destroy(_configurableJoint);
            }

            if (_fixedJointObject != null)
            {
                Destroy(_fixedJointObject);
            }

            Debug.Log("Static object released");
        }
    }
}
