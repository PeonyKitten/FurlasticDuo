using FD.Player;
using UnityEngine;

namespace FD.Collectibles
{
    public class CoinCollectible : MonoBehaviour, ICollectible
    {
        [SerializeField] private int scorePoints;
        
        public void Collect(PlayerController player)
        {
            player.InputHandler.AddScore(scorePoints);
        }
    }
}
