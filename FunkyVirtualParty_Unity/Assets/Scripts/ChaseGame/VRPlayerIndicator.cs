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
    
    [SerializeField]
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
    private Quaternion tRot = Quaternion.identity;

    [SerializeField]
    private Camera cam;

    private void Update()
    {
        if (RealtimeSingletonWeb.instance.isVRAvatarSpawned && !InSight())
        {
            gameObject.SetActive(true);
            RotateToTarget();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void RotateToTarget()
    {
        Vector3 direction = RealtimeSingletonWeb.instance.VRAvatar.head.position;

        // Rotate element on UI (only on z-axis)
        tRot = Quaternion.LookRotation(direction);
        tRot.z = -tRot.y;
        tRot.x = 0;
        tRot.y = 0;

        Vector3 forwardAngle = new Vector3(0, 0, cam.transform.eulerAngles.y);
        RectTransform.rotation = tRot * Quaternion.Euler(forwardAngle);
    }

    bool InSight()
    {
        Vector3 ScreenPoint = cam.WorldToScreenPoint(RealtimeSingletonWeb.instance.VRAvatar.head.position);
        return ScreenPoint.z > 0 && ScreenPoint.x > 0 && ScreenPoint.x < 1 && ScreenPoint.y > 0 && ScreenPoint.y < 1;
    }
}