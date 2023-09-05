using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using UnityEngine.Events;

#if UNITY_ANDROID
using FMODUnity;
#endif

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
    P3dPaintSphere paintSphere;

    [SerializeField]
    MeshRenderer baseMesh;

    [SerializeField]
    PaintPalette palette;

    [SerializeField]
    Collider col;

    private bool canPaint = true;

    bool isInHand;

    public bool active;

    public bool IsInHand { get => isInHand; set => isInHand = value; }
    public bool CanPaint { get => canPaint; set { canPaint = value; if (!value) { StopPainting(); } } }

    public UnityEvent OnSpray;

    private void Awake()
    {
        paintColorMat.color = colors[colorIndex];
        ps.startColor = colors[colorIndex];
        paintSphere.Color = colors[colorIndex];
    }

    private void Start()
    {
        VRtistrySyncer.instance.StartedPainting.AddListener(ps.Play);
        VRtistrySyncer.instance.StoppedPainting.AddListener(ps.Stop);
    }

#if UNITY_ANDROID
    public void OnSqueeze()
    {
        if (canPaint)
        {
            VRtistrySyncer.instance.IsPainting = true;
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
        VRtistrySyncer.instance.IsPainting = false;
    }

    public void ChangeColor(Color c)
    {
#if UNITY_ANDROID
        if (IsInHand)
        {
            if (!(paintColorMat.color == c))
            {
                RuntimeManager.PlayOneShot("event:/SFX/Drop", transform.position);
            }

            paintColorMat.color = c;
            ps.startColor = c;
            paintSphere.Color = c;
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
#endif
#if UNITY_WEBGL
        baseMesh.enabled = active;
        this.active = active;
#endif
    }
}