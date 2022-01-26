using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LagManager : MonoBehaviour
{
    public float fps;
    void Update()
    {
        fps = Time.frameCount / Time.time;
    }
}
