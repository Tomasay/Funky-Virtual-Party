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