using FD.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FD.AI.SteeringBehaviours
{
    public abstract class SteeringBehaviour : MonoBehaviour
    {
        [Header("Steering Behaviour")]
        [SerializeField] protected float weight = 1f;
        public bool useMouseInput = true;
        private Vector3 _target;

        public float Weight { get => weight; set => weight = value; }
        public Vector3 Target
        {
            get => _target;
            set => _target = value;
        }

        [HideInInspector] public SteeringAgent steeringAgent;

        private void Awake()
        {
            // TODO: Perhaps basing all our logic on the position of the SteeringBehaviour should be a conscious choice?
            transform.localPosition = Vector3.zero;
        }

        protected bool CheckMouseInput()
        {
            if (useMouseInput && Mouse.current.leftButton.isPressed)
            {
                var ray = CameraUtils.Main.ScreenPointToRay(Mouse.current.position.value);
                if (Physics.Raycast(ray, out var hit, 100))
                {
                    Target = hit.point;
                    steeringAgent.reachedGoal = false;
                    return true;
                }
            }
            return false;
        }
        public abstract Vector3 CalculateForce();
    }
}
