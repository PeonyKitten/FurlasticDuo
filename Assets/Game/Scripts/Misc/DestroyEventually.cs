using UnityEngine;

namespace Game.Scripts.Misc
{
    public class DestroyEventually: MonoBehaviour
    {
        [SerializeField] private float destroyDelay = 1f;
        [SerializeField] private bool timeScaleIndependent;
        
        private float _destroyTimer;

        private void Start()
        {
            _destroyTimer = destroyDelay;
        }
        
        private void Update()
        {
            _destroyTimer -= timeScaleIndependent ? Time.unscaledDeltaTime : Time.deltaTime;

            if (_destroyTimer < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}