using System;
using System.Collections.Generic;
using System.Linq;
using FD.Levels.Checkpoints;
using FD.Misc;
using UnityEngine;

namespace FD.NPC.Security
{
    [RequireComponent(typeof(SphereCollider))]
    public class DetectPlayer : MonoBehaviour, IReset
    {
        public FOV fov;
        [SerializeField] private LayerMask wallLayerMask;
        private readonly HashSet<Transform> _playersInRange = new();
        private SphereCollider _collider;

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
        }

        public bool IsPlayerInFOV(out Transform detectedPlayer)
        {
            detectedPlayer = null;
            foreach (var player in _playersInRange)
            {
                if (fov.IsContained(player.position))
                {
                    detectedPlayer = player;
                    return true;
                }
            }
            return false;
        }

        public bool IsAnyPlayerInRange()
        {
            return _playersInRange.Count > 0;
        }

        public bool CanSeePlayer(bool inFOV = false)
        {
            var sphereRadius = _collider.radius * _collider.transform.lossyScale.magnitude;
            
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var player in _playersInRange)
            {
                if (inFOV && !fov.IsContained(player.position)) continue;
                
                var ray = new Ray(transform.position, player.position - transform.position);
                var didHit = Physics.Raycast(ray, out var hitInfo, sphereRadius,
                    wallLayerMask.value, QueryTriggerInteraction.Ignore);

                if (didHit && hitInfo.collider.CompareTag("Player"))
                {
                    return true;
                }
            }

            return false;
        }

        public Transform GetClosestPlayer(bool inFOV = false)
        {
            var sphereRadius = _collider.radius * _collider.transform.lossyScale.magnitude;
            
            Transform result = null;
            
            foreach (var player in _playersInRange)
            {
                if (inFOV && !fov.IsContained(player.position)) continue;
                
                var ray = new Ray(transform.position, player.position - transform.position);
                var didHit = Physics.Raycast(ray, out var hitInfo, sphereRadius,
                    wallLayerMask.value, QueryTriggerInteraction.Ignore);
            
                if (!didHit || !hitInfo.collider.CompareTag("Player")) continue;
                
                if (result is null || Vector3.Distance(transform.position, player.position) <
                    Vector3.Distance(transform.position, result.position))
                {
                    result = player;
                }
            }
            
            return result;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playersInRange.Add(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playersInRange.Remove(other.transform);
            }
        }

        public void Reset()
        { 
            _playersInRange.Clear();
        }
    }
}