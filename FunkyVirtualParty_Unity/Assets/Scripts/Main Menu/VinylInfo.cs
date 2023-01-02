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

    Grabbable grabbable;
    Collider col;
    Rigidbody rb;

    Vector3 startingPos;
    Quaternion startingRot;


    [SerializeField] GameObject poofEffect;
    float distanceToRespawn = 10;

    private void Awake()
    {
        startingPos = transform.position;
        startingRot = transform.rotation;

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
            RespawnDisc();
        }
    }

    void RespawnDisc()
    {
        RuntimeManager.PlayOneShot("event:/SFX/Pop", transform.position);
        Instantiate(poofEffect, transform.position, transform.rotation);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startingPos;
        transform.rotation = startingRot;
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
