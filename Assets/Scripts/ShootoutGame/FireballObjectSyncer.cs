using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballObjectSyncer : ObjectSyncer
{
    [SerializeField] GameObject fireballMesh;
    [SerializeField] ParticleSystem explosion, smokePuff;

    [SerializeField] SpriteRenderer indicator;
    private int maxIndicatorDistance = 20;

    private bool lastActiveSent; //Value of isActive last sent to clients

    public class FireballObjectData : ObjectData
    {
        public Vector3 scale;
        public bool isActive;
    }

    FireballObjectData currentFireballData;

    public FireballObjectData CurrentFireballData { get => currentFireballData;}

    protected override void Awake()
    {
        currentFireballData = new FireballObjectData();
        currentFireballData.Awake();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string>("ObjectDataToClient", ReceiveData);
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);

        //indicator.transform.parent = null;
#endif

#if !UNITY_WEBGL
        InvokeRepeating("SendData", 0, 1/UpdatesPerSecond);
#endif
    }

    void Update()
    {
#if UNITY_WEBGL
        if (currentFireballData.isActive)
        {
            //Check to see if above terrain
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxIndicatorDistance))
            {
                if (hit.transform.TryGetComponent<Terrain>(out Terrain ter))
                {
                    //Enable indicator
                    indicator.enabled = true;
                    indicator.transform.position = hit.point + new Vector3(0, 0.1f, 0);
                    indicator.transform.rotation = Quaternion.Euler(Quaternion.identity.eulerAngles + new Vector3(-90, 0, 0));

                    //Color and size
                    float t = hit.distance / maxIndicatorDistance;
                    transform.localScale = Vector3.one * Mathf.Lerp(0.1f, 2, t);
                    indicator.color = Color.Lerp(Color.red, Color.yellow, t);

                }
                else
                {
                    indicator.enabled = false;
                }
            }
            else
            {
                indicator.enabled = false;
            }
        }
        else
        {
            indicator.enabled = false;
        }
#endif
    }

    void MethodCalledFromServer(string methodName, string data)
    {
        if (int.TryParse(data, out int id) && id == currentFireballData.objectID)
        {
            if (methodName.Equals("SmokePuffEvent"))
            {
                smokePuff.Play();
            }
            else if (methodName.Equals("FireballExplosionEvent"))
            {
                explosion.Play();
            }
        }
    }

#if !UNITY_WEBGL
    protected override void SendData()
    {
        Fireball f = GetComponent<Fireball>();
        if (f.fireball.activeSelf || lastActiveSent == true) //Only send data if fireball is active, make sure it is marked inactive on client first
        {
            //Position
            currentFireballData.Position = transform.position;

            //Rotation
            currentFireballData.Rotation = transform.rotation;

            //Fireball variables
            currentFireballData.isActive = lastActiveSent = f.fireball.activeSelf;
            currentFireballData.scale = f.fireball.transform.localScale;

            //Send Data
            string json = JsonUtility.ToJson(currentFireballData);
            if (ClientManager.instance)
            {
                ClientManager.instance.Manager.Socket.Emit("ObjectDataToServer", json);
            }
        }
    }
#endif

    public override void ReceiveData(string json)
    {
        ApplyNewFireballData(JsonUtility.FromJson<FireballObjectData>(json));
    }

    protected void ApplyNewFireballData(FireballObjectData data)
    {
        if (data.objectID != currentFireballData.objectID)
        {
            return;
        }

        //Position
        currentFireballData.Position = transform.position = data.Position;

        //Rotation
        currentFireballData.Rotation = transform.rotation = data.Rotation;

        //Fireball
        currentFireballData.isActive = data.isActive;
        fireballMesh.SetActive(data.isActive);

        currentFireballData.scale = data.scale;
        fireballMesh.transform.localScale = data.scale;
    }
}