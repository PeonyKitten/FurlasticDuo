using UnityEngine;

namespace Game.Scripts
{
    public class DestroyEventually: MonoBehaviour
    {
        [SerializeField] private float destroyDelay = 1f;
        
        private float _destroyTimer;

        private void Start()
        {
            _destroyTimer = destroyDelay;
        }
        
        private void Update()
        {
            _destroyTimer -= Time.deltaTime;

            if (_destroyTimer < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}