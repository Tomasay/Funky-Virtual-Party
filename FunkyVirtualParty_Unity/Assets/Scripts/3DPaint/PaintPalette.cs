using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PaintPalette : MonoBehaviour
{
    [SerializeField]
    Color[] colors;

    [SerializeField]
    MeshRenderer[] colorMeshes;

    [SerializeField]
    Mesh leftHandMesh, rightHandMesh;
    public bool currentMeshLeft = true;
    
    MeshFilter mf;
    MeshCollider mc;

    public UnityEvent OnColorChanged;

#if UNITY_ANDROID
    [SerializeField]
    PaintSprayGun sprayGun;

    [SerializeField]
    ThreeDPen pen;

    Color colorToSet;
#endif

    void Start()
    {
        for (int i = 0; i < colorMeshes.Length; i++)
        {
            colorMeshes[i].material.color = colors[i];
#if UNITY_ANDROID
            var i2 = i;
            colorMeshes[i].GetComponent<TriggerEvents>().OnTriggerEntered.AddListener(delegate { SetColor(colors[i2]); });
            colorMeshes[i].GetComponent<TriggerEvents>().OnTriggerEntered.AddListener(ColorPressed);
#endif
        }

        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif
    }

#if UNITY_ANDROID
    void SetColor(Color c)
    {
        colorToSet = c;
    }

    void ColorPressed(Collider other)
    {
        if (other.name.Equals("Tip"))
        {
            pen.ChangeColor(colorToSet);
        }
        else if(other.name.Equals("Paint Spray Gun"))
        {
            sprayGun.ChangeColor(colorToSet);
        }

        OnColorChanged.Invoke();
    }
#endif

    /// <summary>
    /// Mirror the mesh so that it can be held in opposite hand with same hand pose
    /// </summary>
    public void Mirror()
    {
        currentMeshLeft = !currentMeshLeft;

        mf.sharedMesh = currentMeshLeft ? leftHandMesh : rightHandMesh;
        mc.sharedMesh = currentMeshLeft ? leftHandMesh : rightHandMesh;

        foreach (MeshRenderer mr in colorMeshes)
        {
            Vector3 newScale = mr.gameObject.transform.localScale;
            newScale.Scale(new Vector3(-1, 1, 1));
            mr.gameObject.transform.localScale = newScale;
        }

#if UNITY_ANDROID
        if(ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "MirrorPalette", "");
#endif
    }

#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("MirrorPalette"))
        {
            Mirror();
        }
    }
#endif
}