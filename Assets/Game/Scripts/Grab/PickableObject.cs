using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace FD.Grab
{
    public class PickableObject : MonoBehaviour, IGrabbable
    {
        [SerializeField] private float lerpSpeed = 10f;

        public UnityEvent onGrab;
        public UnityEvent onGrabRelease;

        private Rigidbody _rb;
        private Transform _grabPoint;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void OnGrab(Transform grabPoint)
        {
            // We've stolen the Pickable Object :P
            // if (_grabPoint is not null && _grabPoint != grabPoint)
            // {
            //     
            // }
            _grabPoint = grabPoint;
            _rb.useGravity = false;

            onGrab?.Invoke();
        }

        public void OnRelease(Transform grabPoint)
        {
            // We're releasing a different Grab than our current one
            if (grabPoint != _grabPoint) return;
            
            _grabPoint = null;
            _rb.useGravity = true;

            onGrabRelease?.Invoke();
        }

        public void ReleaseAll()
        {
            _grabPoint = null;
            _rb.useGravity = true;
        }

        private void FixedUpdate()
        {
            if (!_grabPoint) return;
            var newPosition = Vector3.Lerp(transform.position, _grabPoint.position, Time.fixedDeltaTime * lerpSpeed);
            _rb.MovePosition(newPosition);
        }

        bool IGrabbable.ShouldAffectElasticForce()
        {
            return false;
        }
    }
}
