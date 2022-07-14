using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Fireball : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion, smokePuff;
    [SerializeField] public GameObject fireball;
    [SerializeField] public Rigidbody rb;
    [SerializeField] float minSize, maxSize;
    [SerializeField] float fireballGrowSpeed = 0.25f;
    [SerializeField] FireballObjectSyncer syncer;

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
            float s = Mathf.Lerp(minSize, maxSize, currentScale);
            Vector3 scale = new Vector3(s, s, s);
            fireball.transform.localScale = scale;
            explosion.transform.localScale = scale;
            smokePuff.transform.localScale = scale;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>() || collision.gameObject.name.Contains("ChunkObject"))
        {
            TriggerExplosion();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Water"))
        {
            smokePuff.Play();
            Reset();
        }
    }

    private void TriggerExplosion()
    {
        fireball.SetActive(false);
        col.enabled = false;
        rb.isKinematic = true;
        explosion.Play();
        hasExploded = true;

        ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "FireballExplosionEvent", syncer.CurrentFireballData.objectID.ToString());
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
        rb.isKinematic = true;
        rb.useGravity = false;
        fireball.SetActive(false);
        hasExploded = false;
        isDropped = false;
        //constraint.constraintActive = false;

        readyToSpawn = true;
    }
}