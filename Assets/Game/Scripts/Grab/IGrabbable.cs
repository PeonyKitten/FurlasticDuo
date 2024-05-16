using UnityEngine;

public interface IGrabbable
{
    void OnGrab(Transform grabPoint);
    void OnRelease(Transform grabPoint);
}
