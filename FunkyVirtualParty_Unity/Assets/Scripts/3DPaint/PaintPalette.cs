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
        currentMeshLeft = !currentMeshLeft;

        mf.sharedMesh = currentMeshLeft ? leftHandMesh : rightHandMesh;
        mc.sharedMesh = currentMeshLeft ? leftHandMesh : rightHandMesh;

        foreach (MeshRenderer mr in colorMeshes)
        {
            Vector3 newScale = mr.gameObject.transform.localScale;
            newScale.Scale(new Vector3(-1, 1, 1));
            mr.gameObject.transform.localScale = newScale;
        }
    }
}