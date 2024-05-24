using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Game.Scripts.Toys
{
    public class HamsterWheel : MonoBehaviour
    {
        [SerializeField] private float speed;

        private float _currentRotation = 0f;
        private List<Transform> enteredTransforms = new List<Transform>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Transform t))
            {
                enteredTransforms.Add(t);
            }
        }

        private void Update()
        {
            foreach (Transform t in enteredTransforms) 
            {
                Vector3 _movement = t.GetComponent<PlayerController>().Movement;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Transform t))
            {
                enteredTransforms.Remove(t);
            }
        }
    }
}
