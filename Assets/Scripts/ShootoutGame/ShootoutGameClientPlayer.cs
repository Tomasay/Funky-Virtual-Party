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

    [SerializeField] ParticleSystem waterSplash;

    public bool isAlive = true;

    protected override void Awake()
    {
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
        if (IsLocal) //Only read values from analog stick, and emit movement if being done from local device
        {
            // in this game we want the player to feel like they're on ice, so we calculate a low fricition 
            float frictionCoefficient = 0.05f;
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
        }
        else
        {
            transform.Translate(movement * Time.deltaTime);
        }

        anim.transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, Time.deltaTime);
    }
    public struct ShootoutPlayerCollisionData
    {
        public Vector2 prevVectorA;
        public Vector2 prevVectorB;
        public string idA;
        public string idB;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        #if !UNITY_WEBGL
        ShootoutPlayerCollisionData c = new ShootoutPlayerCollisionData();
        // expensive this should be optimized.
        ShootoutGameClientPlayer ext = collision.gameObject.GetComponent<ShootoutGameClientPlayer>();
        if(ext)
        {
            c.prevVectorA = prevVector;
            c.prevVectorB = ext.prevVector;
            c.idA = playerID;
            c.idB = ext.playerID;
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "BounceEvent", JsonUtility.ToJson(c));
            Debug.Log("BounceEvent sent");
            // reflect the vector on VR side
            prevVector = Vector2.Reflect(prevVector, c.prevVectorB.normalized);
        }
        #endif
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
        if (methodName.Equals("BounceEvent"))
        {
            Debug.Log("BounceEvent received");
            PerformBouncePhysics(data);
        }
    }

    // reflects on client side
    void PerformBouncePhysics(string data)
    {
        ShootoutPlayerCollisionData c = JsonUtility.FromJson<ShootoutPlayerCollisionData>(data);
        if (c.idA.Equals(playerID))
        {
            prevVector = Vector2.Reflect(prevVector, c.prevVectorB.normalized);
            Debug.Log("BounceEvent performed");
        }
    }

    void SpawnSplashEffect(Vector3 collisionPoint)
    {
        Instantiate(waterSplash, collisionPoint, Quaternion.identity);
        waterSplash.Play();
    }
}