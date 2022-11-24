using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    public float fps;
    private float timePerFrame;

    public Sprite[] frames;
    private int currentFrame;

    SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();

        timePerFrame = 1 / fps;
        InvokeRepeating("NextFrame", timePerFrame, timePerFrame);
    }

    private void NextFrame()
    {
        currentFrame++;

        if (currentFrame > frames.Length - 1)
            currentFrame = 0;

        rend.sprite = frames[currentFrame];
    }
}
