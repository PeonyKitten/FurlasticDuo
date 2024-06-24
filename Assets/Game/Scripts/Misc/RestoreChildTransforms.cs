// RestoreChildTransforms.cs
// Alvin Philips
// 2024-06-20
// Stores and resets the position and rotation of all children, and optionally reset Rigidbodies.

using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Misc
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
        [SerializeField] private bool storeOnAwake;
        [SerializeField] private bool resetRigidbodyVelocities;
        
        private readonly Dictionary<Transform, TransformData> _children = new();

        public void StoreTransforms()
        {
            _children.Clear();
            for (var index = 0; index < transform.childCount; index++)
            {
                var child = transform.GetChild(index);
                var position = storeGlobalPosition ? child.position : child.localPosition;
                _children.Add(child, new TransformData(position, child.rotation));
            }
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
                childTransform.rotation = childData.rotation;

                if (resetRigidbodyVelocities && childTransform.TryGetComponent(out Rigidbody rb))
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                } 
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