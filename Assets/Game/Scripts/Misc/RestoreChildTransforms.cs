// RestoreChildTransforms.cs
// Alvin Philips
// 2024-06-20
// Stores and resets the position and rotation of all children, and optionally reset Rigidbodies.

using System.Collections.Generic;
using FD.Levels.Checkpoints;
using UnityEngine;

namespace FD.Misc
{
    public class RestoreChildTransforms: MonoBehaviour
    {
        private struct TransformData
        {
            public Vector3 position;
            public Quaternion rotation;

            public TransformData(Vector3 position, Quaternion rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }
        }

        [SerializeField] private bool storeGlobalPosition;
        [SerializeField] private bool storeLocalRotation;
        [SerializeField] private bool storeOnAwake;
        [SerializeField] private bool resetRigidbodyVelocities;
        [SerializeField] private bool resetOtherStuff;
        
        private readonly Dictionary<Transform, TransformData> _children = new();
        private readonly List<IReset> _resets = new();

        public void StoreTransforms()
        {
            _children.Clear();
            for (var index = 0; index < transform.childCount; index++)
            {
                var child = transform.GetChild(index);
                var position = storeGlobalPosition ? child.position : child.localPosition;
                var rotation = storeLocalRotation ? child.localRotation : child.rotation;
                _children.Add(child, new TransformData(position, rotation));
            }
            
            if (!resetOtherStuff) return;
            _resets.Clear();
            GetComponentsInChildren(true, _resets);
        }

        public void RestoreTransforms()
        {
            foreach (var (childTransform, childData) in _children)
            {
                if (storeGlobalPosition)
                {
                    childTransform.position = childData.position;
                }
                else
                {
                    childTransform.localPosition = childData.position;
                }

                if (storeLocalRotation)
                {
                    childTransform.localRotation = childData.rotation;
                }
                else
                {
                    childTransform.rotation = childData.rotation;
                }

                if (resetRigidbodyVelocities && childTransform.TryGetComponent(out Rigidbody rb))
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                } 
            }
            
            if (resetOtherStuff) return;

            foreach (var reset in _resets)
            {
                reset.Reset();
            }
        }

        private void Start()
        {
            if (storeOnAwake)
            {
                StoreTransforms();
            }
        }
    }
}