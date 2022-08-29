using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootoutGameClientPlayer : ClientPlayer
{
    class WaterSplashData
    {
        public Vector3 splashPos; //Where the splash should display
        public string playerID; //Player who died
    }

    public UnityEvent OnDeath;
    public bool isColliding = false;
    float collisionTimer = 0.5f;
    const float collisionTimerDefault = 0.5f;
    Vector2 collisionVector;

    [SerializeField] ParticleSystem waterSplash, iceTrail;

    [SerializeField] GameObject iceCube;


    public bool isAlive = true;

    protected override void Awake()
    {
        startingSpeed = 10.0f;

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
            //TriggerIceCubeAnimation();
            SetPlayerActive(false);
            isAlive = false;
            OnDeath.Invoke();

            WaterSplashData data = new WaterSplashData();
            data.splashPos = pos;
            data.playerID = playerID;
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "WaterSplashEvent", JsonUtility.ToJson(data));
        }
    }
#endif

    public Vector2 prevVector = Vector2.zero;
    protected override void Update()
    {
        if (IsLocal && !isColliding) //Only read values from analog stick, and emit movement if being done from local device
        {
            // in this game we want the player to feel like they're on ice, so we calculate a low fricition 
            float frictionCoefficient = 0.015f;
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            if (prevVector == Vector2.zero)
                prevVector = input;

            Vector2 delta = (input - prevVector) * frictionCoefficient;
            Vector2 target = prevVector + delta;
            prevVector = target;

            if (!(target == Vector2.zero && movement == Vector3.zero)) //No need to send input if we're sending 0 and we're already not moving
            {
                ClientManagerWeb.instance.Manager.Socket.Emit("input", target.x, target.y);
            }

            Move(target.x, target.y);

            Vector3 positionDifference = posFromHost - transform.position;
            transform.Translate((movement + positionDifference / 4) * Time.deltaTime);
            collisionTimer = collisionTimerDefault;

            CheckIceTrailVisibility();

        }
        else if (IsLocal && isColliding)
        {
            ClientManagerWeb.instance.Manager.Socket.Emit("input", collisionVector.normalized.x, collisionVector.normalized.y);
            Move(collisionVector.normalized.x, collisionVector.normalized.y, false);

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

        if (isLocal && isExplosion)
        {
            ClientManagerWeb.instance.Manager.Socket.Emit("input", collisionVector.normalized.x * -2.5f, collisionVector.normalized.y * -2.5f );
            Move(collisionVector.normalized.x * -2.5f, collisionVector.normalized.y * -2.5f, false);

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

    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("WaterSplashEvent"))
        {
            //Trigger splash visual
            WaterSplashData newData = JsonUtility.FromJson<WaterSplashData>(data);
            SpawnSplashEffect(newData.splashPos);

            //Kill corresponding player
            ShootoutGameClientPlayer player = ClientManagerWeb.instance.GetPlayerByID(newData.playerID) as ShootoutGameClientPlayer;
            player.SetPlayerActive(false);
            player.isAlive = false;
        }
    }

    void SpawnSplashEffect(Vector3 collisionPoint)
    {
        Instantiate(waterSplash, collisionPoint, Quaternion.identity);
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

    void TriggerIceCubeAnimation()
    {
        iceCube.SetActive(true);

        playerNameText.enabled = false;
        playerNameTextBack.enabled = false;

        Col.enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        iceCube.GetComponent<Renderer>().material.EnableKeyword("_NORMALMAP");
        iceCube.GetComponent<Renderer>().material.EnableKeyword("_DETAIL_MULX2");

        CanMove = false;

        anim.SetTrigger("StandingPose");

        //Put player under water
        transform.position = new Vector3(transform.position.x, 20.0f, transform.position.z);

        StartCoroutine("BringToTop");
    }

    IEnumerator BringToTop()
    {
        for (int i = 0; i <= 60; i++)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(20.0f, 25.0f, (float)i/60.0f), transform.position.z);
            yield return new WaitForSeconds(0.025f);
        }

        //Continually bob up and down
        InvokeRepeating("Bob", 0, 0.025f);
    }

    void Bob()
    {
        float t = Mathf.PingPong(Time.time, 1);

        transform.position = Vector3.Slerp(new Vector3(transform.position.x, 24, transform.position.z), new Vector3(transform.position.x, 25, transform.position.z), t);
        //transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, -5), Quaternion.Euler(0, 0, 5), t);
    }
}