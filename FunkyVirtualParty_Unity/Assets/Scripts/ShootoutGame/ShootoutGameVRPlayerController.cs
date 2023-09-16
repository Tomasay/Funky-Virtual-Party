using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;
using TMPro;

public class ShootoutGameVRPlayerController : VRPlayerController
{
    [SerializeField] GameObject fireballPrefab, fireballHandAnchorLeft, fireballHandAnchorRight;
    [SerializeField] GameObject handFireEffectLeft, handFireEffectRight;

    public TMP_Text vrInfoText, vrGameTimeText;

    public float fireballThrowPower = 1, handOffset = 0.05f;

    private GameObject currentFireballLeft, currentFireballRight;

    private bool isGrabbingLeft, isGrabbingRight;

    public bool canThrowFireballs = false;

    void Awake()
    {
        ahp.handRight.OnTriggerGrab += OnGrabbed;
        ahp.handRight.OnTriggerRelease += OnRelease;
        ahp.handLeft.OnTriggerGrab += OnGrabbed;
        ahp.handLeft.OnTriggerRelease += OnRelease;
    }

    private void Start()
    {
        PreloadFireball(true);
        PreloadFireball(false);
    }

    private void OnGrabbed(Hand hand, Grabbable grabbable)
    {
        if (!canThrowFireballs)
            return;

        Fireball fireball = hand.left ? currentFireballLeft.GetComponent<Fireball>() : currentFireballRight.GetComponent<Fireball>();
        fireball.col.enabled = true;
        fireball.constraint.enabled = false;
        fireball.rb.isKinematic = false;

        Vector3 dir = (fireball.transform.position - hand.transform.position).normalized;
        Physics.Raycast(hand.transform.position, dir, out RaycastHit hit, 10);

        if (hit.collider.TryGetComponent<Grabbable>(out Grabbable g))
        {
            hand.Grab(hit, g);
        }

        //hand.TryGrab(hand.left ? currentFireballLeft.GetComponent<Grabbable>() : currentFireballRight.GetComponent<Grabbable>());

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
        Fireball nextFireball = Fireball.pool[0];
        for (int i = 0; i < Fireball.pool.Count; i++)
        {
            if(Fireball.pool[i].readyToSpawn)
            {
                nextFireball = Fireball.pool[i];
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

    void SetupCollisionIgnore()
    {
        foreach (Fireball f in Fireball.pool)
        {
            Physics.IgnoreCollision(f.col, ahp.headModel.GetComponent<Collider>());
            Physics.IgnoreCollision(f.col, ahp.handLeft.GetComponent<Collider>());
            Physics.IgnoreCollision(f.col, ahp.handRight.GetComponent<Collider>());
            foreach (Finger fingy in ahp.handLeft.fingers)
            {
                Physics.IgnoreCollision(f.col, fingy.GetComponent<Collider>());
            }
            foreach (Finger fingy in ahp.handRight.fingers)
            {
                Physics.IgnoreCollision(f.col, fingy.GetComponent<Collider>());
            }
            foreach (ClientPlayer cp in ClientPlayer.clients)
            {
                ShootoutGameClientPlayer scp = cp.gameObject.GetComponent<ShootoutGameClientPlayer>();
                if (scp != null)
                {
                    Physics.IgnoreCollision(f.col, scp.Col);
                }
                
            }
        }
    }

    IEnumerator TriggerFireballHaptics(bool isLeft)
    {
        if (isLeft)
        {
            while(isGrabbingLeft)
            {
                HapticsManager.instance.TriggerHaptic(true, 0.1f, currentFireballLeft.GetComponent<Fireball>().syncer.CurrentScale);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            while (isGrabbingRight)
            {
                HapticsManager.instance.TriggerHaptic(false, 0.1f, currentFireballRight.GetComponent<Fireball>().syncer.CurrentScale);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}