using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Normal.Realtime;

public class NormcoreRoomConnector : MonoBehaviour
{
    GameObject localPlayer;

    [SerializeField]
    Realtime realtime;

    [SerializeField]
    TMP_InputField nameInput, roomCodeInput;

    [SerializeField]
    Canvas joinRoomCanvas, controllerCanvas;

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

        localPlayer = Realtime.Instantiate("ClientPlayer", Realtime.InstantiateOptions.defaults);
        localPlayer.GetComponent<ClientPlayer>().PlayerName = nameInput.text;
        localPlayer.GetComponent<ClientPlayer>().IsLocal = true;
    }
}