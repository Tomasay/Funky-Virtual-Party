using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using DG.Tweening;
using Normal.Realtime;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class ShootoutGameClientPlayer : ClientPlayer
{
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void TriggerHaptic(int hapticTime);
#endif
    public bool isColliding = false;
    float collisionTimer = 0.5f;
    const float collisionTimerDefault = 0.5f;
    Vector2 collisionVector;

    private SerializedVector3 splashPos;

    const float frictionCoefficient = 0.1f;

    public Camera cam;

    [SerializeField] ParticleSystem iceTrail;

    [SerializeField] GameObject iceCube;


    public bool isAlive = true;

    protected override void Awake()
    {
        syncer.OnDeath.AddListener(OnPlayerDeath);

        base.Awake();

        startingSpeed = 1;
    }

#if UNITY_WEBGL
    private void OnTriggerEnter(Collider other)
    {
        if (isLocal)
        {
            if (other.tag.Equals("Water"))
            {
                TriggerIceCubeAnimation(transform.position);
            }
            else if (other.gameObject.name.Equals("IcebergHole"))
            {
                TriggerIceCubeAnimation(other.gameObject.transform.position);
            }
        }
    }
#endif

    public Vector2 prevVector = Vector2.zero;
    protected override void CheckInput()
    {
#if UNITY_EDITOR
        if (isDebugPlayer && !isColliding)
        {
            Vector2 input = new Vector2(movement.x, movement.z);

            if (prevVector == Vector2.zero)
                prevVector = input;

            Vector2 delta = (input - prevVector) * frictionCoefficient;
            Vector2 target = prevVector + delta;
            prevVector = target;
        }
        else if(isDebugPlayer && isColliding)
        {
            //ClientManager.instance.Manager.Socket.Emit("inputDebug", SerializeInputData(collisionVector.normalized * -0.5f));

            collisionTimer -= Time.deltaTime;
            if (collisionTimer <= 0)
            {
                isColliding = false;
                collisionTimer = collisionTimerDefault;
            }
        }
#endif
        if (IsLocal && !isColliding) //Only read values from analog stick, and emit movement if being done from local device
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            //Friction
            if (prevVector == Vector2.zero)
                prevVector = input;

            Vector2 delta = (input - prevVector) * frictionCoefficient;
            Vector2 target = prevVector + delta;
            prevVector = target;

            if (!(target == Vector2.zero && movement == Vector3.zero)) //No need to send input if we're sending 0 and we're already not moving
            {
                //ClientManagerWeb.instance.Manager.Socket.Emit("IS", SerializeInputData(target));

                bool isSliding = (input == Vector2.zero && target.magnitude > 0);
                Move(target, !isSliding, !isSliding, input);
            }

            CheckIceTrailVisibility();
        }
        else if (IsLocal && isColliding)
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            //Collision force starts off strong and fades off, input influence starts low and comes back to normal
            float t = (collisionTimer > 0) ? (collisionTimer / collisionTimerDefault) : 0;
            Vector2 collisionInput = prevVector = (input * (1-t)) + (-0.75f * t * collisionVector.normalized);

            //ClientManagerWeb.instance.Manager.Socket.Emit("IS", SerializeInputData(collisionInput));
            Move(collisionInput, (input == Vector2.zero) ? false : true, true, input);

            collisionTimer -= Time.deltaTime;
            if (collisionTimer <= 0)
            {
                isColliding = false;
                collisionTimer = collisionTimerDefault;
            }
        }
        else
        {
            CheckIceTrailVisibility();
        }

#if UNITY_EDITOR
        if (isDebugPlayer && isExplosion)
        {
            //ClientManager.instance.Manager.Socket.Emit("inputDebug", SerializeInputData(collisionVector.normalized * -0.5f));

            explosionTimer -= Time.deltaTime;
            if (explosionTimer <= 0)
            {
                isExplosion = false;
            }
        }
        else
        {
            explosionTimer = explosionTimerDefault;
        }
#endif
        if (isLocal && isExplosion)
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            //ClientManagerWeb.instance.Manager.Socket.Emit("IS", SerializeInputData(collisionVector.normalized * -0.5f));
            Move(collisionVector.normalized * -0.5f, false, false, input);

            /*
            Vector3 positionDifference = posFromHost - transform.position;
            transform.Translate((movement + positionDifference / 4) * Time.deltaTime);
            */

            explosionTimer -= Time.deltaTime;
            if (explosionTimer <= 0)
            {
                isExplosion = false;
            }
        }
        else if(isLocal)
        {
            explosionTimer = explosionTimerDefault;
        }

