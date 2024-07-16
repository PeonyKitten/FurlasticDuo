using UnityEngine;

namespace FD.Grab
{
    public interface IGrabbable
    {
        bool ShouldAffectElasticForce()
        {
            return true;
        }
        void OnGrab(Transform grabPoint);
        void OnRelease(Transform grabPoint);
        void ReleaseAll();
    }
}
