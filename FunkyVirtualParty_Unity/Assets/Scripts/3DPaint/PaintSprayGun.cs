using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using UnityEngine.Events;

public class PaintSprayGun : MonoBehaviour
{
    [SerializeField]
    Color[] colors;
    int colorIndex = 0;

    [SerializeField]
    Material paintColorMat;

    [SerializeField]
    ParticleSystem ps;

    [SerializeField]
    ThreeDPaintGameManager gm;

    [SerializeField]
    P3dPaintSphere paintSphere;

    [SerializeField]
    MeshRenderer baseMesh;

#if UNITY_ANDROID
    [SerializeField]
    Collider col;
#endif

    private bool canPaint = true;

    bool isInHand;

    public bool active;

    public bool IsInHand { get => isInHand; set => isInHand = value; }
    public bool CanPaint { get => canPaint; set { canPaint = value; if (!value) { StopPainting(); } } }

    public UnityEvent OnSpray;

    private void Awake()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif

        paintColorMat.color = colors[colorIndex];
        ps.startColor = colors[colorIndex];
        paintSphere.Color = colors[colorIndex];
    }

#if UNITY_ANDROID
    public void OnSqueeze()
    {
        if (canPaint)
        {
            ps.Play();
            if (ClientManager.instance && gm.State == ThreeDPaintGameState.VRPainting) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PaintGunStartSpray", "");
            OnSpray.Invoke();
        }
    }

    public void OnUnsqueeze()
    {
        if (canPaint)
        {
            StopPainting();
        }
    }
#endif

    void StopPainting()
    {
        ps.Stop();
        if (ClientManager.instance && gm.State == ThreeDPaintGameState.VRPainting) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PaintGunStopSpray", "");
    }

#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("PaintGunStartSpray"))
        {
            ps.Play();
        }
        else if(methodName.Equals("PaintGunStopSpray"))
        {
            ps.Stop();
        }
        else if(methodName.Equals("ChangeColorSprayGun"))
        {
            if (ColorUtility.TryParseHtmlString(data, out Color col))
            {
                ChangeColor(col);
            }
        }
        else if (methodName.Equals("SprayGunDisable"))
        {
            SetActive(false);
        }
        else if (methodName.Equals("SprayGunEnable"))
        {
            SetActive(true);
        }
    }
#endif

    public void ChangeColor(Color c)
    {
#if UNITY_ANDROID
        if (IsInHand)
        {
            paintColorMat.color = c;
            ps.startColor = c;
            paintSphere.Color = c;

            if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ChangeColorSprayGun", "#" + ColorUtility.ToHtmlStringRGB(c));
        }
#endif
#if UNITY_WEBGL
            paintColorMat.color = c;
            ps.startColor = c;
            paintSphere.Color = c;
#endif
    }

    public void SetActive(bool active)
    {
#if UNITY_ANDROID
        baseMesh.enabled = active;
        col.enabled = active;
        this.active = active;
        if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", active ? "SprayGunEnable" : "SprayGunDisable", "");
#endif
#if UNITY_WEBGL
        baseMesh.enabled = active;
        this.active = active;
#endif
    }
}