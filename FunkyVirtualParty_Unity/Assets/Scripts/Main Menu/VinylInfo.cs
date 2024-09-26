using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Autohand;
#if !UNITY_WEBGL
using FMODUnity;
#endif

public class VinylInfo : MonoBehaviour
{
    [SerializeField] public string SceneToLoad;
    [SerializeField] public string Title, Description;
    [SerializeField] public VideoClip videoPreview;

    [SerializeField] public GameObject highlightOutline;

    [SerializeField] public MeshRenderer vinylPreview;

    [SerializeField] public Vector3 distanceCheckerPosition, vinylParentPosition;

    [SerializeField] GameObject poofEffect;

#if UNITY_ANDROID
    [SerializeField] Material previewMat;
#endif

    public bool isOnPlayer;

    AutoHandPlayer vrPlayer;

    Grabbable grabbable;
    Collider col;
    Rigidbody rb;

    float distanceToRespawn = 10;

    Vector3 startingPos;
    Quaternion startingRot;

    float vinylSpinSpeed = 0.2f;
    bool isSpinning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        startingPos = transform.position;
        startingRot = transform.rotation;

        grabbable = GetComponent<Grabbable>();
        grabbable.onHighlight.AddListener(OnHighlight);
        grabbable.onUnhighlight.AddListener(OnUnhighlight);

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        if (previewMat)
        {
            vinylPreview.material = previewMat;
        }
#endif
    }

    private void Start()
    {
        if (RealtimeSingleton.instance)
        {
            RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
        }
    }

    private void OnDestroy()
    {
        if (RealtimeSingleton.instance)
        {
            RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
        }
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        vrPlayer = avatar.GetComponentInChildren<AutoHandPlayer>();

        Physics.IgnoreCollision(col, vrPlayer.headModel.GetComponent<Collider>());
        Physics.IgnoreCollision(col, vrPlayer.capsuleColl);
    }

    private void Update()
    {
        Vector3 discPos = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(discPos, distanceCheckerPosition) > distanceToRespawn)
        {
#if !UNITY_WEBGL
            RuntimeManager.PlayOneShot("event:/SFX/Pop", transform.position);
#endif
            Instantiate(poofEffect, transform.position, transform.rotation);

            RespawnDisc();
        }

        if (isSpinning)
        {
            transform.Rotate(0, vinylSpinSpeed, 0);
        }
    }

    public void OnGrabbed(Hand hand, Grabbable g)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        isSpinning = false;
        isOnPlayer = false;
    }

    public void OnHighlight(Hand hand, Grabbable g)
    {
        HapticsManager.instance.TriggerHaptic(hand.left, 0.1f, 0.1f);
        highlightOutline.SetActive(true);
    }

    public void OnUnhighlight(Hand hand, Grabbable g)
    {
        highlightOutline.SetActive(false);
    }

    public void RespawnDisc()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startingPos;
        transform.rotation = startingRot;
    }

    public void SetDiscOnPlayer()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.position = vinylParentPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        isSpinning = true;
        isOnPlayer = true;
    }
}
