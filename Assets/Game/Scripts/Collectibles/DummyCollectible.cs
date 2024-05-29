using UnityEngine;

namespace Game.Scripts.Collectibles
{
    public class DummyCollectible : MonoBehaviour, ICollectible
    {
        public void Collect()
        {
            Debug.Log("Dummy collectible has been collected :)");
        }
    }
}
