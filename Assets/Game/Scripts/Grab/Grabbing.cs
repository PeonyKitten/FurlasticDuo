using UnityEngine;

namespace Game.Scripts.Grab
{
    [RequireComponent(typeof(PlayerController))]
    public class Grabbing : MonoBehaviour
    {
        [SerializeField] private Vector3 objectCheckOffset = Vector3.zero;
        [SerializeField] private float grabRange = 2f;
        
        private IGrabbable _currentGrabbable;
        private Transform _grabPoint;
        private PlayerController _playerController;
        private GameObject _currentGrabbableObject;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _grabPoint = new GameObject("GrabPoint").transform;
            _grabPoint.SetParent(transform);
            _grabPoint.localPosition = new Vector3(0, 0, 1.4f);
        }

        private void OnGrab()
        {
            Debug.Log("Grab action performed");

            if (_currentGrabbable == null)
            {
                TryGrab();
            }
            else
            {
                Release();
            }
        }

        private void TryGrab()
        {
            var actualForward = _playerController.TargetRotation * Vector3.forward;

            if (Physics.Raycast(transform.position + objectCheckOffset, actualForward, out var hit, grabRange))
            {
                if (hit.collider.TryGetComponent(out IGrabbable grabbable))
                {
                    _currentGrabbable = grabbable;
                    _currentGrabbableObject = hit.collider.gameObject;
                    _currentGrabbable.OnGrab(_grabPoint);

                    AdjustPlayerSpeed(0.5f);

                    Debug.Log("Object grabbed: " + _currentGrabbableObject.name);
                }
            }
        }

        private void Release()
        {
            if (_currentGrabbable == null) return;
            
            _currentGrabbable.OnRelease(_grabPoint);

            ResetPlayerSpeed();

            Debug.Log("Object released: " + _currentGrabbableObject.name);
            
            _currentGrabbable = null;
            _currentGrabbableObject = null;
        }

        private void AdjustPlayerSpeed(float factor)
        {
            _playerController.speedFactor = factor;
        }

        private void ResetPlayerSpeed()
        {
            _playerController.speedFactor = 1.0f;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.blue;
            var startPos = transform.position + objectCheckOffset;
            var actualForward = _playerController.TargetRotation * Vector3.forward;
            Gizmos.DrawLine(startPos, startPos + actualForward);
        }
    }
}
