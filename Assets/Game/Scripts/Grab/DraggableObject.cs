using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Grab
{
    public class DraggableObject : MonoBehaviour, IGrabbable
    {
        public GrabbableType Type => GrabbableType.Draggable;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void OnGrab(Transform playerGrabPoint)
        {
<<<<<<< HEAD
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

=======
            _grabPoints.Add(grabPoint);

            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = grabPoint.GetComponentInParent<Rigidbody>();
            fixedJoint.breakForce = float.MaxValue;
            fixedJoint.breakTorque = float.MaxValue;
            fixedJoint.enableCollision = false;
            fixedJoint.enablePreprocessing = false;

            _fixedJoints.Add(fixedJoint);
            _rb.isKinematic = false;

            Debug.Log("Object grabbed by " + grabPoint.name);
        }

        public void OnRelease(Transform grabPoint)
        {
            int index = _grabPoints.IndexOf(grabPoint);
            if (index != -1)
            {
                Destroy(_fixedJoints[index]);
                _fixedJoints.RemoveAt(index);
                _grabPoints.RemoveAt(index);

                Debug.Log("Object released by " + grabPoint.name);
>>>>>>> 559594f (changed grabbing for objects, fixed some bugs)
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
<<<<<<< HEAD
=======
                _rb.AddForce(combinedForce);
>>>>>>> 559594f (changed grabbing for objects, fixed some bugs)
            }
        }
    }
}
