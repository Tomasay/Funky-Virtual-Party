using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.UI;

public class ShootoutGameVRPlayerController : VRPlayerController
{
    [SerializeField] GameObject fireballPrefab, fireballHandAnchorLeft, fireballHandAnchorRight;

    public float fireballThrowPower = 1, handOffset = 0.05f;

    private GameObject currentFireball;

    void Awake()
    {
        ahp.handRight.OnSqueezed += SpawnFireBall;
        ahp.handRight.OnUnsqueezed += ReleaseFireBall;
    }

    private void SpawnFireBall(Hand hand, Grabbable grabbable)
    {
        currentFireball = Instantiate(fireballPrefab);
        currentFireball.transform.parent = hand.left ? fireballHandAnchorLeft.transform : fireballHandAnchorRight.transform;
        currentFireball.transform.localPosition = Vector3.zero;
        currentFireball.transform.localRotation = Quaternion.identity;
        currentFireball.GetComponent<Rigidbody>().isKinematic = true;
        currentFireball.GetComponent<Collider>().enabled = false;
    }

    private void ReleaseFireBall(Hand hand, Grabbable grabbable)
    {
        currentFireball.transform.parent = null;
        currentFireball.GetComponent<Rigidbody>().isKinematic = false;
        currentFireball.GetComponent<Rigidbody>().velocity = hand.GetComponent<Rigidbody>().velocity * fireballThrowPower;
        StartCoroutine("EnableFireballCollider", currentFireball);
    }

    IEnumerator EnableFireballCollider(GameObject fireball)
    {
        yield return new WaitForSeconds(0.5f);
        fireball.GetComponent<Collider>().enabled = true;
    }
}