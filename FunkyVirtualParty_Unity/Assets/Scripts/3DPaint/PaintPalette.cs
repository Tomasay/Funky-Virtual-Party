using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations;
using Normal.Realtime;

public class PaintPalette : MonoBehaviour
{
    public Color[] colors;

    [SerializeField]
    MeshRenderer[] colorMeshes;

    [SerializeField]
    MeshSyncer[] meshSyncers;

    [SerializeField]
    Mesh leftHandMesh, rightHandMesh;
    public bool currentMeshLeft = true;

    [SerializeField]
    public ParentConstraint constraint;

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    RealtimeTransform realtimeTransform;

    MeshFilter mf;

#if !UNITY_WEBGL //No collider necessary on web
    MeshCollider mc;
#endif

    public UnityEvent OnColorChanged;

    public Rigidbody Rb { get => rb; }
    public RealtimeTransform RealtimeTransform { get => realtimeTransform; }

#if !UNITY_WEBGL
    Color colorToSet;
#endif

    void Awake()
    {
        VRtistrySyncer.instance.PaletteMirrored.AddListener(Mirror);
        VRtistrySyncer.instance.paletteEnabledChanged.AddListener(SetActive);

#if !UNITY_WEBGL
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
#endif

        for (int i = 0; i < colorMeshes.Length; i++)
        {
            colorMeshes[i].material.color = colors[i];
#if !UNITY_WEBGL
            var i2 = i;
            colorMeshes[i].GetComponent<TriggerEvents>().OnTriggerEntered.AddListener(delegate { SetColor(colors[i2]); });
            colorMeshes[i].GetComponent<TriggerEvents>().OnTriggerEntered.AddListener(ColorPressed);
#endif
        }

        mf = GetComponent<MeshFilter>();

#if !UNITY_WEBGL //No collider necessary on web
        mc = GetComponent<MeshCollider>();
#endif
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.PaletteMirrored.RemoveListener(Mirror);
        VRtistrySyncer.instance.paletteEnabledChanged.RemoveListener(SetActive);

#if !UNITY_WEBGL
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
#endif
    }

#if !UNITY_WEBGL
    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        //Setup default constraint
        ConstraintSource newSource = new ConstraintSource();
        newSource.sourceTransform = avatar.GetComponent<VRtistryVRPlayerController>().leftHandGrabPoint;
        newSource.weight = 1;
        constraint.AddSource(newSource);
        constraint.constraintActive = true;
    }
#endif

#if !UNITY_WEBGL
    void SetColor(Color c)
    {
        colorToSet = c;
    }

    void ColorPressed(Collider other)
    {
        if (other.name.Equals("Tip"))
        {
            VRtistrySyncer.instance.PenColor = colorToSet;
        }
        else if(other.name.Equals("PaintSprayGun(Clone)"))
        {
            VRtistrySyncer.instance.SprayGunColor = colorToSet;
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

#if !UNITY_WEBGL //No collider necessary on web
        mc.sharedMesh = currentMeshLeft ? leftHandMesh : rightHandMesh;
#endif

        foreach (MeshRenderer mr in colorMeshes)
        {
            Vector3 newScale = mr.gameObject.transform.localScale;
            newScale.Scale(new Vector3(-1, 1, 1));
            mr.gameObject.transform.localScale = newScale;
        }
    }

    public void SetActive(bool active)
    {
#if UNITY_ANDROID
        foreach (MeshSyncer ms in meshSyncers)
        {
            ms.Enabled = active;
        }
#endif
    }
}