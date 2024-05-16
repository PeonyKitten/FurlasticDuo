using System.Collections.Generic;
using UnityEngine;

public class DraggableObject : MonoBehaviour, IGrabbable
{
    private Rigidbody rb;
    private List<Transform> grabPoints = new List<Transform>();
    private List<FixedJoint> fixedJoints = new List<FixedJoint>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnGrab(Transform grabPoint)
    {
        grabPoints.Add(grabPoint);

        FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = grabPoint.GetComponentInParent<Rigidbody>();
        fixedJoint.breakForce = float.MaxValue;
        fixedJoint.breakTorque = float.MaxValue;
        fixedJoint.enableCollision = false;
        fixedJoint.enablePreprocessing = false;

        fixedJoints.Add(fixedJoint);

        rb.isKinematic = false;
    }

    public void OnRelease(Transform grabPoint)
    {
        int index = grabPoints.IndexOf(grabPoint);
        if (index != -1)
        {
            Destroy(fixedJoints[index]);
            fixedJoints.RemoveAt(index);
            grabPoints.RemoveAt(index);
        }
    }

    private void FixedUpdate()
    {
        if (grabPoints.Count > 0)
        {
            Vector3 combinedForce = Vector3.zero;

            foreach (Transform grabPoint in grabPoints)
            {
                Vector3 positionDifference = grabPoint.position - transform.position;
                combinedForce += positionDifference * 10f;
            }

            rb.AddForce(combinedForce);
        }
    }
}
