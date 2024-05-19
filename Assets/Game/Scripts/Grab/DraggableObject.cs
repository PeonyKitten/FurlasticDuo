using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Grab
{
    public class DraggableObject : MonoBehaviour, IGrabbable
    {
        private Rigidbody _rb;
        private readonly List<Transform> _grabPoints = new List<Transform>();
        private readonly List<FixedJoint> _fixedJoints = new List<FixedJoint>();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void OnGrab(Transform grabPoint)
        {
            _grabPoints.Add(grabPoint);

            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = grabPoint.GetComponentInParent<Rigidbody>();
            fixedJoint.breakForce = float.MaxValue;
            fixedJoint.breakTorque = float.MaxValue;
            fixedJoint.enableCollision = false;
            fixedJoint.enablePreprocessing = false;

            _fixedJoints.Add(fixedJoint);

            _rb.isKinematic = false;
        }

        public void OnRelease(Transform grabPoint)
        {
            int index = _grabPoints.IndexOf(grabPoint);
            if (index != -1)
            {
                Destroy(_fixedJoints[index]);
                _fixedJoints.RemoveAt(index);
                _grabPoints.RemoveAt(index);
            }
        }

        private void FixedUpdate()
        {
            if (_grabPoints.Count > 0)
            {
                Vector3 combinedForce = Vector3.zero;

                foreach (Transform grabPoint in _grabPoints)
                {
                    Vector3 positionDifference = grabPoint.position - transform.position;
                    combinedForce += positionDifference * 10f;
                }

                _rb.AddForce(combinedForce);
            }
        }
    }
}
