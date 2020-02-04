using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    public Transform Handle;
    public bool isDragging = false;
    public float Speed;
    public Vector3 movement;

    private void Start()
    {
        isDragging = false;
    }

    void Update()
    {
        Drag();
    }

    public void StartDrag()
    {
        isDragging = true;
    }
    public void StopDrag()
    {
        isDragging = false;
        Handle.position = transform.position;
    }
    public void Drag()
    {
        if (!isDragging) return;
        if(Input.touchCount != 0)
        {
            Vector3 touchPos = Input.GetTouch(0).position;
            Vector3 Pos = touchPos - transform.position;
            Vector3 FixedPos = Vector3.ClampMagnitude(Pos, 150);
            Handle.position = transform.position + FixedPos;

            Speed = FixedPos.magnitude / 150f;
            movement = FixedPos;
        }
    }
}
