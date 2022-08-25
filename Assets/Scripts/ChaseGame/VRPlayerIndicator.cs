using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayerIndicator : MonoBehaviour
{
    private CanvasGroup canvasGroup = null;
    protected CanvasGroup CanvasGroup
    {
        get
        {
            if(canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if(canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            return canvasGroup;
        }
    }

    private RectTransform rectTransform = null;
    protected RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
                if (rectTransform == null)
                {
                    rectTransform = gameObject.AddComponent<RectTransform>();
                }
            }

            return rectTransform;
        }
    }
    
    public GameManagerWeb Target { get; set; } = null;
    public Transform player = null;

    private Quaternion tRot = Quaternion.identity;
    private Vector3 tPos = Vector3.zero;
    private void Start()
    {
            
        Target = GameObject.Find("GameManager").GetComponent<GameManagerWeb>();
        player = ClientManagerWeb.instance.LocalPlayer.transform;
    }
    private void Update()
    {
        RotateToTarget();
    }

    void RotateToTarget()
    {
        if(Target)
        {
            tPos = Target.VRPlayerHeadPos;
        }    
        Vector3 direction = player.position - tPos;

        // Rotate element on UI (only on z-axis)
        tRot = Quaternion.LookRotation(direction);
        tRot.z = -tRot.y;
        tRot.x = 0;
        tRot.y = 0;

    }
}
