using UnityEngine;

namespace FD.Collectibles
{
    public class DummyCollectible : MonoBehaviour, ICollectible
    {
        public void Collect()
        {
            Debug.Log("Dummy collectible has been collected :)");
        }
    }
}
