using UnityEngine;

namespace Game.Scripts.Toys
{
    public enum PlatformType
    {
        Pingpong,
        Loop
    }

    public class MovingPlatform : MonoBehaviour
    {
        public Transform waypoint1;
        public Transform waypoint2;
        public float speed = 2f;
        public PlatformType platformType;

        private Vector3 targetPosition;

        void Start()
        {
            // Set the initial target position to the first waypoint
            targetPosition = waypoint1.position;
        }

        void Update()
        {
            // Move the platform towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Check if the platform has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // Switch target position to the other waypoint
                if (targetPosition == waypoint1.position)
                {
                    targetPosition = waypoint2.position;
                }
                else
                {
                    if (platformType == PlatformType.Loop)
                    {
                        transform.position = waypoint1.position;
                        targetPosition = waypoint2.position;
                    }
                    else targetPosition = waypoint1.position;
                }
            }
        }

        // Visualize the waypoints and path in the editor
        void OnDrawGizmos()
        {
            if (waypoint1 != null && waypoint2 != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(waypoint1.position, waypoint2.position);
                Gizmos.DrawSphere(waypoint1.position, 0.2f);
                Gizmos.DrawSphere(waypoint2.position, 0.2f);
            }
        }
    }
}