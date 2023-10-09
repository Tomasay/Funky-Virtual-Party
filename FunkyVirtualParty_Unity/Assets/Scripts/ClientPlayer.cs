using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;
using UnityEngine.InputSystem;
using Normal.Realtime;
using UnityEngine.Events;

[Serializable]
public class ClientInputData
{
    public byte id;
    public Vector2 input;
}

[Serializable]
public class MyCPEvent : UnityEvent<ClientPlayer>
{
}

public class ClientPlayer : MonoBehaviour
{
    [SerializeField] public ClientSync syncer;
    [SerializeField] public AnimatorSyncer animSyncer;

    public static List<ClientPlayer> clients;
    public static int maxClients = 8;

    [SerializeField] public Texture2D colorPalette;
    public static List<Color> availableColors; //Colors not used from the available palette
    [SerializeField] public TMP_Text playerNameText;
    [SerializeField] protected TMP_Text playerIndicatorText;
    [SerializeField] public SkinnedMeshRenderer smr;
    [SerializeField] protected Animator anim;
    [SerializeField] protected GameObject spineBone; //Used to change height of player
    [SerializeField] protected Collider col; //Used to change height of player

    [SerializeField] public RealtimeView realtimeView;
    [SerializeField] public  RealtimeTransform realtimeTransform, animRealtimeTransform;

    [SerializeField] public GameObject[] hats;
    [SerializeField] GameObject hatAttachPoint;
    [SerializeField] ParticleSystem hatPoofParticles;
    protected GameObject currentHat;

    [SerializeField] public Vector3[] spawnPoints;

    protected string playerSocketID;
    protected byte playerByteID;

    protected string playerIP;
    protected bool isLocal = false; //Is this the player being controlled by device?
    private Vector3 spinePos;

    protected Vector3 movement;
    protected float startingSpeed = 5, speed;
    protected bool canMove = true;

    protected Rigidbody rb;

    protected PlayerInput playerInput;

    public string PlayerSocketID { get => playerSocketID; set => playerSocketID = value; }
    public byte PlayerByteID { get => playerByteID; set => playerByteID = value; }
    public string PlayerIP { get => playerIP; set => playerIP = value; }

    public bool IsLocal { get => isLocal; set => isLocal = value; }
    public bool CanMove { get => canMove; set => canMove = value; }

    public Animator Anim { get => anim;}
    public Collider Col { get => col; }
    public SkinnedMeshRenderer Smr { get => smr; }

    public static MyCPEvent OnClientConnected, OnClientDisconnected, OnReadyUp, OnColorChanged;

    //Debug client player variables
    public static int debugPlayerCount = 10; //Starting at 10 so debug player's index doesn't interfere with real clients

    protected int debugPlayerIndex;

    public int DebugPlayerIndex { get => debugPlayerIndex; }

    protected virtual void Awake()
    {
        if (clients == null)
            clients = new List<ClientPlayer>();

        if (OnClientConnected == null)
            OnClientConnected = new MyCPEvent();

        if (OnClientDisconnected == null)
            OnClientDisconnected = new MyCPEvent();

        if (OnReadyUp == null)
            OnReadyUp = new MyCPEvent();

        if (OnColorChanged == null)
            OnColorChanged = new MyCPEvent();

        clients.Add(this);

        //Instantiate list of available colors from palette
        if (availableColors == null)
        {
            availableColors = new List<Color>();
            for (int i = 0; i < colorPalette.width; i++)
            {
                availableColors.Add(colorPalette.GetPixel(i, 0));
            }
        }

        //smr only uses shared mesh. Have to instantiate individual mesh so they can have unique vertex colors
        Mesh m = smr.sharedMesh;
        Mesh m2 = Instantiate(m);
        smr.sharedMesh = m2;

        speed = startingSpeed;

        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        spinePos = spineBone.transform.localPosition;
    }

    protected virtual void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (syncer.IsDebugPlayer)
        {
            debugPlayerIndex = debugPlayerCount;
            debugPlayerCount++;
        }

        if (realtimeView.isOwnedLocallyInHierarchy)
            LocalStart();
        
