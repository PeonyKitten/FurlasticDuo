using UnityEngine;

namespace Game.Scripts.Grab
{
    public class StaticObject : MonoBehaviour, IGrabbable
    {
        private Transform _grabPoint;
        private FixedJoint _fixedJoint;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
        }

        public void OnGrab(Transform grabPoint)
        {
            _grabPoint = grabPoint;
            _fixedJoint = gameObject.AddComponent<FixedJoint>();
            _fixedJoint.connectedBody = grabPoint.GetComponentInParent<Rigidbody>();
            _fixedJoint.breakForce = float.MaxValue;
            _fixedJoint.breakTorque = float.MaxValue;
            _fixedJoint.enableCollision = false;
            _fixedJoint.enablePreprocessing = false;

            Debug.Log("Static object grabbed");
        }

        public void OnRelease(Transform grabPoint)
        {
            if (_grabPoint != grabPoint) return;
            
            _grabPoint = null;

            if (_fixedJoint)
            {
                Destroy(_fixedJoint);
            }

            Debug.Log("Static object released");
        }

        private void FixedUpdate()
        {
            // TODO: needs rework
            
            // Ensure the object remains immovable
            if (_rb.isKinematic == false)
            {
                _rb.isKinematic = true;
            }
        }
    }
}
