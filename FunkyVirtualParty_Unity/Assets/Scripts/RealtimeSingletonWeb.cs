using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Normal.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using DG.Tweening;
using NaughtyAttributes;

public class RealtimeSingletonWeb : MonoBehaviour
{
    private static RealtimeSingletonWeb Instance = null;
    public static RealtimeSingletonWeb instance;

    ClientPlayer localPlayer;

    RealtimeAvatarManager realtimeAvatarManager;

    [SerializeField]
    Realtime realtime;

    [SerializeField]
    public KeyboardController keyboardController;

    [SerializeField]
    GameObject[] objectsToEnableOnJoin;

    [SerializeField]
    TMP_InputField nameInput, partyCodeInput;

    [SerializeField]
    GameObject partyCodeInvalid;

    [SerializeField]
    GameObject maxPlayersReached;

    [SerializeField]
    GameObject minigameInProgress;

    [SerializeField]
    Button submitButton;

    [SerializeField]
    Camera mainMenuCam;

    [SerializeField] Button enableCustomizationsButton;

    [SerializeField]
    Canvas joinRoomCanvas, controllerCanvas, logoCanvas;

    [SerializeField]
    GameObject loadingCircle;

    bool disconnectingDueToNoHost, disconnectingMaxPlayers, disconnectingMinigameInProgress;


    public UnityEvent LocalPlayerSpawned;


    public ClientPlayer LocalPlayer { get => localPlayer; }

    public Realtime Realtime { get => realtime; }

    public RealtimeAvatarManager RealtimeAvatarManager { get => realtimeAvatarManager; }

    public RealtimeAvatar VRAvatar { get => realtimeAvatarManager.avatars[0]; }
    public bool isVRAvatarSpawned { get => realtimeAvatarManager.avatars.Count > 0; }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void ReloadPage();
#endif

    private void Awake()
    {
        //Singleton instantiation
        if (!instance)
        {
            instance = this;
        }
        else
        {
            SetJoinedUI(false);
            instance.keyboardController = keyboardController;
            Destroy(gameObject);
        }

        if (LocalPlayerSpawned == null)
            LocalPlayerSpawned = new UnityEvent();

        realtimeAvatarManager = GetComponent<RealtimeAvatarManager>();

        realtime.didConnectToRoom += ConnectedToRoom;
        realtime.didDisconnectFromRoom += CheckDisconnectedReason;

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        realtimeAvatarManager.avatarDestroyed += AvatarManager_avatarDestroyed;

        DontDestroyOnLoad(gameObject);
    }

    private void AvatarManager_avatarDestroyed(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if(!avatar.isOwnedRemotelyInHierarchy) //Check to make sure avatar isn't just being destroyed to switch scenes
        {
#if UNITY_WEBGL
            ReloadPage();
#endif
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (realtime.connected)
        {
            Invoke("SpawnPlayer", 1);
        }
    }

    private void OnDestroy()
    {
        realtime.didConnectToRoom -= ConnectedToRoom;
        realtime.didDisconnectFromRoom -= CheckDisconnectedReason;

        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    [SerializeField]
    public string roomCode;

    [Button]
    public void ManualConnect()
    {
        realtime.Connect(roomCode);
    }

    public void SubmitButtonPressed()
    {
        submitButton.interactable = false;

        realtime.Connect(partyCodeInput.text);

        //Display loading indicator
        joinRoomCanvas.enabled = false;
        loadingCircle.SetActive(true);

        partyCodeInvalid.gameObject.SetActive(false);
        maxPlayersReached.gameObject.SetActive(false);
        minigameInProgress.gameObject.SetActive(false);
    }

    void ConnectedToRoom(Realtime realtime)
    {
        Invoke("CheckProperConnection", 1);
    }

    void CheckProperConnection()
    {
        //Disconnect circumstances
        Debug.Log("Avatars: " + realtimeAvatarManager.avatars.Count);
        if (!(realtimeAvatarManager.avatars.Count > 0))
        {
            realtime.Disconnect();
            disconnectingDueToNoHost = true;
            return;
        }

        if (ClientPlayer.clients != null && ClientPlayer.clients.Count >= ClientPlayer.maxClients)
        {
            realtime.Disconnect();
            disconnectingMaxPlayers = true;
            return;
        }

        if (!SceneChangerSyncer.instance.CurrentScene.Equals("MainMenu") && !SceneChangerSyncer.instance.CurrentScene.Equals(""))
        {
            realtime.Disconnect();
            disconnectingMinigameInProgress = true;
            return;
        }


        //If all good, connect and spawn player
        SetJoinedUI(true);

#if !UNITY_EDITOR && UNITY_WEBGL
            if(keyboardController) keyboardController.CloseKeyboard();
#endif

        SpawnPlayer();

        GameObject tracker = Realtime.Instantiate("ClientConnectedTracker", Realtime.InstantiateOptions.defaults);
        tracker.GetComponent<RealtimeView>().RequestOwnership();

        CheckForDuplicateAvatars();
    }

    //Temporary fix
    void CheckForDuplicateAvatars()
    {
        RealtimeAvatar[] avatars = GameObject.FindObjectsOfType<RealtimeAvatar>();
        for (int i = 0; i < avatars.Length-1; i++)
        {
            Destroy(avatars[i].gameObject);
        }
    }

    void SetJoinedUI(bool animate)
    {
        loadingCircle.SetActive(false);

        foreach (GameObject g in objectsToEnableOnJoin)
        {
            g.SetActive(true);
        }

        if(animate)
        {
            joinRoomCanvas.GetComponent<Animator>().SetTrigger("Close");
            logoCanvas.GetComponent<Animator>().SetTrigger("Close");
            mainMenuCam.transform.DORotate(new Vector3(45, 0, 0), 1.5f);
        }
        else
        {
            joinRoomCanvas.gameObject.SetActive(false);
            logoCanvas.gameObject.SetActive(false);
            mainMenuCam.transform.Rotate(90, 0, 0);
        }

        controllerCanvas.enabled = true;
        enableCustomizationsButton.gameObject.SetActive(true);
    }

    void CheckDisconnectedReason(Realtime realtime)
    {
        if(disconnectingDueToNoHost)
        {
            loadingCircle.SetActive(false);

            joinRoomCanvas.enabled = true;
            partyCodeInvalid.gameObject.SetActive(true);
            CheckValidPartyCode(partyCodeInput.text);

            disconnectingDueToNoHost = false;
        }

        if(disconnectingMaxPlayers)
        {
            loadingCircle.SetActive(false);

            joinRoomCanvas.enabled = true;
            maxPlayersReached.gameObject.SetActive(true);
            CheckValidPartyCode(partyCodeInput.text);

            disconnectingMaxPlayers = false;
        }

        if (disconnectingMinigameInProgress)
        {
            loadingCircle.SetActive(false);

            joinRoomCanvas.enabled = true;
            minigameInProgress.gameObject.SetActive(true);
            CheckValidPartyCode(partyCodeInput.text);

            disconnectingMinigameInProgress = false;
        }

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
            localPlayer.InitialCustomize();
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