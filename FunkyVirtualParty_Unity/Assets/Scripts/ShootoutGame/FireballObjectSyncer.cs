using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Animations;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class FireballObjectSyncer : GrabbableObjectSyncer
{
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void TriggerHaptic(int hapticTime);
#endif

    private bool isActive, isBoosted, isMini;
    private float currentScale;
    
    [SerializeField] GameObject fireballHandAnchorLeft, fireballHandAnchorRight;
    [SerializeField] GameObject fireballMesh, fireballTrail;
    [SerializeField] ParticleSystem mainFireball, explosion, smokePuff, ember, fireTrail;
    [SerializeField] float minSize, maxSize, fireballGrowSpeed;
    [SerializeField] Color minColor, maxColor;
    [SerializeField] Color emberColor, emberColorBoosted;
    [SerializeField] Color boostedMainColor, boostedSecondaryColor;

    [SerializeField] SpriteRenderer indicator;
    private int maxIndicatorDistance = 4;
    private int fireballExplosionRange = 1;

    private bool lastActiveSent; //Value of isActive last sent to clients

    protected override void Awake()
    {
#if UNITY_ANDROID
        grabbable.onRelease.AddListener(OnDrop);
#endif

#if UNITY_WEBGL
        if (ClientManagerWeb.instance)
        {
            ClientManagerWeb.instance.Manager.Socket.On<byte[]>("ObjectDataToClient", ReceiveData);
            ClientManagerWeb.instance.Manager.Socket.On<string, byte>("MethodCallToClientByte", MethodCalledFromServer);
            ClientManagerWeb.instance.Manager.Socket.On<string, byte[]>("MethodCallToClientByteArray", MethodCalledFromServer);
        }
#endif
    }

    void Update()
    {
#if UNITY_WEBGL
        if (isActive)
        {
            //Check to see if above terrain
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxIndicatorDistance))
            {
                if (hit.transform.TryGetComponent<Terrain>(out Terrain ter) || hit.transform.gameObject.name.Contains("Chunk"))
                {
                    //Enable indicator
                    indicator.enabled = true;
                    indicator.transform.position = hit.point + new Vector3(0, 0.1f, 0);
                    indicator.transform.rotation = Quaternion.Euler(Quaternion.identity.eulerAngles + new Vector3(-90, 0, 0));

                    //Color and size
                    float t = hit.distance / maxIndicatorDistance;
                    indicator.transform.localScale = Vector3.one * Mathf.Lerp(0.25f, 2.5f, t);
                    indicator.color = Color.Lerp(Color.red, Color.yellow, t);
                }
                else
                {
                    indicator.enabled = false;
                }
            }
            else
            {
                indicator.enabled = false;
            }

            if (!isDropped && !isMini)
            {
                currentScale = Mathf.Lerp(currentScale, 1, fireballGrowSpeed * Time.deltaTime);
            }

            //Scale
            float s = Mathf.Lerp(minSize, maxSize, currentScale);
            Vector3 scale = new Vector3(s, s, s);
            fireballMesh.transform.localScale = scale;
            explosion.transform.localScale = scale;
            smokePuff.transform.localScale = scale;

            //Colors
            mainFireball.startColor = isBoosted ? boostedMainColor : Color.Lerp(minColor, maxColor, currentScale);
            fireTrail.startColor = isBoosted ? boostedSecondaryColor : emberColor;
            ember.startColor = isBoosted ? emberColorBoosted : emberColor;
        }
        else
        {
            indicator.enabled = false;
        }
#endif
    }

#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, byte id)
    {
        if (id == objectID)
        {
            if (methodName.Equals("SmokePuffEvent"))
            {
                smokePuff.Play();

                Reset();
            }
            else if (methodName.Equals("FireballExplosionEvent"))
            {
                explosion.Play();

                ShootoutGameClientPlayer sp = (ShootoutGameClientPlayer)ClientManagerWeb.instance.LocalPlayer;
                sp.CheckCollisionWithFireball(transform.position, Mathf.Max(2, currentScale * fireballExplosionRange) );

                Reset();

                TriggerHaptic(200);
            }
            else if (methodName.Equals("FireballActivateLeft"))
            {
                Activate();
                EnableConstraint(true);
            }
            else if (methodName.Equals("FireballActivateRight"))
            {
                Activate();
                EnableConstraint(false);
            }
            else if (methodName.Equals("FireballActivateMini"))
            {
                isMini = true;
                Activate();
                currentScale = 0.5f;
            }
            else if (methodName.Equals("FireballBoost"))
            {
                isBoosted = true;
            }
        }
    }

    private void Activate()
    {
        isActive = true;
        fireballMesh.SetActive(true);
        if (!fireballTrail.activeSelf)
        {
            StartCoroutine("ActivateTrailDelayed", 0.5f);
        }
    }

    private void EnableConstraint(bool isLeft)
    {
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = isLeft ? fireballHandAnchorLeft.transform : fireballHandAnchorRight.transform;
        src.weight = 1;

        if (constraint.sourceCount > 0)
        {
            constraint.SetSource(0, src);
        }
        else
        {
            constraint.AddSource(src);
        }
        constraint.constraintActive = true;
        constraint.enabled = true;
    }

    private void Reset()
    {
        currentScale = 0;
        rb.isKinematic = true;
        rb.useGravity = false;
        isActive = false;
        fireballMesh.SetActive(false);
        isBoosted = false;
        fireballMesh.transform.localScale = new Vector3(minSize, minSize, minSize);
        mainFireball.startColor = minColor;
        ember.startColor = emberColor;
        fireTrail.startColor = emberColor;
        isDropped = false;
        isMini = false;
    }
#endif

    IEnumerator ActivateTrailDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        fireballTrail.SetActive(true);
    }
}