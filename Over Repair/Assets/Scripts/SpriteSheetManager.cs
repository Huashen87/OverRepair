using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetManager : MonoBehaviour {

    [System.Serializable]
    public struct SpriteData {
        public string name;
        public Sprite[] sprites;
    }

    public SpriteData[] spriteDatas;

    SpriteRenderer _spriteRenderer;

    bool     _isAnimPlaying   = false;
    float    _prevSpriteTime  = 0f;
    float    _nowInterval     = 0f;
    Sprite[] _nowSprites      = new Sprite[0];
    int      _nowSpritesIndex = 0;

    void Awake () {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update () {

        if (_isAnimPlaying) {

            if (Time.timeSinceLevelLoad - _prevSpriteTime > _nowInterval) {

                _nowSpritesIndex = (_nowSpritesIndex + 1) % _nowSprites.Length;
                _prevSpriteTime  = Time.timeSinceLevelLoad;

                _spriteRenderer.sprite = _nowSprites[_nowSpritesIndex];
            }
        }

    }

    public void LoadAnim(string animName)
    {
        foreach (SpriteData data in spriteDatas)
        {
            if (data.name == animName)
            {
                _nowSprites = data.sprites;
                _nowSpritesIndex = 0;
            }
        }

        // set the 1st sprite
        SetAnimFrame(0);
    }

    public void PlayAnim (string animName, float interval) {


    }

    public void SetAnimFrame(int index)
    {
        if (_nowSprites.Length == 0)
        {
            _spriteRenderer.sprite = null;
            _nowSpritesIndex = 0;
            return;
        }

        _nowSpritesIndex = index % _nowSprites.Length;
        _spriteRenderer.sprite = _nowSprites[_nowSpritesIndex];
    }

    public void NextFrame()
    {
        SetAnimFrame(_nowSpritesIndex + 1);
    }

    public void SetAnimInterval (float interval) {
        _nowInterval = interval;
    }

    public void StopAnim () {
        _isAnimPlaying = false;
    }


}
