using UnityEngine;

public class PickableObject : MonoBehaviour, IGrabbable
{
    private Rigidbody rb;
    private Transform grabPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnGrab(Transform grabPoint)
    {
        this.grabPoint = grabPoint;
        rb.useGravity = false;
    }

    public void OnRelease(Transform grabPoint)
    {
        this.grabPoint = null;
        rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        if (grabPoint != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, grabPoint.position, Time.deltaTime * lerpSpeed);
            rb.MovePosition(newPosition);
        }
    }
}
