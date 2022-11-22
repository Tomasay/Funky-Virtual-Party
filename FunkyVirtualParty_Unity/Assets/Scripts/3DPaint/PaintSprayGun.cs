using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;

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

    public bool canPaint = true;

    bool isInHand;

    public bool IsInHand { get => isInHand; set => isInHand = value; }

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
        }
    }

    public void OnUnsqueeze()
    {
        if (canPaint)
        {
            ps.Stop();
            if (ClientManager.instance && gm.State == ThreeDPaintGameState.VRPainting) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PaintGunStopSpray", "");
        }
    }
#endif

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
            ChangeColor();
        }
    }
#endif

    public void ChangeColor()
    {
#if UNITY_ANDROID
        if (IsInHand)
        {
            if (colorIndex < colors.Length - 1)
            {
                colorIndex++;
            }
            else
            {
                colorIndex = 0;
            }

            paintColorMat.color = colors[colorIndex];
            ps.startColor = colors[colorIndex];
            paintSphere.Color = colors[colorIndex];

            if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ChangeColorSprayGun", "");
        }
#endif
#if UNITY_WEBGL
        if (colorIndex < colors.Length - 1)
            {
                colorIndex++;
            }
            else
            {
                colorIndex = 0;
            }

            paintColorMat.color = colors[colorIndex];
            ps.startColor = colors[colorIndex];
            paintSphere.Color = colors[colorIndex];
#endif
    }
}