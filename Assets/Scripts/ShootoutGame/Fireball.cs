using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Fireball : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject fireball;
    [SerializeField] Rigidbody rb;
    [SerializeField] float minSize, maxSize;

    public Collider col;
    public ParentConstraint constraint;
    public float currentScale = 0; //Between 0 and 1, indicates level between min and max size
    public bool readyToSpawn = false;

    private bool hasExploded = false, isDropped = false;
    private float maxTimeAlive = 10, timeDropped;
    private Material fireballMat;

    private void Awake()
    {
        fireballMat = fireball.GetComponent<Renderer>().material;

        Reset();
    }

    void Update()
    {
        if((hasExploded && explosion.isStopped) || (timeDropped != 0 && Time.time - timeDropped > maxTimeAlive))
        {
            Reset();
        }

        if(fireball.activeSelf && !isDropped)
        {
            currentScale = Mathf.Lerp(currentScale, 1, 0.5f * Time.deltaTime);
            float s = Mathf.Lerp(minSize, maxSize, currentScale);
            fireball.transform.localScale = new Vector3(s, s, s);

            //Mat effect
            fireballMat.SetVector("Flame_fre_pow_burn_amount", new Vector4(0, currentScale, 0, 0));
            fireballMat.SetFloat("fresnel_power", Mathf.Lerp(20, 5, currentScale));
            Color fireColor = fireballMat.GetColor("BaseColor");
            fireColor.a = currentScale;
            fireballMat.SetColor("BaseColor", fireColor);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>() || collision.gameObject.name.Contains("ChunkObject"))
        {
            TriggerExplosion();
        }
        else if (collision.gameObject.name.Contains("Water"))
        {
            //TODO: Trigger smoke burnout effect
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