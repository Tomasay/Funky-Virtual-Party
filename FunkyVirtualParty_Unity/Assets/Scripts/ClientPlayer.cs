using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;

[Serializable]
public class ClientInputData
{
    public byte id;
    public Vector2 input;
}

public class ClientPlayer : MonoBehaviour
{
    [SerializeField] public Texture2D colorPalette;
    public static List<Color> availableColors; //Colors not used from the available palette
    [SerializeField] protected TMP_Text playerNameText;
    [SerializeField] protected TMP_Text playerIndicatorText;
    [SerializeField] protected SkinnedMeshRenderer smr;
    [SerializeField] protected Animator anim;
    [SerializeField] protected GameObject spineBone; //Used to change height of player
    [SerializeField] protected Collider col; //Used to change height of player

    [SerializeField] public GameObject[] hats;
    [SerializeField] GameObject hatAttachPoint;
    [SerializeField] ParticleSystem hatPoofParticles;
    protected GameObject currentHat;
    protected int currentHatIndex = -1;

    protected string playerSocketID;
    protected byte playerByteID;

    protected string playerIP, playerName;
    protected bool isLocal = false; //Is this the player being controlled by device?
    protected Color playerColor = Color.white;
    protected int headType;
    protected float height; //Between -0.2f anf 2.0f
    private Vector3 spinePos;

    protected Vector3 movement;
    protected Quaternion lookRotation;
    protected float startingSpeed = 20, speed;
    protected bool canMove = true;

    protected Rigidbody rb;

    public Vector3 posFromHost; //Current position from host, we need to sync to this if different

    protected PlayerInput playerInput;

    public string PlayerSocketID { get => playerSocketID; set => playerSocketID = value; }
    public byte PlayerByteID { get => playerByteID; set => playerByteID = value; }
    public string PlayerIP { get => playerIP; set => playerIP = value; }
    public string PlayerName { get => playerName; set { playerNameText.text = playerName = value; } }
    public Color PlayerColor { get => playerColor; set{ playerColor = value; ChangeColor(value); } }
    public Color PlayerNameColor { get => playerNameText.color; set => playerNameText.color = value; }
    public int PlayerHeadType { get => headType; set{ headType = value; if (headType > -1) { smr.SetBlendShapeWeight(value, 100); } } }
    public float PlayerHeight { get => height; set{ height = value; spineBone.transform.localPosition = spinePos + new Vector3(0, height, 0); } }
    public int PlayerHatIndex { get => currentHatIndex; set{ UpdateHat(value); } }

    public bool IsLocal { get => isLocal; set => isLocal = value; }
    public bool CanMove { get => canMove; set => canMove = value; }

    public Animator Anim { get => anim;}
    public Collider Col { get => col; }
    public SkinnedMeshRenderer Smr { get => smr; }

    protected float inputPollRate = 0.05f;

#if UNITY_EDITOR
    public bool isDebugPlayer;
#endif


