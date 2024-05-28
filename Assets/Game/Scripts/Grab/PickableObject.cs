using UnityEngine;

namespace Game.Scripts.Grab
{
    public class PickableObject : MonoBehaviour, IGrabbable
    {
        public GrabbableType Type => GrabbableType.Pickable;

        [SerializeField] private float lerpSpeed = 10f;
        private Rigidbody _rb;
        private Transform _grabPoint;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void OnGrab(Transform grabPoint)
        {
            _grabPoint = grabPoint;
            _rb.useGravity = false;
        }

        public void OnRelease(Transform grabPoint)
        {
            this._grabPoint = null;
            _rb.useGravity = true;
        }

        private void FixedUpdate()
        {
            if (!_grabPoint) return;

            var newPosition = Vector3.Lerp(transform.position, _grabPoint.position, Time.deltaTime * lerpSpeed);
            _rb.MovePosition(newPosition);
        }
    }
}
