using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem2 : MonoBehaviour
{
    public Sprite[] sprites;

    void Update()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[GetComponentInParent<PlayerControl>().Item2];
    }
}