        OnClientConnected.Invoke(this);
    }

    protected virtual void LocalStart()
    {
        realtimeTransform.RequestOwnership();
        animRealtimeTransform.RequestOwnership();

        SetSpawnPoint();

        playerInput.actions["Action"].started += Action;
    }

    private void OnDestroy()
    {
        playerInput.actions["Action"].started -= Action;

        clients.Remove(this);
    }

    protected virtual void CheckInput()
    {
        if (IsLocal) //Only read values from analog stick, and emit movement if being done from local device
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            if (!(input == Vector2.zero && movement == Vector3.zero)) //No need to call Move if input is 0 and we're already not moving
            {
                Move(input);
            }
        }
    }

    protected virtual void Update()
    {
        if (Camera.main)
        {
            playerNameText.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }

        CheckInput();
    }

    protected void SetSpawnPoint()
    {
        if (syncer.IsDebugPlayer)
        {
            transform.position = spawnPoints[debugPlayerIndex - 9];
            return;
        }

        transform.position = spawnPoints[realtimeView.ownerIDSelf-1];
    }

    //Doesn't seem to work currently
    public int GetPlayerIndex()
    {
        return clients.IndexOf(this);
    }

    public void InitialCustomize()
    {
        //Color
        syncer.Color = availableColors[UnityEngine.Random.Range(0, availableColors.Count)];

        //Head shapes
        syncer.HeadType = UnityEngine.Random.Range(-1, smr.sharedMesh.blendShapeCount);

        //Height
        syncer.Height = UnityEngine.Random.Range(-0.2f, 0.75f);

        //Hat, default none
        syncer.HatIndex = -1;
    }

    public void SetCustomizations(Color color, int headShape, float height, int hatIndex)
    {
        //Color
        syncer.Color = color;

        //Head shape
        syncer.HeadType = headShape;

        //Height
        syncer.Height = height;

        //Hat, default none
        syncer.HatIndex = hatIndex;
    }

    public void UpdateHat(int newIndex, bool triggerParticles = true)
    {
        if (currentHat)
        {
            Destroy(currentHat);
        }

        if (newIndex != -1)
        {
            currentHat = Instantiate(hats[newIndex], hatAttachPoint.transform);
            if(triggerParticles) hatPoofParticles.Play();
        }
    }

    public void UpdateHeadType(int newHeadType)
    {
        if (newHeadType > -1)
        {
            smr.SetBlendShapeWeight(newHeadType, 100);
        }
    }

    public void UpdateHeight(float newHeight)
    {
        spineBone.transform.localPosition = spinePos + new Vector3(0, newHeight, 0);
    }

    public void SetCustomizations(string color, int headShape, float height, int hatIndex)
    {
        if (ColorUtility.TryParseHtmlString(color, out Color newCol))
        {
            syncer.Color = newCol;
        }

        syncer.HeadType = headShape;
        syncer.Height = height;

        bool newHat = (syncer.HatIndex != hatIndex);

        if(currentHat)
        {
            Destroy(currentHat);
        }

        if (hatIndex > -1)
        {
            currentHat = Instantiate(hats[syncer.HatIndex], hatAttachPoint.transform);
            if (newHat) { hatPoofParticles.Play(); }
        }
    }

    /// <summary>
    /// Changes the color locally
    /// </summary>
    /// <param name="col"></param>
    public void ChangeColor(Color col)
    {
        Mesh mesh = smr.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = col;
        }
        availableColors.Remove(col);
        mesh.colors = colors;
    }

    public int GetColorIndex()
    {
        List<Color> cols = new List<Color>();
        for (int i = 0; i < colorPalette.width; i++)
        {
            cols.Add(colorPalette.GetPixel(i, 0));
        }

        for (int i = 0; i < cols.Count; i++)
        {
            if (syncer.Color.Equals(cols[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public virtual void Move(Vector2 input, bool changeDirection = true, bool animate = true, Vector2 overrideRotation = default)
    {
        if (canMove)
        {
            movement = new Vector3(input.x * speed, 0, input.y * speed);
            transform.Translate(movement * Time.deltaTime);

            //Magnitude of movement for animations
            if (animate)
            {
                float val = Mathf.Abs(input.magnitude);
                if ((val > 0.05) || (val < -0.05))
                {
                    syncer.AnimSpeed = val;
                }
                else
                {
                    syncer.AnimSpeed = 0;
                }
            }
            else
            {
                syncer.AnimSpeed = 0;
            }

            //Update rotation
            if (changeDirection)
            {
                Vector3 lookDirection;
                if (overrideRotation != Vector2.zero)
                {
                    lookDirection = new Vector3(overrideRotation.x, 0, overrideRotation.y);
                }
                else
                {
                    lookDirection = new Vector3(input.x, 0, input.y);
                }

                if (lookDirection != Vector3.zero)
                {
                    anim.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                }
            }
        }
        else
        {
            movement = Vector3.zero;
            syncer.AnimSpeed = 0;
        }
    }

    //Sets player visuals and movement
    protected void SetPlayerActive(bool active)
    {
        smr.enabled = active;
        playerNameText.enabled = active;

        CanMove = active;
    }

    //Sets player name visibility
    public void SetPlayerNameVisibility(bool active)
    {
        playerNameText.enabled = active;
    }

    //Sets player indicator
    public void SetPlayerIndicatorVisibility(bool active)
    {
        playerIndicatorText.enabled = active;
    }

    //Default action, dance of course
    public virtual void Action()
    {
        animSyncer.Trigger = "Dance" + UnityEngine.Random.Range(1, 4);
    }

    public virtual void Action(InputAction.CallbackContext obj)
    {
        animSyncer.Trigger = "Dance" + UnityEngine.Random.Range(1, 4);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("COLLIDING WITH: " + collision.gameObject.name + " WITH TAG " + collision.gameObject.tag);
    }

    public static ClientPlayer GetClientByOwnerID(int id)
    {
        foreach (ClientPlayer cp in clients)
        {
            if (cp.realtimeView.ownerIDSelf == id)
            {
                return cp;
            }
        }

        return null;
    }
}