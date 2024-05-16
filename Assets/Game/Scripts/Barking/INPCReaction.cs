using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCReaction
{
    bool isReacting { get; set; }
    void ReactToBark(Vector3 barkOrigin);
}
