using UnityEngine;

namespace Game.Scripts.Barking
{
    public interface IBarkReaction
    {
        public bool IsReacting { get; set; }
        public void React(Bark bark);
    }
}
