using FD.Player;
using UnityEngine;

namespace FD.Collectibles
{
    public class DummyCollectible : MonoBehaviour, ICollectible
    {
        public void Collect(PlayerController _)
        {
            Debug.Log("Dummy collectible has been collected :)");
        }
    }
}
