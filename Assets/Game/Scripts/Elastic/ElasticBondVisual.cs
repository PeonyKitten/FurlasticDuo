using UnityEngine;

public class ElasticBondVisual : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
    }

    void Update()
    {
        lineRenderer.SetPosition(0, player1.position);
        lineRenderer.SetPosition(1, player2.position);
    }
}
