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

    [SerializeField] public AutoHandPlayer vrPlayer;

    [SerializeField] public Transform distanceChecker;

    [SerializeField] public VinylDiscSyncer syncer;

    Grabbable grabbable;
    Collider col;
    Rigidbody rb;


    [SerializeField] GameObject poofEffect;
    float distanceToRespawn = 10;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        Physics.IgnoreCollision(col, vrPlayer.headModel.GetComponent<Collider>());
        Physics.IgnoreCollision(col, vrPlayer.capsuleColl);

        grabbable = GetComponent<Grabbable>();
        grabbable.onHighlight.AddListener(OnHighlight);
        grabbable.onUnhighlight.AddListener(OnUnhighlight);
    }

    private void Update()
    {
        Vector3 discPos = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(discPos, distanceChecker.position) > distanceToRespawn)
        {
            RuntimeManager.PlayOneShot("event:/SFX/Pop", transform.position);
            Instantiate(poofEffect, transform.position, transform.rotation);

            syncer.RespawnDisc();

            if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByte", "RespawnDisc", syncer.objectID);
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
}
