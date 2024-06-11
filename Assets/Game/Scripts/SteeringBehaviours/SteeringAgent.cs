using Game.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public class SteeringAgent: MonoBehaviour
    {
        [Serializable]
        public enum SummingStrategy
        {
            WeightedAverage,
            Prioritized
        }

        [Serializable]
        public enum MovementStrategy
        {
            DirectTransform,
            CharacterController,
            RigidbodyForces
        }

        [Header("Agent Settings")]
        public SummingStrategy summingStrategy = SummingStrategy.WeightedAverage;
        public MovementStrategy movementStrategy = MovementStrategy.DirectTransform;

        [SerializeField] private float mass = 1f;
        public float maxSpeed = 1f;
        public float maxForce = 10f;
        public bool reachedGoal;
        
        public float Mass
        {
            get => mass;
            set
            {
                mass = value;
                InverseMass = 1f / value;
            }
        }

        public float InverseMass { get; private set; }
        public Vector3 Velocity { get; private set; } = Vector3.zero;

        private readonly List<SteeringBehaviour> _steeringBehaviours = new();

        public int SteeringBehaviourCount => _steeringBehaviours.Count;

        public float angularDampeningTime = 5f;
        public float deadZone = 10f;
        
        public bool useRootMotion = true;
        public bool useGravity = true;
        
        private Animator _animator;
        private CharacterController _controller;
        private Rigidbody _rigidbody;
        private readonly int _animHashSpeed = Animator.StringToHash("Speed");

        private Vector3 CalculateSteeringForce()
        {
            var totalForce = Vector3.zero;

            foreach (var behaviour in _steeringBehaviours)
            {
                if (!behaviour.enabled) continue;

                var behaviourForce = behaviour.CalculateForce() * behaviour.Weight;

                switch (summingStrategy)
                {
                    case SummingStrategy.WeightedAverage:
                        totalForce += behaviourForce;
                        break;
                    case SummingStrategy.Prioritized:
                        if (!AccumulateForce(ref totalForce, behaviourForce))
                        {
                            return totalForce;
                        }
                        break;
                }
            }

            // Clamp Total Force
            totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
            
            return totalForce;
        }

        protected virtual void Start()
        {
            // Force calculation of inverse mass
            Mass = mass;
            
            _steeringBehaviours.AddRange(GetComponentsInChildren<SteeringBehaviour>());
            foreach (var behaviour in _steeringBehaviours)
            {
                behaviour.steeringAgent = this;
            }
            
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                useRootMotion = false;
            }

            _controller = GetComponent<CharacterController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Update()
        {
            var steeringForce = CalculateSteeringForce();

            if (reachedGoal)
            {
                Velocity = Vector3.zero;
                if (_animator != null)
                {
                    _animator.SetFloat(_animHashSpeed, 0);
                }
                return;
            }
            
            var acceleration = steeringForce * InverseMass;

            Velocity += acceleration * Time.deltaTime;

            if (Velocity.sqrMagnitude == 0) return;
            
            // Slow down, Speedy Gonzales
            Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

            if (_animator != null)
            {
                _animator.SetFloat(_animHashSpeed, Velocity.magnitude);
            }
            
            // Rotate Agent
            var angle = Vector3.Angle(transform.forward, Velocity);
            if (Mathf.Abs(angle) < deadZone)
            {
                transform.LookAt(transform.position + Velocity.Flatten(UtilsMath.FlattenAxis3D.Y).Bulk());
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    Quaternion.LookRotation(Velocity.Flatten(UtilsMath.FlattenAxis3D.Y).Bulk()), 
                    Time.deltaTime * angularDampeningTime);
            }
            
            // Movement is handled for us :)
            if (useRootMotion) return;
            
            Move(Velocity);
        }
        
        private void OnAnimatorMove()
        {
            if (!useRootMotion || Time.deltaTime == 0f) return;

            var animationVelocity = _animator.deltaPosition / Time.deltaTime;

            var motion = transform.forward * animationVelocity.magnitude;
            
            Move(motion);
        }

        private void Move(Vector3 motion)
        {
            var useTransformMovement = movementStrategy == MovementStrategy.DirectTransform;
            
            switch (movementStrategy)
            {
                case MovementStrategy.CharacterController:
                    if (_controller)
                    {
                        _controller.Move(motion * Time.deltaTime);
                        if (useGravity)
                        {
                            _controller.Move(Physics.gravity * Time.deltaTime);
                        }
                    } else
                    {
                        Debug.LogWarning("Steering Agent movement mode set to 'CharacterController' but missing Component. Falling back to position update.");
                        useTransformMovement = true;
                    }
                    break;
                case MovementStrategy.RigidbodyForces:
                    if (_rigidbody)
                    {
                        _rigidbody.AddForce(motion, ForceMode.Force);
                    } else
                    {
                        Debug.LogWarning("Steering Agent movement mode set to 'Rigidbody' but missing Component. Falling back to position update.");
                        useTransformMovement = true;
                    }
                    break;
            }

            if (useTransformMovement)
            {
                transform.position += motion * Time.deltaTime;
            }
        }

        private bool AccumulateForce(ref Vector3 totalForce, Vector3 force)
        {
            var magnitudeCapacityLeft = maxForce - totalForce.magnitude;

            if (magnitudeCapacityLeft <= 0) return false;

            totalForce += Vector3.ClampMagnitude(force, magnitudeCapacityLeft);
            
            return true;
        }

        public void SetTarget(Vector3 target)
        {
            foreach (var behaviour in _steeringBehaviours)
            {
                behaviour.Target = target;
            }
        }
    }
}