    public byte[] SerializeInputData(Vector2 input)
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(PlayerByteID);
                writer.Write(input);
            }
            return m.ToArray();
        }
    }

    public static ClientInputData DeserializeInputData(byte[] data)
    {
        ClientInputData result = new ClientInputData();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                result.id = reader.ReadByte();
                result.input = reader.ReadVector2();
            }
        }
        return result;
    }

    protected virtual void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void Awake()
    {
        //Instantiate list of available colors from palette
        if(availableColors == null)
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

        InvokeRepeating("CheckInput", 0, inputPollRate);
    }

    private void OnDestroy()
    {
        CancelInvoke("CheckInput");
    }

    protected virtual void CheckInput()
    {
        if (IsLocal) //Only read values from analog stick, and emit movement if being done from local device
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            if (!(input == Vector2.zero && movement == Vector3.zero)) //No need to send input if we're sending 0 and we're already not moving
            {
                ClientManagerWeb.instance.Manager.Socket.Emit("IS", SerializeInputData(input));
                Move(input);
            }

            /*
            Vector3 positionDifference = posFromHost - transform.position;
            transform.Translate((movement + positionDifference / 4) * Time.deltaTime);
            */
          
        }

        // check if we are below the floor
        if (transform.position.y < -10 && transform.position.y != posFromHost.y)
        {
            transform.position = posFromHost;
            rb.velocity = Vector3.zero;
        }

        //anim.transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, Time.deltaTime);
    }    

    protected virtual void Update()
    {
        if (Camera.main)
        {
            playerNameText.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }

        // This needs to exist here and not in CheckInput becuase of the polling rate, if it's not here the game will drop the input
        if (playerInput.actions["ActionButton"].triggered)
        {
            ClientManagerWeb.instance.ActionButtonPressed();
        }
    }

    public void InitialCustomize()
    {
        //Color
        if (playerColor == Color.white)
        {
            Color newCol = availableColors[UnityEngine.Random.Range(0, availableColors.Count)];
            ChangeColor(newCol);
        }

        //Head shapes
        headType = UnityEngine.Random.Range(-1, smr.sharedMesh.blendShapeCount);
        if (headType > -1) //if -1, keep base head shape
        {
            smr.SetBlendShapeWeight(headType, 100);
        }

        //Height
        Vector3 pos = spineBone.transform.localPosition;
        height = UnityEngine.Random.Range(-0.2f, 0.75f);
        pos.y += height;
        spineBone.transform.localPosition = pos;

        //Hat, default none
        currentHatIndex = -1;

        if (isLocal)
        {
            ClientManagerWeb.instance.Manager.Socket.Emit("syncCustomizationsFromClient", "#" + ColorUtility.ToHtmlStringRGB(playerColor), headType, height, currentHatIndex);
        }
    }

    void UpdateHat(int newIndex)
    {
        if (currentHat)
        {
            Destroy(currentHat);
        }

        currentHatIndex = newIndex;
        if (newIndex > -1)
        {
            currentHat = Instantiate(hats[currentHatIndex], hatAttachPoint.transform);
            hatPoofParticles.Play();
        }

        if (isLocal)
        {
            ClientManagerWeb.instance.Manager.Socket.Emit("syncCustomizationsFromClient", "#" + ColorUtility.ToHtmlStringRGB(playerColor), headType, height, currentHatIndex);
        }
    }

    public void SetCustomizations(string color, int headShape, float height, int hatIndex)
    {
        if (ColorUtility.TryParseHtmlString(color, out Color newCol))
        {
            ChangeColor(newCol);
        }

        PlayerHeadType = headShape;
        PlayerHeight = height;

        bool newHat = (currentHatIndex != hatIndex);

        if(currentHat)
        {
            Destroy(currentHat);
        }

        currentHatIndex = hatIndex;
        if (hatIndex > -1)
        {
            currentHat = Instantiate(hats[currentHatIndex], hatAttachPoint.transform);
            if (newHat) { hatPoofParticles.Play(); }
        }
    }

    /// <summary>
    /// Changes the color locally
    /// </summary>
    /// <param name="col"></param>
    private void ChangeColor(Color col)
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
        playerColor = col;
    }

    /// <summary>
    /// Changes the color locally and syncs on network
    /// </summary>
    /// <param name="col"></param>
    public void UpdateColor(Color col)
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
        playerColor = col;

        if (isLocal)
        {
            ClientManagerWeb.instance.Manager.Socket.Emit("syncCustomizationsFromClient", "#" + ColorUtility.ToHtmlStringRGB(playerColor), headType, height, currentHatIndex);
        }
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
            if (PlayerColor.Equals(cols[i]))
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
            movement = new Vector3(input.x * speed, 0, input.y * speed) * .01f;

            transform.DOBlendableMoveBy(movement, inputPollRate);

            //Magnitude of movement for animations
            if (animate)
            {
                float val = Mathf.Abs(input.magnitude);
                if ((val > 0.05) || (val < -0.05))
                {
                    anim.SetFloat("Speed", val);
                }
                else
                {
                    anim.SetFloat("Speed", 0);
                }
            }
            else
            {
                anim.SetFloat("Speed", 0);
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
                    lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                    anim.transform.DORotateQuaternion(lookRotation, inputPollRate);
                }
            }
        }
        else
        {
            movement = Vector3.zero;
            anim.SetFloat("Speed", 0);
        }
    }

    public virtual void Move(byte[] inputData)
    {
        ClientInputData newData = DeserializeInputData(inputData);

#if UNITY_WEBGL
        ClientManagerWeb.instance.GetPlayerByByteID(newData.id).Move(newData.input);
#elif UNITY_ANDROID
        ClientManager.instance.GetPlayerByByteID(newData.id).Move(newData.input);
#endif

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
        Debug.Log("Set name visible: " + active);
    }

    //Sets player indicator
    public void SetPlayerIndicatorVisibility(bool active)
    {
        playerIndicatorText.enabled = active;
    }

    //Default action, dance of course
    public virtual void Action()
    {
        int dance = UnityEngine.Random.Range(1, 4);
        anim.SetTrigger("Dance" + dance);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("COLLIDING WITH: " + collision.gameObject.name + " WITH TAG " + collision.gameObject.tag);
    }
}