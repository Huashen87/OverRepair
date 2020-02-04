using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{

    public SpriteSheetManager body;
    public SpriteSheetManager leftHand;
    public SpriteSheetManager rightHand;
    public SpriteSheetManager leftFoot;
    public SpriteSheetManager rightFoot;

    public float framePerSec = 2;

    private SpriteSheetManager[] spriteSheetManagers = new SpriteSheetManager[5];
    private Vector2 currentDir = Vector2.zero;
    private bool isPlaying = false;
    private float animTimer = 0;

    public void SetPart(bool[] parts)
    {
        for (int i = 1; i < Mathf.Min(parts.Length, spriteSheetManagers.Length); i++)
        {
            spriteSheetManagers[i].gameObject.SetActive(parts[i]);
        }
    }

    private void Start()
    {
        spriteSheetManagers[0] = body;
        spriteSheetManagers[1] = leftHand;
        spriteSheetManagers[2] = rightHand;
        spriteSheetManagers[3] = leftFoot;
        spriteSheetManagers[4] = rightFoot;

        SetDirection(Vector2.right);
    }

    private void Update()
    {
        Vector2 dir = PlayerControl.Instance.direction;
        SetDirection(dir);

        if (dir == Vector2.zero)
        {
            if (isPlaying)
            {
                IdleFrame();
                isPlaying = false;
            }
        }
        else
        {
            if (!isPlaying)
            {
                animTimer = 0;
                isPlaying = true;
            }
        }

        if (isPlaying)
        {
            animTimer -= Time.deltaTime;
            if (animTimer <= 0)
            {
                NextFrame();
                animTimer += 1f / framePerSec;
            }
        }
    }

    private void SetDirection(Vector2 dir)
    {
        if (dir.x == 0) return;

        string _AniName = "";

        if (dir.x > 0)
        {
            if (currentDir == Vector2.right) return;
            _AniName = "Right";
            currentDir = Vector2.right;
        }
        else if (dir.x < 0)
        {
            if (currentDir == Vector2.left) return;
            _AniName = "Left";
            currentDir = Vector2.left;
        }

        for (int i = 0; i < spriteSheetManagers.Length; i++)
        {
            if (spriteSheetManagers[i] == null) continue;
            spriteSheetManagers[i].LoadAnim(_AniName);
        }
    }

    private void NextFrame()
    {
        for (int i = 0; i < spriteSheetManagers.Length; i++)
        {
            if (spriteSheetManagers[i] == null) continue;
            spriteSheetManagers[i].NextFrame();
        }
    }

    private void IdleFrame()
    {
        for (int i = 0; i < spriteSheetManagers.Length; i++)
        {
            if (spriteSheetManagers[i] == null) continue;
            spriteSheetManagers[i].SetAnimFrame(0);
        }
    }
}
