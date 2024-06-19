using System;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Toys {
    public class Breakable : MonoBehaviour
    {
        [SerializeField] private bool onlyAllowPlayerBreak = true;
        [SerializeField] private float breakForce = 200f;
        [SerializeField] private bool breakOnce = true;

        public UnityEvent onBreak;
        
        public bool IsBroken { get; private set; }
        
        private void OnCollisionEnter(Collision other)
        {
            // Check if we were already broken
            if (breakOnce && IsBroken) return;
            
            // Check if we're the Player
            if (onlyAllowPlayerBreak && !other.gameObject.TryGetComponent(out PlayerController _)) return;

            // Check whether sufficient force was applied
            if (other.impulse.sqrMagnitude < breakForce * breakForce) return;
            
            IsBroken = true;
            
            onBreak?.Invoke();
        }
    }
}
