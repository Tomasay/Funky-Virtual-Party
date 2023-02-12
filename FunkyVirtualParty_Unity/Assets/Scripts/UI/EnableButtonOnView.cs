using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableButtonOnView : MonoBehaviour
{
    public RectTransform rect;
    public Button button;

    Image img;
    Rect screenBounds;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        screenBounds = new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2);
        button.interactable = img.raycastTarget = (screenBounds.Contains(Camera.main.WorldToScreenPoint(rect.transform.position)));
    }
}