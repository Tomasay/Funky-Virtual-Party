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
    bool currentMeshRight;

    [SerializeField]
    PaintSprayGun sprayGun;

    [SerializeField]
    ThreeDPen pen;

    MeshFilter mf;
    MeshCollider mc;

    Color colorToSet;

    public UnityEvent OnColorChanged;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < colorMeshes.Length; i++)
        {
            colorMeshes[i].material.color = colors[i];
            var i2 = i;
            colorMeshes[i].GetComponent<TriggerEvents>().OnTriggerEntered.AddListener(delegate { SetColor(colors[i2]); });
            colorMeshes[i].GetComponent<TriggerEvents>().OnTriggerEntered.AddListener(ColorPressed);
        }

        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();
    }

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

    public void Mirror()
    {
        currentMeshRight = !currentMeshRight;

        mf.sharedMesh = currentMeshRight ? rightHandMesh : leftHandMesh;
        mc.sharedMesh = currentMeshRight ? rightHandMesh : leftHandMesh;
    }
}