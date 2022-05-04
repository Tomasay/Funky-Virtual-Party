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

    private bool hasExploded = false, isDropped = false;
    private float maxTimeAlive = 10, timeDropped;
    private Material fireballMat;

    private void Awake()
    {
        fireball.transform.localScale = new Vector3(minSize, minSize, minSize);
        fireball.SetActive(false);
        fireballMat = fireball.GetComponent<Renderer>().material;
    }

    void Update()
    {
        if((hasExploded && explosion.isStopped) || (timeDropped != 0 && Time.time - timeDropped > maxTimeAlive))
        {
            Destroy(gameObject);
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
    }

    public void OnDrop()
    {
        timeDropped = Time.time;
        isDropped = true;
    }
}