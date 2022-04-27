using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Fireball : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject fireball;
    [SerializeField] Rigidbody rb;

    public Collider col;
    public ParentConstraint constraint;

    private bool hasExploded;
    private float maxTimeAlive = 10, timeDropped;

    void Update()
    {
        if((hasExploded && explosion.isStopped) || (timeDropped != 0 && Time.time - timeDropped > maxTimeAlive))
        {
            Destroy(gameObject);
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
    }
}