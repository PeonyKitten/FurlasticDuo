using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillbord : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }
}
