using UnityEngine;

namespace Game.Scripts.Grab
{
    public class StaticObject : MonoBehaviour, IGrabbable
    {
        public GrabbableType Type => GrabbableType.Static;

        private Transform _grabPoint;
        private Rigidbody _rb;

        private void Awake()
        {
            if (!TryGetComponent<Rigidbody>(out _rb))
            {
                _rb = gameObject.AddComponent<Rigidbody>();
            }

            _rb.isKinematic = true;
        }

        public void OnGrab(Transform grabPoint)
        {
            _grabPoint = grabPoint;
            Debug.Log("Static object grabbed");
        }

        public void OnRelease(Transform grabPoint)
        {
            if (_grabPoint != grabPoint) return;

            _grabPoint = null;

            Debug.Log("Static object released");
        }
    }
}
