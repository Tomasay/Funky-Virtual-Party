using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    TMP_Text text;

    float timer;
    int frameCount;
    float fpsSum;

    void Update()
    {
        frameCount++;
        fpsSum += 1.0f / Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= 1.0f)
        {
            text.text = "FPS: " + (int)(fpsSum / frameCount);
            timer = 0f;
            frameCount = 0;
            fpsSum = 0f;
        }
    }
}