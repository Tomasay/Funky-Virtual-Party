using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;

public class ShootoutGameVRPlayerController : VRPlayerController
{
    [SerializeField] GameObject fireballPrefab, fireballHandAnchorLeft, fireballHandAnchorRight;
    [SerializeField] GameObject handFireEffectLeft, handFireEffectRight;

    public float fireballThrowPower = 1, handOffset = 0.05f;

    private GameObject currentFireballLeft, currentFireballRight;

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

        HapticsManager.instance.TriggerHaptic(hand.left, 99);
        if (hand.left)
        {
            handFireEffectLeft.SetActive(true);
        }
        else
        {
            handFireEffectRight.SetActive(true);
        }
    }

    private void OnRelease(Hand hand, Grabbable grabbable)
    {
        if (hand.left)
        {
            currentFireballLeft.GetComponent<Fireball>().OnDrop();
        }
        else
        {
            currentFireballRight.GetComponent<Fireball>().OnDrop();
        }

        HapticsManager.instance.StopHaptics(hand.left);
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
        if (isLeftHand)
        {
            currentFireballLeft = Instantiate(fireballPrefab);
            currentFireballLeft.transform.localPosition = fireballHandAnchorLeft.transform.position;
            currentFireballLeft.transform.localRotation = Quaternion.identity;
            UnityEngine.Animations.ConstraintSource src = new UnityEngine.Animations.ConstraintSource();
            src.sourceTransform = fireballHandAnchorLeft.transform;
            src.weight = 1;
            currentFireballLeft.GetComponent<Fireball>().constraint.AddSource(src);
            currentFireballLeft.GetComponent<Fireball>().constraint.constraintActive = true;
        }
        else
        {
            currentFireballRight = Instantiate(fireballPrefab);
            currentFireballRight.transform.localPosition = fireballHandAnchorRight.transform.position;
            currentFireballRight.transform.localRotation = Quaternion.identity;
            UnityEngine.Animations.ConstraintSource src = new UnityEngine.Animations.ConstraintSource();
            src.sourceTransform = fireballHandAnchorRight.transform;
            src.weight = 1;
            currentFireballRight.GetComponent<Fireball>().constraint.AddSource(src);
            currentFireballRight.GetComponent<Fireball>().constraint.constraintActive = true;
        }
    }
}