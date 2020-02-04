using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFill : MonoBehaviour
{
    public Timer Timer;

    public Vector3 fullPosition;
    public Vector3 emptyPosition;
    
    void Update()
    {
        transform.localPosition = Vector3.Lerp(emptyPosition, fullPosition, Mathf.Clamp01(Timer.TimeLeft / Timer.Maxtime));
    }

}
