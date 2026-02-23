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

#if UNITY_EDITOR
using ParrelSync;
#endif

public class RealtimeSingletonWeb : MonoBehaviour
{
    private static RealtimeSingletonWeb Instance = null;
    public static RealtimeSingletonWeb instance;

    ClientPlayer localPlayer;

    CustomAvatars.RealtimeAvatarManager realtimeAvatarManager;

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

    [SerializeField]
    public TMP_Text scenePartyCodeLetters;

    [SerializeField]
    public TMP_Text[] scenePartyCodeLettersIndividuals;

    bool disconnectingDueToNoHost, disconnectingMaxPlayers, disconnectingMinigameInProgress;


    public UnityEvent LocalPlayerSpawned;

    public UnityEvent ProperlyConnectedToRoom;


    public ClientPlayer LocalPlayer { get => localPlayer; }

    public Realtime Realtime { get => realtime; }

    public CustomAvatars.RealtimeAvatarManager RealtimeAvatarManager { get => realtimeAvatarManager; }

    public CustomAvatars.RealtimeAvatar VRAvatar { get => realtimeAvatarManager.avatars[0]; }
    public bool isVRAvatarSpawned { get => realtimeAvatarManager.avatars.Count > 0; }

    public bool safeToJoinMinigames = false;

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void ReloadPage();

    [DllImport("__Internal")]
    private static extern void StoreNameData(string name);
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
            instance.scenePartyCodeLetters = scenePartyCodeLetters;
            instance.SetScenePartyCodeText();
            instance.keyboardController = keyboardController;
            Destroy(gameObject);
        }

        if (LocalPlayerSpawned == null)
            LocalPlayerSpawned = new UnityEvent();

        if (ProperlyConnectedToRoom == null)
            ProperlyConnectedToRoom = new UnityEvent();

        realtimeAvatarManager = GetComponent<CustomAvatars.RealtimeAvatarManager>();

        realtime.didConnectToRoom += ConnectedToRoom;
        realtime.didDisconnectFromRoom += CheckDisconnectedReason;

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        realtimeAvatarManager.avatarDestroyed += AvatarManager_avatarDestroyed;

        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        nameInput.text = ClonesManager.GetArgument();
        partyCodeInput.text = "TEST";
        Invoke("SubmitButtonPressed", 1);
#endif
    }

    private void AvatarManager_avatarDestroyed(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if(!avatar.isOwnedRemotelyInHierarchy) //Check to make sure avatar isn't just being destroyed to switch scenes
        {
#if UNITY_WEBGL
            if (!disconnectingDueToNoHost && !disconnectingMaxPlayers && !disconnectingMinigameInProgress)
            {
#if !UNITY_EDITOR
                ReloadPage();
#endif
            }
            else
            {
                disconnectingDueToNoHost = disconnectingMaxPlayers = disconnectingMinigameInProgress = false;
            }
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
#if UNITY_WEBGL && !UNITY_EDITOR
        StoreNameData(nameInput.text);
#endif

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
        if (!(realtimeAvatarManager.avatars.Count > 0))
        {
            disconnectingDueToNoHost = true;
            realtime.Disconnect();
            return;
        }

        if (ClientPlayer.clients != null && ClientPlayer.clients.Count >= ClientPlayer.maxClients)
        {
            disconnectingMaxPlayers = true;
            realtime.Disconnect();
            return;
        }

        if(SceneManager.GetActiveScene().name.Equals("VRtistryStandaloneClient"))
        {
            //TODO: setup system to check if game is in progress for standalone
        }
        else if (!SceneChangerSyncer.instance.CurrentScene.Equals("MainMenu") && !SceneChangerSyncer.instance.CurrentScene.Equals(""))
        {
            disconnectingMinigameInProgress = true;
            realtime.Disconnect();
            return;
        }


        //If all good, connect and spawn player
        ProperlyConnectedToRoom.Invoke();

        safeToJoinMinigames = true;

        SetJoinedUI(true);
        SetScenePartyCodeText();

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
        CustomAvatars.RealtimeAvatar[] avatars = GameObject.FindObjectsOfType<CustomAvatars.RealtimeAvatar>();
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
            if(logoCanvas) logoCanvas.GetComponent<Animator>().SetTrigger("Close");
            mainMenuCam.transform.DORotate(new Vector3(45, 0, 0), 1.5f);
        }
        else
        {
            joinRoomCanvas.gameObject.SetActive(false);
            if(logoCanvas) logoCanvas.gameObject.SetActive(false);
            mainMenuCam.transform.Rotate(90, 0, 0);
        }

        if(controllerCanvas) controllerCanvas.enabled = true;
        enableCustomizationsButton.gameObject.SetActive(true);
    }

    public void SetScenePartyCodeText()
    {
        if(scenePartyCodeLetters)
        {
            scenePartyCodeLetters.text = "Party Code: " + realtime.room.name;
        }
        if (scenePartyCodeLettersIndividuals.Length > 0)
        {
            for (int i = 0; i < scenePartyCodeLettersIndividuals.Length; i++)
            {
                scenePartyCodeLettersIndividuals[i].text = "" + realtime.room.name[i];
            }
        }
    }

    void CheckDisconnectedReason(Realtime realtime)
    {
        if(disconnectingDueToNoHost)
        {
            loadingCircle.SetActive(false);

            joinRoomCanvas.enabled = true;
            partyCodeInvalid.gameObject.SetActive(true);
            CheckValidPartyCode(partyCodeInput.text);
        }

        if(disconnectingMaxPlayers)
        {
            loadingCircle.SetActive(false);

            joinRoomCanvas.enabled = true;
            maxPlayersReached.gameObject.SetActive(true);
            CheckValidPartyCode(partyCodeInput.text);
        }

        if (disconnectingMinigameInProgress)
        {
            loadingCircle.SetActive(false);

            joinRoomCanvas.enabled = true;
            minigameInProgress.gameObject.SetActive(true);
            CheckValidPartyCode(partyCodeInput.text);
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