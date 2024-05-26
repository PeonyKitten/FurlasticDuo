using UnityEngine;

namespace Game.Scripts.Elastic
{
    public class ElasticVisual : MonoBehaviour
    {
        public Transform player1;
        public Transform player2;
        [SerializeField] private float maxDistance = 5f;

        private LineRenderer lineRenderer;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        void Update()
        {
            if (player1 is null || player2 is null) return;

            lineRenderer.SetPosition(0, player1.position);
            lineRenderer.SetPosition(1, player2.position);

            Vector3 direction = player2.position - player1.position;
            float distance = direction.magnitude;

            if (distance <= maxDistance * 0.5f)
            {
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
            }
            else if (distance <= maxDistance * 0.8f)
            {
                lineRenderer.startColor = Color.yellow;
                lineRenderer.endColor = Color.yellow;
            }
            else
            {
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
            }
        }
    }
}
