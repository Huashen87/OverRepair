using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDownNeedDialog : MonoBehaviour
{
    public RobotDown RobotDown;
    public GameObject Need1;
    private Sprite Need1Sprite;
    private bool[] NeedItem;
    private float Timer;
    public Sprite[] sprites;
    private int Count;
    private int MaxCount;

    private void Start()
    {
        Need1Sprite = Need1.GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
        Timer += Time.deltaTime;
        NeedItem = RobotDown.GetEatable();
        ReflashSprite();
    }

    private void ReflashSprite()
    {
        if (Timer >= 1)
        {

            bool hasSomeThing = false;
            int ccount = 0;
            for (int i = Count; true; i=(i+1)%8)
            {

                if (NeedItem[i])
                {
                    Need1.GetComponent<SpriteRenderer>().sprite = sprites[i];
                    Count = (i + 1) % 8;
                    hasSomeThing = true;
                    break;
                }
                if (i == Count)
                    ccount++;
                if (ccount == 2)
                    break;
            }
            if (!hasSomeThing)
                Need1.GetComponent<SpriteRenderer>().sprite = sprites[8];
            Timer = 0;
        }

    }
}
