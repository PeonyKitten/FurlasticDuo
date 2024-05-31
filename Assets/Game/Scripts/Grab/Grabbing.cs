using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Grab
{
    [RequireComponent(typeof(PlayerController))]
    public class Grabbing : MonoBehaviour
    {
        [SerializeField] private Vector3 objectCheckOffset = Vector3.zero;
        //[SerializeField] private float grabRange = 2f; not  needed as we have sphereradius and object offset
        [SerializeField] private float playerSpeedModifier = 0.5f;
        [SerializeField] private bool holdToGrab = true;

        [SerializeField] private Material selectedMaterial;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private float sphereRadius = 0.6f;
        [SerializeField] private LayerMask layerMask = ~0;

        private IGrabbable _currentGrabbable;
        private Transform _grabPoint;
        private PlayerController _playerController;
        private GameObject _currentGrabbableObject;

        private PlayerInput _playerInput;
        private InputAction _grabAction;



        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _playerInput = GetComponent<PlayerInput>();
            _grabPoint = new GameObject("GrabPoint").transform;
            _grabPoint.SetParent(transform);
            _grabPoint.localPosition = new Vector3(0, 0, 1.4f);
            _grabAction = _playerInput.actions["Grab"];

            _grabAction.performed += OnGrabPerformed;
            _grabAction.canceled += OnGrabReleased;
        }

        private void OnGrab()
        {
            if (holdToGrab) return;

            if (_currentGrabbable == null)
            {
                TryGrab();
            }
            else
            {
                Release();
            }
        }

        private void OnGrabPerformed(InputAction.CallbackContext callbackContext)
        {
            if (!holdToGrab) return;

            if (_currentGrabbable != null) return;

            TryGrab();
        }

        private void OnGrabReleased(InputAction.CallbackContext callbackContext)
        {
            if (!holdToGrab) return;

            if (_currentGrabbable == null) return;

            Release();
        }

        private void TryGrab()
        {
            var startPos = transform.position + objectCheckOffset;

            var colliders = Physics.OverlapSphere(startPos, sphereRadius, layerMask, QueryTriggerInteraction.Ignore);
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