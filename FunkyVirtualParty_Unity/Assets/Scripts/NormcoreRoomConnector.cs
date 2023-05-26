using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Normal.Realtime;
using UnityEngine.UI;

public class NormcoreRoomConnector : MonoBehaviour
{
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

    public ClientPlayer LocalPlayer { get => localPlayer; }

    private void Awake()
    {
        avatarManager = GetComponent<RealtimeAvatarManager>();

        realtime.didConnectToRoom += ConnectedToRoom;
        realtime.didDisconnectFromRoom += CheckDisconnectedDueToNoHost;
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
        Debug.Log("Avatars in room: " + avatarManager.avatars.Count);

        if (avatarManager.avatars.Count > 0)
        {
            loadingCircle.SetActive(false);

            foreach (GameObject g in objectsToEnableOnJoin)
            {
                g.SetActive(true);
            }

            logoCanvas.enabled = false;
            controllerCanvas.enabled = true;
            enableCustomizationsButton.gameObject.SetActive(true);

            GameObject newPlayer = Realtime.Instantiate("ClientPlayer", Realtime.InstantiateOptions.defaults);
            localPlayer = newPlayer.GetComponent<ClientPlayer>();
            localPlayer.syncer.Name = nameInput.text;
            localPlayer.IsLocal = true;
        }
        else
        {
            realtime.Disconnect();
            disconnectingDueToNoHost = true;
        }
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

    public void CheckValidPartyCode(string val)
    {
        submitButton.interactable = (val.Length == 4);
    }

    public void PartyCodeInputToUpper(string val)
    {
        partyCodeInput.text = val.ToUpper();
    }
}