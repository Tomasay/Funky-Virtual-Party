using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    TMP_Text text;

    void Update()
    {
        text.text = "FPS: " + (int)(1.0f / Time.deltaTime);
    }
}