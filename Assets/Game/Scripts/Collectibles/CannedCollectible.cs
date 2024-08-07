using FD.Player;
using UnityEngine;

namespace FD.Collectibles
{
    public class CannedCollectible: MonoBehaviour, ICollectible
    {
        public void Collect(PlayerController player)
        {
            Debug.Log("Can Collectible");
        }
    }
}