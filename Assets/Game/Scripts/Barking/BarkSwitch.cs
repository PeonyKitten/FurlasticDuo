using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Barking
{
    public class BarkSwitch : MonoBehaviour, IBarkReaction
    {
        [Header("Reaction Settings")]
        [SerializeField] private bool reactIfInRange = false;
        [SerializeField] private float reactionRange = 10f;
        
        [Header("Stop Reaction Settings")]
        [SerializeField] private bool stopReactionAfterTime = true;
        [SerializeField] private float barkStopReactionTime = 3f;
        
        [Header("Callbacks")]
        public UnityEvent onBark;
        public UnityEvent onBarkReactionEnd;

        public bool IsReacting { get; set; }

        private Coroutine _barkReaction;
        
        public void React(Bark bark)
        {
            var isOutOfRange = Vector3.Distance(bark.transform.position, transform.position) > reactionRange;
            
            if (reactIfInRange && isOutOfRange) return;
            
            onBark?.Invoke();

            if (!stopReactionAfterTime) return;
            
            if (_barkReaction is not null)
            {
                StopCoroutine(_barkReaction);
            }
            _barkReaction = StartCoroutine(OnBarkReactionEnd());
        }

        private IEnumerator OnBarkReactionEnd()
        {
            yield return new WaitForSeconds(barkStopReactionTime);
            
            onBarkReactionEnd?.Invoke();
        }
    }
}
