using UnityEngine;

namespace FD.Misc
{
    public class RotateAroundAxis : MonoBehaviour
    {
        [SerializeField] private Vector3 axis;
        [SerializeField] private float degreesPerSecond;

        private void Update()
        {
            transform.Rotate(axis, degreesPerSecond * Time.deltaTime);
        }
    }
}
