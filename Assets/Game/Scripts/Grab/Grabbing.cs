using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Game.Scripts.Grab
{
    [RequireComponent(typeof(PlayerController))]
    public class Grabbing : MonoBehaviour
    {
        [SerializeField] private Vector3 objectCheckOffset = Vector3.zero;
        [SerializeField] private float playerSpeedModifier = 1f;
        [SerializeField] private bool holdToGrab = true;

        [SerializeField] private Material selectedMaterial;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private float sphereRadius = 0.6f;
        [SerializeField] private LayerMask grabLayerMask = ~0;

        [Header("Floor Grab Settings")]
        [SerializeField] private bool groundGrab = true;
        [SerializeField] private LayerMask groundGrabLayerMask;
        [SerializeField] private float groundGrabDistance = 1f;

        private IGrabbable _currentGrabbable;
        private Transform _grabPoint;
        private PlayerController _playerController;
        private GameObject _currentGrabbableObject;

        private PlayerInput _playerInput;
        private InputAction _grabAction;
        private readonly Collider[] _colliders = new Collider[10];

        public bool IsGrabbing { get; private set; } 

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
            // Leave as toggle for Keyboard Input
            if (_currentGrabbable != null)
            {
                if (callbackContext.control.device is Keyboard)
                {
                    Release();
                }
                return;
            }
            
            if (!holdToGrab) return;

            TryGrab();
        }

        private void OnGrabReleased(InputAction.CallbackContext callbackContext)
        {
            if (!holdToGrab || callbackContext.control.device is Keyboard) return;

            if (_currentGrabbable == null) return;

            Release();
        }

        private void TryGrab()
        {
            var startPos = transform.position + objectCheckOffset;

            var size = Physics.OverlapSphereNonAlloc(startPos, sphereRadius, _colliders, grabLayerMask.value, QueryTriggerInteraction.Ignore);
            for (var index = 0; index < size; index++)
            {
                if (FindAndGrab(_colliders[index])) return;
            }
            
            // Fallback to ground grab
            if (!groundGrab) return;

            var groundRay = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(groundRay, out var groundHit, groundGrabDistance, groundGrabLayerMask,
                    QueryTriggerInteraction.Ignore))
            {
                FindAndGrab(groundHit.collider);
            }
        }

        private bool FindAndGrab(Collider otherCollider)
        {
            if (!otherCollider.TryGetComponent(out IGrabbable grabbable)) return false;
            
            _currentGrabbable = grabbable;
            _currentGrabbableObject = otherCollider.gameObject;
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
                    
            IsGrabbing = grabbable.ShouldAffectElastcForce();

            return true;
        }

        private void Release()
        {
            if (_currentGrabbableObject is null) return;

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
            IsGrabbing = false;
        }

        private void AdjustPlayerSpeed()
        {
            _playerController.speedFactor *= playerSpeedModifier;
        }

        private void ResetPlayerSpeed()
        {
            _playerController.speedFactor /= playerSpeedModifier;
        }

        private void OnDestroy()
        {
            _grabAction.performed -= OnGrabPerformed;
            _grabAction.canceled -= OnGrabReleased;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.blue;
            var startPos = transform.position + objectCheckOffset;
            Gizmos.DrawWireSphere(startPos, sphereRadius);

            if (groundGrab)
            {
                Gizmos.DrawRay(transform.position, Vector3.down * groundGrabDistance);
            }
        }
    }
}