using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Collectibles
{
    public class PickupCollectible: MonoBehaviour
    {
        [SerializeField] private GameObject collectibleObject;
        [SerializeField] private bool disableAfterCollect = true;

        public UnityEvent onCollectibleCollect;

        private ICollectible _collectible;

        private void Start()
        {
            if (!collectibleObject.TryGetComponent(out _collectible))
            {
                Debug.LogWarning($"Collectible {collectibleObject} does not have a Component that implements ICollectible :(");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController _)) return;
            
            _collectible.Collect();
            onCollectibleCollect?.Invoke();

            if (disableAfterCollect)
            {
                gameObject.SetActive(false);
            }
        }
    }
}