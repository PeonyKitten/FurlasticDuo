using UnityEngine;

namespace Game.Scripts.Misc
{
    public class DummyRigidbody : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private int divisor = 1;

        public float Mass => rb.mass / divisor;
        public Rigidbody Rb => rb;
    
        private void Start()
        {
            Debug.Assert(rb is not null, "Attached Rigidbody not set");
        }
    }
}
