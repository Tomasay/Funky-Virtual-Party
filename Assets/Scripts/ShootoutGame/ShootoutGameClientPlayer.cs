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

    Vector2 prevVector = Vector2.zero;
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
}