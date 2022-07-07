using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;

public class ShootoutGameVRPlayerController : VRPlayerController
{
    [SerializeField] GameObject fireballPrefab, fireballHandAnchorLeft, fireballHandAnchorRight;
    [SerializeField] GameObject handFireEffectLeft, handFireEffectRight;
    [SerializeField] Fireball[] fireballsPool;


    public float fireballThrowPower = 1, handOffset = 0.05f;

    private GameObject currentFireballLeft, currentFireballRight;

    private bool isGrabbingLeft, isGrabbingRight;

    void Awake()
    {
        ahp.handRight.OnTriggerGrab += OnGrabbed;
        ahp.handRight.OnTriggerRelease += OnRelease;
        ahp.handLeft.OnTriggerGrab += OnGrabbed;
        ahp.handLeft.OnTriggerRelease += OnRelease;

        PreloadFireball(true);
        PreloadFireball(false);
    }

    private void OnGrabbed(Hand hand, Grabbable grabbable)
    {
        Fireball fireball = hand.left ? currentFireballLeft.GetComponent<Fireball>() : currentFireballRight.GetComponent<Fireball>();
        fireball.col.enabled = true;
        fireball.constraint.enabled = false;

        hand.TryGrab(hand.left ? currentFireballLeft.GetComponent<Grabbable>() : currentFireballRight.GetComponent<Grabbable>());

        if (hand.left)
        {
            handFireEffectLeft.SetActive(true);
            isGrabbingLeft = true;
            fireball.isInLeftHand = true;
        }
        else
        {
            handFireEffectRight.SetActive(true);
            isGrabbingRight = true;
            fireball.isInRightHand = true;
        }

        StartCoroutine(TriggerFireballHaptics(hand.left));
    }

    private void OnRelease(Hand hand, Grabbable grabbable)
    {
        if (hand.left)
        {
            currentFireballLeft.GetComponent<Fireball>().OnDrop();
            isGrabbingLeft = false;
        }
        else
        {
            currentFireballRight.GetComponent<Fireball>().OnDrop();
            isGrabbingRight = false;
        }

        PreloadFireball(hand.left);

        if (hand.left)
        {
            handFireEffectLeft.SetActive(false);
        }
        else
        {
            handFireEffectRight.SetActive(false);
        }
    }

    private void PreloadFireball(bool isLeftHand)
    {
        //Get next available fireball in pool
        Fireball nextFireball = fireballsPool[0];
        for (int i = 0; i < fireballsPool.Length; i++)
        {
            if(fireballsPool[i].readyToSpawn)
            {
                nextFireball = fireballsPool[i];
                nextFireball.col.enabled = false;
                break;
            }
        }

        //Set up fireball
        if (isLeftHand)
        {
            currentFireballLeft = nextFireball.gameObject;
            currentFireballLeft.transform.localPosition = fireballHandAnchorLeft.transform.position;
            currentFireballLeft.transform.localRotation = Quaternion.identity;
            UnityEngine.Animations.ConstraintSource src = new UnityEngine.Animations.ConstraintSource();
            src.sourceTransform = fireballHandAnchorLeft.transform;
            src.weight = 1;

            Fireball f = currentFireballLeft.GetComponent<Fireball>();
            if (f.constraint.sourceCount > 0)
            {
                f.constraint.SetSource(0, src);
            }
            else
            {
                f.constraint.AddSource(src);
            }
            f.constraint.constraintActive = true;
            f.constraint.enabled = true;
            f.readyToSpawn = false;
        }
        else
        {
            currentFireballRight = nextFireball.gameObject;
            currentFireballRight.transform.localPosition = fireballHandAnchorRight.transform.position;
            currentFireballRight.transform.localRotation = Quaternion.identity;
            UnityEngine.Animations.ConstraintSource src = new UnityEngine.Animations.ConstraintSource();
            src.sourceTransform = fireballHandAnchorRight.transform;
            src.weight = 1;

            Fireball f = currentFireballRight.GetComponent<Fireball>();
            if (f.constraint.sourceCount > 0)
            {
                f.constraint.SetSource(0, src);
            }
            else
            {
                f.constraint.AddSource(src);
            }
            f.constraint.constraintActive = true;
            f.constraint.enabled = true;
            f.readyToSpawn = false;
        }
    }

    IEnumerator TriggerFireballHaptics(bool isLeft)
    {
        if (isLeft)
        {
            while(isGrabbingLeft)
            {
                HapticsManager.instance.TriggerHaptic(true, 0.1f, currentFireballLeft.GetComponent<Fireball>().currentScale);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            while (isGrabbingRight)
            {
                HapticsManager.instance.TriggerHaptic(false, 0.1f, currentFireballRight.GetComponent<Fireball>().currentScale);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}