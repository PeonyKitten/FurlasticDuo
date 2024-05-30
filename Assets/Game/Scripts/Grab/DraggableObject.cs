using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Grab
{
    public class DraggableObject : MonoBehaviour, IGrabbable
    {
        public GrabbableType Type => GrabbableType.Draggable;

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

            var player = grabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                var fixedJoint = player.gameObject.AddComponent<FixedJoint>();
                fixedJoint.connectedBody = _rb;
                fixedJoint.breakForce = float.MaxValue;
                fixedJoint.breakTorque = float.MaxValue;
                fixedJoint.enableCollision = false;
                fixedJoint.enablePreprocessing = false;

                Debug.Log("Object grabbed by " + grabPoint.name);
            }
            else
            {
                Debug.LogError("PlayerController not found in parent hierarchy.");
            }
        }

        public void OnRelease(Transform grabPoint)
        {
            var player = grabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                var joint = player.GetComponent<FixedJoint>();
                if (joint != null)
                {
                    Destroy(joint);
                    Debug.Log("Object released by " + grabPoint.name);
                }
            }

            _grabPoints.Remove(grabPoint);
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
