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
            foreach (var childData in _children)
            {
                if (storeGlobalPosition)
                {
                    childData.Key.position = childData.Value.position;
                }
                else
                {
                    childData.Key.localPosition = childData.Value.position;
                }
                childData.Key.rotation = childData.Value.rotation;

                if (resetRigidbodyVelocities && childData.Key.TryGetComponent(out Rigidbody rb))
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