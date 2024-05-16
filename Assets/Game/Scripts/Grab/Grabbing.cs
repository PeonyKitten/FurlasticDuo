using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts
{
    [RequireComponent(typeof(PlayerController))]
    public class Grabbing : MonoBehaviour
    {
        [SerializeField] private Vector3 objectCheckOffset = Vector3.zero;

        public float grabRange = 2f;
        private IGrabbable currentGrabbable;
        private Transform grabPoint;
        private PlayerController playerController;
        private Collider playerCollider;
        private GameObject currentGrabbableObject;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerCollider = GetComponent<Collider>();
            grabPoint = new GameObject("GrabPoint").transform;
            grabPoint.SetParent(transform);
            grabPoint.localPosition = new Vector3(0, 0, 1.4f);
        }

        private void OnGrab()
        {
            Debug.Log("Grab action performed");

            if (currentGrabbable == null)
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
            var actualForward = playerController.TargetRotation * Vector3.forward;

            if (Physics.Raycast(transform.position + objectCheckOffset, actualForward, out var hit, grabRange))
            {
                IGrabbable grabbable = hit.collider.GetComponent<IGrabbable>();
                if (grabbable != null)
                {
                    currentGrabbable = grabbable;
                    currentGrabbableObject = hit.collider.gameObject;
                    currentGrabbable.OnGrab(grabPoint);

                    AdjustPlayerSpeed(0.5f);

                    Debug.Log("Object grabbed: " + currentGrabbableObject.name);
                }
            }
        }

        private void Release()
        {
            if (currentGrabbable != null)
            {
                currentGrabbable.OnRelease(grabPoint);

                ResetPlayerSpeed();

                Debug.Log("Object released: " + currentGrabbableObject.name);
                currentGrabbable = null;
                currentGrabbableObject = null;
            }
        }

        private void AdjustPlayerSpeed(float factor)
        {
            playerController.speedFactor = factor;
        }

        private void ResetPlayerSpeed()
        {
            playerController.speedFactor = 1.0f;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.blue;
            var startPos = transform.position + objectCheckOffset;
            var actualForward = playerController.TargetRotation * Vector3.forward;
            Gizmos.DrawLine(startPos, startPos + actualForward);
        }
    }
}
