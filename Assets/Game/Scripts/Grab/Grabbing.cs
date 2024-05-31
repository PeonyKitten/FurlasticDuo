using UnityEngine;
using System.Collections.Generic;

namespace Game.Scripts.Grab
{
    [RequireComponent(typeof(PlayerController))]
    public class Grabbing : MonoBehaviour
    {
        [SerializeField] private Vector3 objectCheckOffset = Vector3.zero;
        // [SerializeField] private float grabRange = 2f; // dont need at the moment as we have sphereradius to play with
        [SerializeField] private float playerSpeedModifier = 0.5f;
        [SerializeField] private float sphereRadius = 2f;
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
            var startPos = transform.position + objectCheckOffset;

            var colliders = Physics.OverlapSphere(startPos, sphereRadius, layerMask);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IGrabbable grabbable))
                {
                    _currentGrabbable = grabbable;
                    _currentGrabbableObject = collider.gameObject;
                    _currentGrabbable.OnGrab(_grabPoint);

                    AdjustPlayerSpeed();

                    var renderers = _currentGrabbableObject.GetComponentsInChildren<MeshRenderer>();
                    foreach (var mRenderer in renderers)
                    {
                        foreach (var rendererMaterial in mRenderer.sharedMaterials)
                        {
                            if (rendererMaterial != defaultMaterial) continue;
                            mRenderer.material = selectedMaterial;
                        }
                    }
                    return;
                }
            }
        }

        private void Release()
        {
            if (_currentGrabbable == null) return;

            _currentGrabbable.OnRelease(_grabPoint);

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

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.blue;
            var startPos = transform.position + objectCheckOffset;

            Gizmos.DrawWireSphere(startPos, sphereRadius);
        }
    }
}