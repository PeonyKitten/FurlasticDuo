using UnityEngine;

namespace FD.Animation
{
    // Based on: https://www.youtube.com/watch?v=OCd7terfNxk
    public class PlayRandomIdleAnimation : StateMachineBehaviour
    {
        [SerializeField] private float timeout;
        [SerializeField] private int numberOfAnimations;
        [SerializeField] private string parameterName;

        private bool _isBored;
        private float _timeoutTimer;
        private int _animHashParameterName;
        private int _boredAnim;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _animHashParameterName = Animator.StringToHash(parameterName);
            ResetIdle();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_isBored)
            {
                _timeoutTimer += Time.deltaTime;

                if (_timeoutTimer > timeout && stateInfo.normalizedTime % 1 < 0.02f)
                {
                    _isBored = true;
                    _boredAnim = Random.Range(1, numberOfAnimations + 1);
                    _boredAnim = _boredAnim * 2 - 1;
                    
                    animator.SetFloat(_animHashParameterName, _boredAnim - 1);
                }
            } else if (stateInfo.normalizedTime % 1 > 0.98f)
            {
                ResetIdle();
            }
                    
            animator.SetFloat(_animHashParameterName, _boredAnim, 0.2f, Time.deltaTime);
        }

        private void ResetIdle()
        {
            if (_isBored)
            {
                _boredAnim--;
            }
            
            _isBored = false;
            _timeoutTimer = 0;
        }
    }
}
