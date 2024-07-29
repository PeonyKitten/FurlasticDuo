using FD.Levels.Checkpoints;
using FD.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FD.AI.SteeringBehaviours
{
    public abstract class SteeringBehaviour : MonoBehaviour, IReset
    {
        [Header("Steering Behaviour")]
        [SerializeField] protected float weight = 1f;
        private Vector3 _target;

        public float Weight { get => weight; set => weight = value; }
        public Vector3 Target
        {
            get => _target;
            set => _target = value;
        }

        [HideInInspector] public SteeringAgent steeringAgent;
        private IReset _resetImplementation;

        private void Awake()
        {
            // TODO: Perhaps basing all our logic on the position of the SteeringBehaviour should be a conscious choice?
            transform.localPosition = Vector3.zero;
        }
        public abstract Vector3 CalculateForce();
        public abstract void Reset();
    }
}
