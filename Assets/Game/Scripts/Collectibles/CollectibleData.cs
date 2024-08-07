using UnityEngine;

namespace FD.Collectibles
{
    [CreateAssetMenu(menuName = "FD/Collectible")]
    public class CollectibleData: ScriptableObject
    {
        public string collectibleName;
        public string description;
        public Sprite thumbnail;
    }
}