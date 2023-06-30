using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Autohand;
using FMODUnity;

public class VinylInfo : MonoBehaviour
{
    [SerializeField] public string SceneToLoad;
    [SerializeField] public string Title, Description;
    [SerializeField] public VideoClip videoPreview;

    [SerializeField] public GameObject highlightOutline;

    [SerializeField] public Transform distanceChecker;

    [SerializeField] Transform vinylParent;

    [SerializeField] GameObject poofEffect;

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
    }

    private void Start()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
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

        if (Vector3.Distance(discPos, distanceChecker.position) > distanceToRespawn)
        {
            RuntimeManager.PlayOneShot("event:/SFX/Pop", transform.position);
            Instantiate(poofEffect, transform.position, transform.rotation);

            RespawnDisc();
        }

        if (isSpinning)
        {
            transform.Rotate(0, vinylSpinSpeed, 0);
        }
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
        rb.useGravity = false;
        rb.isKinematic = true;
        transform.position = vinylParent.position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        isSpinning = true;
    }
}
