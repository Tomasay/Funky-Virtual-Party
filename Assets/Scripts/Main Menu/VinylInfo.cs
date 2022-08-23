using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Autohand;

public class VinylInfo : MonoBehaviour
{
    [SerializeField] public string SceneToLoad;
    [SerializeField] public string Title, Description;
    [SerializeField] public VideoClip videoPreview;

    [SerializeField] public GameObject highlightOutline;

    Grabbable grabbable;

    private void Awake()
    {
        grabbable = GetComponent<Grabbable>();

        grabbable.OnUIPointerHighlight.AddListener(OnHighlight);
        grabbable.OnUIPointerUnhighlight.AddListener(OnUnhighlight);
    }

    void OnHighlight(Hand hand, Grabbable g)
    {
        HapticsManager.instance.TriggerHaptic(hand.left, 0.1f, 0.1f);
        highlightOutline.SetActive(true);
    }

    void OnUnhighlight(Hand hand, Grabbable g)
    {
        highlightOutline.SetActive(false);
    }
}
