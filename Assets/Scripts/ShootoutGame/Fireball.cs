using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Fireball : MonoBehaviour
{
    public static List<Fireball> pool;

    [SerializeField] ParticleSystem mainFireball, explosion, smokePuff, ember, fireTrail;
    [SerializeField] public GameObject fireball;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Image chargeIndicator;
    [SerializeField] public float minSize, maxSize;
    [SerializeField] Color minColor, maxColor;
    [SerializeField] Color emberColor, emberColorBoosted;
    [SerializeField] float fireballGrowSpeed = 0.25f;
    [SerializeField] FireballObjectSyncer syncer;
    [SerializeField] Shootout_DestructibleTerrian terrain;

    [SerializeField] Color boostedMainColor, boostedSecondaryColor;
    [SerializeField] ParticleSystem boostedParticleEffect;
    public bool boosted = false;
    private float scaleForBoosted = 0.8f; //What level currentScale has to be for fireball to become boosted

    public Collider col;
    public ParentConstraint constraint;
    public float currentScale = 0; //Between 0 and 1, indicates level between min and max size
    public bool readyToSpawn = false;
    public bool isInLeftHand, isInRightHand;

    public bool hasExploded = false, isDropped = false;
    public float maxTimeAlive = 10, timeDropped;

    private void Awake()
    {
        Reset();

        if(pool == null)
        {
            pool = new List<Fireball>();
        }
        pool.Add(this);
    }

    void Update()
    {
        if((hasExploded && explosion.isStopped) || (isDropped && timeDropped != 0 && Time.time - timeDropped > maxTimeAlive))
        {
            Reset();
        }

        if(fireball.activeSelf && !isDropped)
        {
            currentScale = Mathf.Lerp(currentScale, 1, fireballGrowSpeed * Time.deltaTime);
            SetScale();

            chargeIndicator.color = Color.Lerp(minColor, maxColor, currentScale);
            chargeIndicator.fillAmount = currentScale / scaleForBoosted;

            if (!boosted && currentScale > scaleForBoosted)
            {
                //Level up fireball
                boostedParticleEffect.Play();

                mainFireball.startColor = boostedMainColor;

                ember.startColor = emberColorBoosted;

                fireTrail.startColor = boostedSecondaryColor;

                boosted = true;

                chargeIndicator.enabled = false;
            }
            else if(!boosted)
            {
                mainFireball.startColor = Color.Lerp(minColor, maxColor, currentScale);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>() || collision.gameObject.name.Contains("ChunkObject"))
        {
            terrain.Explosion(collision, this);
            TriggerExplosion();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Water"))
        {
            smokePuff.Play();
            Reset();

            if (ClientManager.instance)
            {
                ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "SmokePuffEvent", syncer.CurrentFireballData.objectID.ToString());
            }
        }
    }

    private void TriggerExplosion()
    {
        fireball.SetActive(false);
        col.enabled = false;
        rb.isKinematic = true;
        explosion.Play();
        hasExploded = true;

        if(boosted)
        {
            //Get three available fireballs
            int spawnedCount = 0;
            foreach (Fireball f in pool)
            {
                if(f.readyToSpawn)
                {
                    //Set pos to current explosion
                    f.transform.position = transform.position + (Vector3.up*2);

                    //Enable them and set scale
                    f.readyToSpawn = false;
                    f.Activate();
                    f.OnDrop();
                    
                    f.currentScale = 0.5f;
                    f.SetScale();

                    //Launch them outwards
                    switch (spawnedCount)
                    {
                        case 0:
                            f.rb.AddForce(-250, 400, 0);
                            break;
                        case 1:
                            f.rb.AddForce(250, 500, 0);
                            break;
                        case 2:
                            f.rb.AddForce(0, 600, 250);
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

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "FireballExplosionEvent", syncer.CurrentFireballData.objectID.ToString());
        }
    }

    IEnumerator SetColliderDelayed(Fireball f, bool enabled, float delay)
    {
        yield return new WaitForSeconds(delay);
        f.col.enabled = enabled;
    }

    public void SetScale()
    {
        float s = Mathf.Lerp(minSize, maxSize, currentScale);

        Vector3 scale = new Vector3(s, s, s);
        fireball.transform.localScale = scale;
        explosion.transform.localScale = scale;
        smokePuff.transform.localScale = scale;
    }

    public void Activate()
    {
        fireball.SetActive(true);
        rb.isKinematic = false;
        rb.useGravity = true;
        readyToSpawn = false;
    }

    public void OnDrop()
    {
        timeDropped = Time.time;
        isDropped = true;
        isInLeftHand = false;
        isInRightHand = false;
    }

    private void Reset()
    {
        currentScale = 0;
        fireball.transform.localScale = new Vector3(minSize, minSize, minSize);
        mainFireball.startColor = minColor;
        ember.startColor = emberColor;
        fireTrail.startColor = emberColor;
        rb.isKinematic = true;
        rb.useGravity = false;
        fireball.SetActive(false);
        hasExploded = false;
        isDropped = false;
        boosted = false;
        chargeIndicator.fillAmount = 0;
        chargeIndicator.enabled = true;
        //constraint.constraintActive = false;

        readyToSpawn = true;
    }
}