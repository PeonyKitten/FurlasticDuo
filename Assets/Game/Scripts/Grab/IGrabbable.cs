using UnityEngine;

namespace Game.Scripts.Grab
{
    public interface IGrabbable
    {
        void OnGrab(Transform grabPoint);
        void OnRelease(Transform grabPoint);
    }
}
