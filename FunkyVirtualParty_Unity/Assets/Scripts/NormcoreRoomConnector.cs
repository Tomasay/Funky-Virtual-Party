using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Normal.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public class NormcoreRoomConnector : MonoBehaviour
{
    private static NormcoreRoomConnector Instance = null;
    public static NormcoreRoomConnector instance;

    ClientPlayer localPlayer;

    RealtimeAvatarManager avatarManager;

    [SerializeField]
    Realtime realtime;

    [SerializeField]
    GameObject[] objectsToEnableOnJoin;

    [SerializeField]
    TMP_InputField nameInput, partyCodeInput;

    [SerializeField]
    GameObject partyCodeInvalid;

    [SerializeField]
    Button submitButton;

    [SerializeField] Button enableCustomizationsButton;

    [SerializeField]
    Canvas joinRoomCanvas, controllerCanvas, logoCanvas;

    [SerializeField]
    GameObject loadingCircle;

    bool disconnectingDueToNoHost;


    public UnityEvent LocalPlayerSpawned;


    public ClientPlayer LocalPlayer { get => localPlayer; }

    public RealtimeAvatar VRAvatar { get => avatarManager.avatars[0]; }
    public bool isVRAvatarSpawned { get => avatarManager.avatars.Count > 0; }

    [DllImport("__Internal")]
    private static extern void ReloadPage();

    private void Awake()
    {
        //Singleton instantiation
        if (!instance)
        {
            instance = this;
        }
        else
        {
            SetJoinedUI();
            Destroy(gameObject);
        }

        if (LocalPlayerSpawned == null)
            LocalPlayerSpawned = new UnityEvent();

        avatarManager = GetComponent<RealtimeAvatarManager>();

        realtime.didConnectToRoom += ConnectedToRoom;
        realtime.didDisconnectFromRoom += CheckDisconnectedDueToNoHost;

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        avatarManager.avatarDestroyed += AvatarManager_avatarDestroyed;

        DontDestroyOnLoad(gameObject);
    }

    private void AvatarManager_avatarDestroyed(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if(!avatar.isOwnedRemotelyInHierarchy) //Check to make sure avatar isn't just being destroyed to switch scenes
        {
            ReloadPage();
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (realtime.connected)
        {
            StartCoroutine("SpawnPlayerDelayed");
        }
    }

    private void OnDestroy()
    {
        realtime.didConnectToRoom -= ConnectedToRoom;
        realtime.didDisconnectFromRoom -= CheckDisconnectedDueToNoHost;

        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    public void SubmitButtonPressed()
    {
        realtime.Connect(partyCodeInput.text);

        //Display loading indicator
        joinRoomCanvas.enabled = false;
        loadingCircle.SetActive(true);
    }

    void ConnectedToRoom(Realtime realtime)
    {
        if (avatarManager.avatars.Count > 0)
        {
            SetJoinedUI();

            SpawnPlayer();
        }
        else
        {
            realtime.Disconnect();
            disconnectingDueToNoHost = true;
        }
    }

    void SetJoinedUI()
    {
        loadingCircle.SetActive(false);

        foreach (GameObject g in objectsToEnableOnJoin)
        {
            g.SetActive(true);
        }

        joinRoomCanvas.enabled = false;
        logoCanvas.enabled = false;
        controllerCanvas.enabled = true;
        enableCustomizationsButton.gameObject.SetActive(true);
    }

    void CheckDisconnectedDueToNoHost(Realtime realtime)
    {
        if(disconnectingDueToNoHost)
        {
            loadingCircle.SetActive(false);

            joinRoomCanvas.enabled = true;
            partyCodeInvalid.gameObject.SetActive(true);

            disconnectingDueToNoHost = false;
        }
    }

    IEnumerator SpawnPlayerDelayed()
    {
        yield return new WaitForSeconds(1);

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        GameObject newPlayer = Realtime.Instantiate(currentScene + "Player", Realtime.InstantiateOptions.defaults);

        if (localPlayer) //If player has already been spawned before
        {
            ClientPlayer oldPlayer = localPlayer;
            localPlayer = newPlayer.GetComponent<ClientPlayer>();

            localPlayer.syncer.Name = oldPlayer.syncer.Name;
            localPlayer.SetCustomizations(oldPlayer.syncer.Color, oldPlayer.syncer.HeadType, oldPlayer.syncer.Height, oldPlayer.syncer.HatIndex);

            Realtime.Destroy(oldPlayer.gameObject);
        }
        else
        {
            localPlayer = newPlayer.GetComponent<ClientPlayer>();
            localPlayer.syncer.Name = nameInput.text;
        }

        localPlayer.IsLocal = true;
        LocalPlayerSpawned.Invoke();
    }

    public void CheckValidPartyCode(string val)
    {
        submitButton.interactable = (val.Length == 4);
    }

    public void PartyCodeInputToUpper(string val)
    {
        partyCodeInput.text = val.ToUpper();
    }
}