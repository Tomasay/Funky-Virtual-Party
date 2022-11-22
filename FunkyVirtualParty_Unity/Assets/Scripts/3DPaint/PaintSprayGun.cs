using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSprayGun : MonoBehaviour
{
    [SerializeField]
    ParticleSystem ps;

    [SerializeField]
    ThreeDPaintGameManager gm;

    public bool canPaint = true;

    private void Awake()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif
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
    }
#endif
}