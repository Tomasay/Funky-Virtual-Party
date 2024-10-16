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
    
    [SerializeField] GameObject fireballMesh, fireballTrail;
    [SerializeField] ParticleSystem mainFireball, explosion, smokePuff, ember, fireTrail;
    [SerializeField] float minSize, maxSize, fireballGrowSpeed;
    [SerializeField] Color minColor, maxColor;
    [SerializeField] Color emberColor, emberColorBoosted;
    [SerializeField] Color boostedMainColor, boostedSecondaryColor;

    [SerializeField] SpriteRenderer indicator;
    private int maxIndicatorDistance = 4;
    private int fireballExplosionRange = 1;

    [SerializeField] GameObject icebergHolePrefab;
    [SerializeField] float minHoleScale, maxHoleScale;

    private bool lastActiveSent; //Value of isActive last sent to clients

    protected override void Awake()
    {
#if UNITY_ANDROID
        grabbable.onRelease.AddListener(OnDrop);
        grabbable.onGrab.AddListener(OnGrab);
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

    private void OnDisable()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("ObjectDataToClient");
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClientByte");
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClientByteArray");
#endif
    }

    void Update()
    {
#if UNITY_WEBGL
        if (isActive)
        {
            //Check to see if above iceberg
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxIndicatorDistance))
            {
                if (hit.transform.gameObject.name.Contains("Iceberg_Collider"))
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
    protected override void MethodCalledFromServer(string methodName, byte data)
    {
        base.MethodCalledFromServer(methodName, data);

        if (data == objectID)
        {
            if (methodName.Equals("SmokePuffEvent"))
            {
                smokePuff.Play();

                Reset();
            }
            else if (methodName.Equals("FireballExplosionEvent"))
            {
                explosion.Play();

                TriggerIcebergHole(transform.position);

                ShootoutGameClientPlayer sp = (ShootoutGameClientPlayer)ClientManagerWeb.instance.LocalPlayer;
                sp.CheckCollisionWithFireball(transform.position, Mathf.Max(2, currentScale * fireballExplosionRange) );

                Reset();

                TriggerHaptic(200);
            }
            else if (methodName.Equals("OnGrabLeft"))
            {
                Activate();
            }
            else if (methodName.Equals("OnGrabRight"))
            {
                Activate();
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

    /// <summary>
    /// Creates a prefab for a hole in the iceberg
    /// </summary>
    /// <param name="pos">Position of collision contact point</param>
    /// <param name="scale">Scale of the current fireball. For WebGL, no need to input this parameter as it will use local scale. For VR, must override</param>
    public void TriggerIcebergHole(Vector3 pos, float scale = -1)
    {
        GameObject newHole = Instantiate(icebergHolePrefab, pos, Quaternion.identity);

        float holeSize = Mathf.Lerp(minHoleScale, maxHoleScale, (scale == -1) ? currentScale : scale);
        newHole.transform.localScale = new Vector3(holeSize, holeSize, holeSize);
    }

    IEnumerator ActivateTrailDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        fireballTrail.SetActive(true);
    }
}