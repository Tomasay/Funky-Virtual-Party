using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class ShootoutGameClientPlayer : ClientPlayer
{
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void TriggerHaptic(int hapticTime);
#endif

    class WaterSplashData
    {
        public Vector3 splashPos; //Where the splash should display
        public string playerID; //Player who died
    }

    public UnityEvent OnDeath;
    public bool isColliding = false;
    float collisionTimer = 0.5f;
    const float collisionTimerDefault = 1;
    Vector2 collisionVector;

    const float frictionCoefficient = 0.015f;

    public Camera cam;

    [SerializeField] ParticleSystem waterSplash, iceTrail;

    [SerializeField] GameObject iceCube;


    public bool isAlive = true;

    protected override void Awake()
    {
        startingSpeed = 1.5f;

        base.Awake();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif
    }

#if !UNITY_WEBGL
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Water"))
        {
            Vector3 pos = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            SpawnSplashEffect(pos);
            SetPlayerActive(false);
            TriggerIceCubeAnimation();
            isAlive = false;
            OnDeath.Invoke();

            WaterSplashData data = new WaterSplashData();
            data.splashPos = pos;
            data.playerID = PlayerSocketID;
            if(ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "WaterSplashEvent", JsonUtility.ToJson(data));
        }
    }
#endif

    public Vector2 prevVector = Vector2.zero;
    protected override void Update()
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

            collisionTimer = collisionTimerDefault;
        }
        else if(isDebugPlayer && isColliding)
        {
            ClientManager.instance.Manager.Socket.Emit("inputDebug", collisionVector.normalized.x * -1.25f, collisionVector.normalized.y * -1.25f, PlayerByteID);

            collisionTimer -= Time.deltaTime;
            if (collisionTimer <= 0)
            {
                isColliding = false;
            }
        }
#endif
        if (IsLocal && !isColliding) //Only read values from analog stick, and emit movement if being done from local device
        {
            // in this game we want the player to feel like they're on ice, so we calculate a low fricition 
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            if (prevVector == Vector2.zero)
                prevVector = input;

            Vector2 delta = (input - prevVector) * frictionCoefficient;
            Vector2 target = prevVector + delta;
            prevVector = target;

            if (!(target == Vector2.zero && movement == Vector3.zero)) //No need to send input if we're sending 0 and we're already not moving
            {
                ClientManagerWeb.instance.Manager.Socket.Emit("input", target.x, target.y, PlayerByteID);
            }

            Move(target.x, target.y);

            Vector3 positionDifference = posFromHost - transform.position;
            transform.Translate((movement + positionDifference / 4) * Time.deltaTime);
            collisionTimer = collisionTimerDefault;

            CheckIceTrailVisibility();
        }
        else if (IsLocal && isColliding)
        {
            ClientManagerWeb.instance.Manager.Socket.Emit("input", collisionVector.normalized.x * -1.25f, collisionVector.normalized.y * -1.25f, PlayerByteID);
            Move(collisionVector.normalized.x * -1.25f, collisionVector.normalized.y * -1.25f, false);

            Vector3 positionDifference = posFromHost - transform.position;   
            transform.Translate((movement + positionDifference / 4) * Time.deltaTime);

            collisionTimer -= Time.deltaTime;
            if (collisionTimer <= 0)
            {
                isColliding = false;
            }
        }
        else
        { 
            transform.Translate(movement * Time.deltaTime);

            CheckIceTrailVisibility();
        }


#if UNITY_EDITOR
        if (isDebugPlayer && isExplosion)
        {
            ClientManager.instance.Manager.Socket.Emit("inputDebug", collisionVector.normalized.x * -2, collisionVector.normalized.y * -2, PlayerByteID);

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
            ClientManagerWeb.instance.Manager.Socket.Emit("input", collisionVector.normalized.x * -2, collisionVector.normalized.y * -2, PlayerByteID);
            Move(collisionVector.normalized.x * -2, collisionVector.normalized.y * -2, false);

            Vector3 positionDifference = posFromHost - transform.position;
            transform.Translate((movement + positionDifference / 4) * Time.deltaTime); 

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
        // check if we are below the floor
        if(transform.position.y < -10 && transform.position.y != posFromHost.y)
        {
            transform.position = posFromHost;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        anim.transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, Time.deltaTime);

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
            collisionVector = Vector2.Reflect(prevVector, collision.contacts[0].normal );
        }
    }

#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("WaterSplashEvent"))
        {
            //Trigger splash visual
            WaterSplashData newData = JsonUtility.FromJson<WaterSplashData>(data);
            SpawnSplashEffect(newData.splashPos);

            //Kill corresponding player
            ShootoutGameClientPlayer player = ClientManagerWeb.instance.GetPlayerBySocketID(newData.playerID) as ShootoutGameClientPlayer;
            player.SetPlayerActive(false);
            player.isAlive = false;

            player.TriggerIceCubeAnimation();

            TriggerHaptic(500);
        }
    }
#endif

    void SpawnSplashEffect(Vector3 collisionPoint)
    {
        ParticleSystem ps = Instantiate(waterSplash, collisionPoint, Quaternion.identity);
        ps.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        waterSplash.Play();
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
    [SerializeField] int jumpCooldown = 2;
    float timeJumped = 0;
    public override void Action()
    {
        if (CanMove && timeJumped == 0 || (Time.time - timeJumped) > jumpCooldown)
        {
            timeJumped = Time.time;
            anim.SetTrigger("Jump");

            // Jump!
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }
    }

    public void TriggerIceCubeAnimation()
    {
        iceCube.SetActive(true);

        playerNameText.enabled = false;

        Col.enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        smr.enabled = true;

        CanMove = false;

        anim.SetTrigger("StandingPose");

        //Put player under water
        transform.position = new Vector3(transform.position.x, 24.5f, transform.position.z);

        StartCoroutine("BringToTop");
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