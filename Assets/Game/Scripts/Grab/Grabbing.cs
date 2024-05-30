using UnityEngine;
using System.Collections.Generic;

namespace Game.Scripts.Grab
{
    [RequireComponent(typeof(PlayerController))]
    public class Grabbing : MonoBehaviour
    {
        [SerializeField] private Vector3 objectCheckOffset = Vector3.zero;
        [SerializeField] private float grabRange = 2f;
        [SerializeField] private float playerSpeedModifier = 0.5f;
        [SerializeField] private float sphereCastRadius = 0.5f;
        [SerializeField] private Material selectedMaterial;
        [SerializeField] private Material defaultMaterial;

        private IGrabbable _currentGrabbable;
        private PlayerController _playerController;
        private GameObject _currentGrabbableObject;
        private Transform _grabPoint;
        public LayerMask layerMask;
        public static HashSet<Transform> ActiveGrabPoints = new HashSet<Transform>();

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _grabPoint = new GameObject("GrabPoint").transform;
            _grabPoint.SetParent(transform);
            _grabPoint.localPosition = new Vector3(0, 0, 1.4f);
        }

        private void OnGrab()
        {
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

            if (Physics.SphereCast(transform.position + objectCheckOffset, sphereCastRadius, actualForward, out var hit, grabRange, layerMask))
            {
                if (hit.collider.TryGetComponent(out IGrabbable grabbable))
                {
                    _currentGrabbable = grabbable;
                    _currentGrabbableObject = hit.collider.gameObject;
                    _currentGrabbable.OnGrab(this.transform);

                    AdjustPlayerSpeed();
                    IgnoreCollisions(true);

                    var renderers = _currentGrabbableObject.GetComponentsInChildren<MeshRenderer>();
                    foreach (var mRenderer in renderers)
                    {
                        foreach (var rendererMaterial in mRenderer.sharedMaterials)
                        {
                            if (rendererMaterial != defaultMaterial) continue;
                            mRenderer.material = selectedMaterial;
                        }
                    }

                    Debug.Log("Object grabbed: " + _currentGrabbableObject.name);
                }
            }
        }

        private void Release()
        {
            if (_currentGrabbable == null) return;

            _currentGrabbable.OnRelease(this.transform);
            IgnoreCollisions(false);

            var renderers = _currentGrabbableObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var mRenderer in renderers)
            {
                foreach (var rendererMaterial in mRenderer.sharedMaterials)
                {
                    if (rendererMaterial != selectedMaterial) continue;
                    mRenderer.material = defaultMaterial;
                }
            }
            ResetPlayerSpeed();

            Debug.Log("Object released: " + _currentGrabbableObject.name);

            _currentGrabbable = null;
            _currentGrabbableObject = null;
        }

        private void AdjustPlayerSpeed()
        {
            _playerController.speedFactor *= playerSpeedModifier;
        }

        private void ResetPlayerSpeed()
        {
            _playerController.speedFactor /= playerSpeedModifier;
        }

        private void IgnoreCollisions(bool ignore)
        {
            var playerColliders = GetComponentsInChildren<Collider>();
            var objectColliders = _currentGrabbableObject.GetComponentsInChildren<Collider>();

            foreach (var playerCollider in playerColliders)
            {
                foreach (var objectCollider in objectColliders)
                {
                    Physics.IgnoreCollision(playerCollider, objectCollider, ignore);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.blue;
            var startPos = transform.position + objectCheckOffset;
            var actualForward = _playerController.TargetRotation * Vector3.forward;
            Gizmos.DrawLine(startPos, startPos + actualForward * grabRange);
            Gizmos.DrawWireSphere(startPos + actualForward * grabRange, sphereCastRadius);
        }
    }
}
