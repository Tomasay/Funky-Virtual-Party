using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using Normal.Realtime;

public class Fireball : MonoBehaviour
{
    public static List<Fireball> pool;

    [SerializeField] public FireballSyncer syncer;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Canvas chargeCanvas;
    [SerializeField] public Image chargeIndicator;
    [SerializeField] public float minSize, maxSize;
    [SerializeField] float fireballGrowSpeed = 0.25f;
    [SerializeField] float minHoleScale, maxHoleScale;

    public Collider col;
    public ParentConstraint constraint;
    public bool readyToSpawn = false;
    public bool isInLeftHand, isInRightHand;

    public bool hasExploded = false, isDropped = false;
    public float maxTimeAlive = 10, timeDropped;

    private void Awake()
    {
        if(pool == null)
        {
            pool = new List<Fireball>();
        }
        pool.Add(this);

        chargeCanvas.transform.SetParent(null);
    }

    private void Start()
    {
        Reset();
    }

    private void OnDisable()
    {
        pool = null;
    }

    void Update()
    {
        chargeCanvas.transform.position = transform.position;

        if ((hasExploded && syncer.explosion.PSystem.isStopped) || (isDropped && timeDropped != 0 && Time.time - timeDropped > maxTimeAlive))
        {
            Reset();
        }

        if(syncer.IsActive && !isDropped)
        {
            syncer.CurrentScale = Mathf.Lerp(syncer.CurrentScale, 1, fireballGrowSpeed * Time.deltaTime);
            SetScale();

            chargeIndicator.color = Color.Lerp(syncer.minColor, syncer.maxColor, syncer.CurrentScale);
            chargeIndicator.fillAmount = syncer.CurrentScale / syncer.scaleForBoosted;

            if (!syncer.IsBoosted && syncer.CurrentScale > syncer.scaleForBoosted)
            {
                syncer.IsBoosted = true;
                syncer.boostedParticleEffect.Play();

                chargeIndicator.enabled = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Equals("Iceberg_Collider"))
        {
            Vector3 point = collision.GetContact(0).point;

            TriggerIcebergHole(point);
            TriggerExplosion();
        }
        else if(collision.gameObject.name.Contains("IceTower"))
        {
            TriggerSmokePuff();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Water") || other.name.Equals("HoleVolume"))
        {
            TriggerSmokePuff();
        }
    }

    private void TriggerSmokePuff()
    {
        syncer.smokePuff.Play();
        Reset();
    }

    private void TriggerExplosion()
    {
        syncer.IsActive = false;
        col.enabled = false;
        syncer.ExplosionTrigger = true;
        syncer.explosion.Play();
        hasExploded = true;

        if(syncer.IsBoosted)
        {
            //Get three available fireballs
            int spawnedCount = 0;
            foreach (Fireball f in pool)
            {
                if(f.readyToSpawn)
                {
                    //if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByte", "FireballActivateMini", f.objectSyncer.objectID);
#if UNITY_ANDROID
                    //objectSyncer.ManualSendTrajectory();
#endif

                    //Set pos to current explosion
                    f.transform.position = transform.position + Vector3.up;

                    //Enable them and set scale
                    f.readyToSpawn = false;
                    f.Activate();
                    f.OnDrop();
                    
                    f.syncer.CurrentScale = 0.5f;
                    f.SetScale();

                    //Launch them outwards
                    switch (spawnedCount)
                    {
                        case 0:
                            f.rb.AddForce(-175, 200, 0);
                            break;
                        case 1:
                            f.rb.AddForce(175, 250, 0);
                            break;
                        case 2:
                            f.rb.AddForce(0, 300, 175);
                            break;
                        default:
                            break;
                    }

                    StartCoroutine(SetColliderDelayed(f, true, 1));

                    spawnedCount++;
                }
                if(spawnedCount >= 3)
                {
                    break;
                }
            }
        }

        /*
        if (ClientManager.instance)
        {
            //ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByte", "FireballExplosionEvent", objectSyncer.objectID);

#if UNITY_EDITOR
            foreach (ClientPlayer cp in ClientManager.instance.Players)
            {
                if (cp.isDebugPlayer)
                {
                    ShootoutGameClientPlayer sp = (ShootoutGameClientPlayer)cp;
                    sp.CheckCollisionWithFireball(transform.position, Mathf.Max(2, syncer.CurrentScale));
                }
            }
#endif
        }
        */
    }

    void TriggerIcebergHole(Vector3 pos)
    {
        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = true;
        GameObject newHole = Realtime.Instantiate("IcebergHole", pos, Quaternion.identity, options);

        float holeSize = Mathf.Lerp(minHoleScale, maxHoleScale, syncer.CurrentScale);
        newHole.transform.localScale = new Vector3(holeSize, holeSize, holeSize);
    }

    IEnumerator SetColliderDelayed(Fireball f, bool enabled, float delay)
    {
        yield return new WaitForSeconds(delay);
        f.col.enabled = enabled;
    }

    public void SetScale()
    {
        float s = Mathf.Lerp(minSize, maxSize, syncer.CurrentScale);

        Vector3 scale = new Vector3(s, s, s);
        syncer.fireball.transform.localScale = scale;
        syncer.explosion.transform.localScale = scale;
        syncer.smokePuff.transform.localScale = scale;
    }

    public void Activate()
    {
        syncer.IsActive = true;
        readyToSpawn = false;
    }

    public void OnDrop()
    {
        timeDropped = Time.time;
        isDropped = true;
        isInLeftHand = false;
        isInRightHand = false;
        chargeIndicator.enabled = false;
    }

    private void Reset()
    {
        syncer.CurrentScale = 0;
        syncer.fireball.transform.localScale = new Vector3(minSize, minSize, minSize);
        syncer.mainFireball.StartColor = syncer.minColor;
        syncer.ember.StartColor = syncer.emberColor;
        syncer.fireTrail.StartColor = syncer.emberColor;
        syncer.IsActive = false;
        hasExploded = false;
        isDropped = false;
        syncer.IsBoosted = false;
        chargeIndicator.fillAmount = 0;
        chargeIndicator.enabled = true;
        readyToSpawn = true;
    }
}