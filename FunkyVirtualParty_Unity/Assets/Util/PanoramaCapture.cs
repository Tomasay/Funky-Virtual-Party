using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PanoramaCapture : MonoBehaviour
{
    public Camera targetCamera;
    public RenderTexture cubeMapLeft;
    public RenderTexture equirectRT;

    public float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayedCapture());
    }

    [Button]
    public void Capture()
    {
        targetCamera.RenderToCubemap(cubeMapLeft);
        cubeMapLeft.ConvertToEquirect(equirectRT);
        Save(equirectRT);
    }

    IEnumerator DelayedCapture()
    {
        yield return new WaitForSeconds(delay);
        Capture();
    }

    public void Save(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height);

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToJPG(100);

        string path = Application.dataPath + "/Panorama" + ".jpg";

        System.IO.File.WriteAllBytes(path, bytes);
    }
}