#if UNITY_WEBGL
        playerNameText.transform.LookAt(2 * transform.position - cam.transform.position);
#else
        if (Camera.main)
        {
            playerNameText.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }
#endif
    }

    void CheckIceTrailVisibility()
    {
        if ((Time.time - timeJumped) < jumpCooldown)
        {
            iceTrail.Stop();
        }
        else
        {
            if (iceTrail.isPlaying && movement.magnitude < 0.1f)
            {
                iceTrail.Stop();
            }
            else if (!iceTrail.isPlaying && movement.magnitude > 0.1f)
            {
                iceTrail.Play();
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // expensive this should be optimized.
        ShootoutGameClientPlayer ext = collision.gameObject.GetComponent<ShootoutGameClientPlayer>();
        if(ext)
        {
            isColliding = true;
            collisionVector = Vector2.Reflect(prevVector - new Vector2(ext.movement.x, ext.movement.z), collision.contacts[0].normal );
        }
    }

    void SpawnSplashEffect(Vector3 collisionPoint)
    {
        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = true;
        GameObject splash = Realtime.Instantiate("Water Splash", collisionPoint, Quaternion.identity, options);
        splash.GetComponent<ParticleSystemStoppedEvent>().ParticleSystemStopped.AddListener(delegate { Realtime.Destroy(splash.gameObject); });

#if UNITY_WEBGL
        TriggerHaptic(500);
#endif
    }

    bool isExplosion = false;
    float explosionTimer = 0.25f;
    const float explosionTimerDefault = 0.25f;
    public void CheckCollisionWithFireball(Vector3 firePos, float radius)
    {
        float dist = Vector3.Distance(firePos, transform.position);
        isExplosion = dist < radius;
        if(isExplosion)
        {
            collisionVector = firePos - transform.position;
        }

    }

    // Jumping fields
    [SerializeField] private int jumpForce = 500;
    [SerializeField] float jumpCooldown = 1;
    float timeJumped = 0;
    public override void Action()
    {
        if (CanMove && (timeJumped == 0 || (Time.time - timeJumped) > jumpCooldown))
        {
            timeJumped = Time.time;
            anim.SetTrigger("Jump");

            // Jump!
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }
    }

    void OnPlayerDeath()
    {
        iceCube.SetActive(true);
        playerNameText.enabled = false;
        Col.enabled = false;

        anim.SetTrigger("FallIntoWater");
        StartCoroutine("SetStandingPose");
    }

    public void TriggerIceCubeAnimation(Vector3 holeCenterPos)
    {
        syncer.OnDeathTrigger = true;

        splashPos = new Vector3(holeCenterPos.x, 25, holeCenterPos.z);

        CanMove = false;

        transform.DOMoveX(holeCenterPos.x, 0.25f);
        transform.DOMoveZ(holeCenterPos.z, 0.25f);

        StartCoroutine("IceCubeAnimation");

        isAlive = false;
    }

    IEnumerator IceCubeAnimation()
    {
        yield return new WaitForSeconds(0.75f);

        SpawnSplashEffect(splashPos);
        SetPlayerActive(false);

        yield return new WaitForSeconds(1);

        iceCube.SetActive(true);

        playerNameText.enabled = false;

        Col.enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        CanMove = false;

        //Put player under water
        transform.position = new Vector3(transform.position.x, 24.5f, transform.position.z);

        StartCoroutine("BringToTop");
    }

    IEnumerator SetStandingPose()
    {
        yield return new WaitForSeconds(1.75f);

        smr.enabled = true;
        anim.SetTrigger("StandingPose");
    }

    IEnumerator BringToTop()
    {
        for (int i = 0; i <= 60; i++)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(24.5f, 24.9f, (float)i/60.0f), transform.position.z);
            yield return new WaitForSeconds(0.025f);
        }

        //Continually bob up and down
        InvokeRepeating("Bob", 0, 0.025f);
    }

    void Bob()
    {
        float t = Mathf.PingPong(Time.time, 1);

        transform.position = Vector3.Slerp(new Vector3(transform.position.x, 24.8f, transform.position.z), new Vector3(transform.position.x, 24.9f, transform.position.z), t);
        //transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, -5), Quaternion.Euler(0, 0, 5), t);
    }
}