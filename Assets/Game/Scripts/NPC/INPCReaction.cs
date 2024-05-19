using UnityEngine;

namespace Game.Scripts.Barking
{
    public interface INPCReaction
    {
        bool IsReacting { get; set; }
        void ReactToBark(Vector3 barkOrigin);
    }
}
