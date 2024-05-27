using UnityEngine;

namespace Game.Scripts.Elastic
{
    [RequireComponent(typeof(ElasticForce), typeof(LineRenderer))]
    public class ElasticVisual : MonoBehaviour
    {
        [SerializeField] private Material ghostMaterial;
        [SerializeField] private float lineWidth = 0.1f;
        [SerializeField] private bool useDebugColors = true;

        private ElasticForce _elasticForce;
        private LineRenderer _lineRenderer;

        private void Start()
        {
            _elasticForce = GetComponent<ElasticForce>();
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = lineWidth;
            _lineRenderer.endWidth = lineWidth;

            if (ghostMaterial)
            {
                _lineRenderer.material = ghostMaterial;
            }
        }

        private void Update()
        {
            var player1 = _elasticForce.Player1;
            var player2 = _elasticForce.Player2;

            if (player1 is null || player2 is null) return;

            _lineRenderer.SetPosition(0, player1.position);
            _lineRenderer.SetPosition(1, player2.position);

            if (!useDebugColors) return;

            Vector3 direction = player2.position - player1.position;
            float distance = direction.magnitude;

            if (distance <= _elasticForce.ForceAppliedDistance)
            {
                _lineRenderer.startColor = Color.green;
                _lineRenderer.endColor = Color.green;
            }
            else if (distance <= _elasticForce.SnapbackDistance)
            {
                _lineRenderer.startColor = Color.yellow;
                _lineRenderer.endColor = Color.yellow;
            }
            else
            {
                _lineRenderer.startColor = Color.red;
                _lineRenderer.endColor = Color.red;
            }
        }
    }
}
