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
    
    public GameManagerWeb Target { get; set; } = null;
    private Transform player = null;

    private Quaternion tRot = Quaternion.identity;
    private Vector3 tPos = Vector3.zero;

    [SerializeField]
    private Camera camera;
    private void Start()
    {
            
        Target = GameObject.Find("GameManager").GetComponent<GameManagerWeb>();
        if(!Target)
        {
            Debug.Log("GameManager not Found in VRPLAYERINDICATOR");
        }
        else
        {
            Debug.Log(Target.VRPlayerHeadPos);
        }
        player = ClientManagerWeb.instance.LocalPlayer.transform;
    }
    private void Update()
    {
        if (!InSight())
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
        if(Target)
        {
            tPos = Target.VRPlayerHeadPos;
        }
        Vector3 direction = player ? player.position - tPos : -tPos;

        // Rotate element on UI (only on z-axis)
        tRot = Quaternion.LookRotation(direction);
        tRot.z = -tRot.y;
        tRot.x = 0;
        tRot.y = 0;

        RectTransform.rotation = tRot;
    }

    bool InSight()
    {
        Vector3 ScreenPoint = camera.WorldToScreenPoint(Target.VRPlayerHeadPos);
        return ScreenPoint.z > 0 && ScreenPoint.x > 0 && ScreenPoint.x < 1 && ScreenPoint.y > 0 && ScreenPoint.y < 1;
    }
}
