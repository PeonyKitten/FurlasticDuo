using UnityEngine;

namespace Game.Scripts.Grab
{
    public enum GrabbableType
    {
        Static,
        Pickable,
        Draggable
    }

    public interface IGrabbable
    {
        GrabbableType Type { get; }
        void OnGrab(Transform grabPoint);
        void OnRelease(Transform grabPoint);
    }
}
