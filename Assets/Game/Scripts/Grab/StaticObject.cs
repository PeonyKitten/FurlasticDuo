using UnityEngine;

public class StaticObject : MonoBehaviour, IGrabbable
{
    private Transform grabPoint;
    private FixedJoint fixedJoint;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void OnGrab(Transform grabPoint)
    {
        this.grabPoint = grabPoint;
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = grabPoint.GetComponentInParent<Rigidbody>();
        fixedJoint.breakForce = float.MaxValue;
        fixedJoint.breakTorque = float.MaxValue;
        fixedJoint.enableCollision = false;
        fixedJoint.enablePreprocessing = false;

        Debug.Log("Static object grabbed");
    }

    public void OnRelease(Transform grabPoint)
    {
        if (this.grabPoint == grabPoint)
        {
            this.grabPoint = null;

            if (fixedJoint != null)
            {
                Destroy(fixedJoint);
            }

            Debug.Log("Static object released");
        }
    }

    private void FixedUpdate()
    {
        // Ensure the object remains immovable
        if (rb.isKinematic == false)
        {
            rb.isKinematic = true;
        }
    }
}
