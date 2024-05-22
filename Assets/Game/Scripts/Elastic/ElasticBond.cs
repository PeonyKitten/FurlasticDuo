using UnityEngine;

public class ElasticBond : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public float maxDistance = 5f; 
    public float elasticity = 5f; 
    public float maxYankForce = 20f; 

    private Rigidbody rb1;
    private Rigidbody rb2;

    void Start()
    {
        rb1 = player1.GetComponent<Rigidbody>();
        rb2 = player2.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 direction = player2.position - player1.position;
        float distance = direction.magnitude;

        if (distance > maxDistance)
        {
            float yankForce = Mathf.Min((distance - maxDistance) * elasticity, maxYankForce);
            Vector3 force = direction.normalized * yankForce;

            if (rb1.velocity.magnitude < 0.1f)
            {
                rb1.AddForce(force, ForceMode.Impulse);
            }
            if (rb2.velocity.magnitude < 0.1f)
            {
                rb2.AddForce(-force, ForceMode.Impulse);
            }
        }
    }
}
