using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Grab
{
    public class DraggableObject : MonoBehaviour, IGrabbable
    {
        public GrabbableType Type => GrabbableType.Draggable;

        private Rigidbody _rb;
        private readonly List<Transform> _grabPoints = new List<Transform>();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            foreach (Transform child in transform)
            {
                if (child.CompareTag("GrabPoint")) 
                {
                    _grabPoints.Add(child);
                }
            }
            if (_grabPoints.Count == 0)
            {
                Debug.LogError("No grab points found on DraggableObject.");
            }
        }

        public Transform GetClosestGrabPoint(Vector3 position)
        {
            Transform closestPoint = null;
            float closestDistance = float.MaxValue;

            foreach (var grabPoint in _grabPoints)
            {
                float distance = Vector3.Distance(position, grabPoint.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = grabPoint;
                }
            }

            return closestPoint;
        }

        public void OnGrab(Transform playerGrabPoint)
        {
            Transform closestGrabPoint = GetClosestGrabPoint(playerGrabPoint.position);
            if (closestGrabPoint != null && !Grabbing.ActiveGrabPoints.Contains(closestGrabPoint))
            {
                Grabbing.ActiveGrabPoints.Add(closestGrabPoint);
                AttachConfigurableJoint(playerGrabPoint, closestGrabPoint);
                MovePlayerToGrabPoint(playerGrabPoint, closestGrabPoint.position);
                Debug.Log($"Object grabbed by {playerGrabPoint.name} at {playerGrabPoint.position}. Closest grab point: {closestGrabPoint.name} at {closestGrabPoint.position}");
            }
            else
            {
                Debug.LogWarning($"Grab point {closestGrabPoint?.name} is already in use or no valid grab point found.");
            }
        }

        private void AttachConfigurableJoint(Transform playerGrabPoint, Transform objectGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                var configurableJoint = player.gameObject.AddComponent<ConfigurableJoint>();
                configurableJoint.connectedBody = _rb;
                configurableJoint.anchor = playerGrabPoint.InverseTransformPoint(objectGrabPoint.position);
                configurableJoint.autoConfigureConnectedAnchor = false;
                configurableJoint.connectedAnchor = objectGrabPoint.localPosition;

                configurableJoint.xMotion = ConfigurableJointMotion.Locked;
                configurableJoint.yMotion = ConfigurableJointMotion.Locked;
                configurableJoint.zMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;

                Debug.Log($"Configurable joint created at: {objectGrabPoint.position} (Grab point: {objectGrabPoint.name})");
            }
            else
            {
                Debug.LogError("PlayerController not found in parent hierarchy.");
            }
        }

        private void MovePlayerToGrabPoint(Transform player, Vector3 grabPointPosition)
        { 
            player.position = Vector3.Lerp(player.position, grabPointPosition, Time.deltaTime * 10f);
        }

        public void OnRelease(Transform playerGrabPoint)
        {
            var player = playerGrabPoint.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                var joint = player.GetComponent<ConfigurableJoint>();
                if (joint != null)
                {
                    Destroy(joint);
                    Debug.Log("Object released by " + playerGrabPoint.name);
                }
            }

            Transform closestGrabPoint = GetClosestGrabPoint(playerGrabPoint.position);
            if (closestGrabPoint != null)
            {
                Grabbing.ActiveGrabPoints.Remove(closestGrabPoint);
            }
        }
    }
}
