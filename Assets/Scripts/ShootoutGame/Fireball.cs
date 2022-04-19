using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    [SerializeField] GameObject fireball;
    [SerializeField] Collider col;
    [SerializeField] Rigidbody rb;

    private bool hasExploded;

    void Update()
    {
        if(hasExploded && explosion.isStopped)
        {
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Terrain>())
        {
            TriggerExplosion();
            Debug.Log("Colliding with floor");
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
}