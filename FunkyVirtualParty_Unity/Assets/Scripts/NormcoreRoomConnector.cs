using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Normal.Realtime;
using UnityEngine.UI;

public class NormcoreRoomConnector : MonoBehaviour
{
    ClientPlayer localPlayer;

    [SerializeField]
    Realtime realtime;

    [SerializeField]
    TMP_InputField nameInput, roomCodeInput;

    [SerializeField] Button enableCustomizationsButton;

    [SerializeField]
    Canvas joinRoomCanvas, controllerCanvas;

    public ClientPlayer LocalPlayer { get => localPlayer; }

    private void Awake()
    {
        realtime.didConnectToRoom += ConnectedToRoom;
    }

    public void SubmitButtonPressed()
    {
        realtime.Connect(roomCodeInput.text);
    }

    void ConnectedToRoom(Realtime realtime)
    {
        joinRoomCanvas.enabled = false;
        controllerCanvas.enabled = true;
        enableCustomizationsButton.gameObject.SetActive(true);

        GameObject newPlayer = Realtime.Instantiate("ClientPlayer", Realtime.InstantiateOptions.defaults);
        localPlayer = newPlayer.GetComponent<ClientPlayer>();
        localPlayer.syncer.Name = nameInput.text;
        localPlayer.IsLocal = true;
    }
}