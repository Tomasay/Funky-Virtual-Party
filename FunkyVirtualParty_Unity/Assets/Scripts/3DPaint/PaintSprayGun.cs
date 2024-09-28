using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using UnityEngine.Events;
using UnityEngine.Animations;

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
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

    [SerializeField]
    ParentConstraint constraint;

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
        VRtistrySyncer.instance.penEnabledChanged.AddListener(IsPenEnabledChanged);
        VRtistrySyncer.instance.spraygunColorChanged.AddListener(ChangeColor);

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
#endif
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.StartedPainting.RemoveListener(ps.Play);
        VRtistrySyncer.instance.StoppedPainting.RemoveListener(ps.Stop);
        VRtistrySyncer.instance.penEnabledChanged.RemoveListener(IsPenEnabledChanged);
        VRtistrySyncer.instance.spraygunColorChanged.RemoveListener(ChangeColor);

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
#endif
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        //Setup default constraint
        ConstraintSource newSource = new ConstraintSource();
        newSource.sourceTransform = avatar.GetComponent<VRtistryVRPlayerController>().rightHandGrabPoint;
        newSource.weight = 1;
        constraint.AddSource(newSource);
        constraint.constraintActive = true;
    }

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
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
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
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

    void IsPenEnabledChanged(bool penEnabled)
    {
        SetActive(!penEnabled);
    }

    public void SetActive(bool active)
    {
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
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