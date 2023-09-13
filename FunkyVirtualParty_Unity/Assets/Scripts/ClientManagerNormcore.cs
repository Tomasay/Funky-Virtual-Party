using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;
using TMPro;

public class ClientManagerNormcore : MonoBehaviour
{
    [SerializeField]
    TMP_Text linkInfoText, partyCodeText, connectingText, serverConnectionErrorText, internetConnectionErrorText;

    [SerializeField]
    Image loadingIcon, errorIcon;

    [SerializeField]
    Camera connectingUICamera;

    private const int PASSCODE_LENGTH = 4;

    void Start()
    {
        RealtimeSingleton.instance.Realtime.didConnectToRoom += Realtime_didConnectToRoom;
        RealtimeSingleton.instance.Realtime.didDisconnectFromRoom += Realtime_didDisconnectFromRoom;

        if (!RealtimeSingleton.instance.Realtime.connected)
        {
            RealtimeSingleton.instance.Realtime.Connect(GenerateCode());
        }
        else
        {
            partyCodeText.text = "Party Code: " + RealtimeSingleton.instance.Realtime.room.name;

            loadingIcon.enabled = false;
            connectingText.enabled = false;
            serverConnectionErrorText.enabled = false;
            internetConnectionErrorText.enabled = false;
            errorIcon.enabled = false;

            linkInfoText.enabled = true;
            partyCodeText.enabled = true;

            connectingUICamera.enabled = false;
        }
    }

    private void Update()
    {
        if (RealtimeSingleton.instance.Realtime.connecting)
        {
            loadingIcon.enabled = true;
            connectingText.enabled = true;
            serverConnectionErrorText.enabled = false;
            internetConnectionErrorText.enabled = false;
            errorIcon.enabled = false;

            linkInfoText.enabled = false;
            partyCodeText.enabled = false;
        }
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.Realtime.didConnectToRoom -= Realtime_didConnectToRoom;
        RealtimeSingleton.instance.Realtime.didDisconnectFromRoom -= Realtime_didDisconnectFromRoom;
    }

    private void Realtime_didDisconnectFromRoom(Realtime realtime)
    {
        loadingIcon.enabled = false;
        connectingText.enabled = false;
        errorIcon.enabled = true;

        linkInfoText.enabled = false;
        partyCodeText.enabled = false;

        //Check to see if error is because device is not connected to internet
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            internetConnectionErrorText.enabled = true;
        }
        else
        {
            serverConnectionErrorText.enabled = true;
        }
    }

    private void Realtime_didConnectToRoom(Realtime realtime)
    {
        loadingIcon.enabled = false;
        connectingText.enabled = false;
        serverConnectionErrorText.enabled = false;
        internetConnectionErrorText.enabled = false;

        linkInfoText.enabled = true;
        partyCodeText.enabled = true;

        connectingUICamera.enabled = false;
    }

    private string GenerateCode()
    {
        string newCode = "";
        for (int i = 0; i < PASSCODE_LENGTH; i++)
        {
            newCode += GetRandomLetter();
        }

        partyCodeText.text = "Party Code: " + newCode;

        return newCode;
    }

    public static char GetRandomLetter()
    {
        return (char)Random.Range(65, 91);
    }
}