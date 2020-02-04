using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{ 
    public int ID;
    public SpriteRenderer spriteRenderer;

    public void Init(int id, Sprite sprite)
    {
        ID = id;
        spriteRenderer.sprite = sprite;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 localPos = transform.localPosition;
        localPos.z = -2f + ((pos.y + 3.84f) / 10f) + (pos.x / 100f);
        transform.localPosition = localPos;
    }
}
