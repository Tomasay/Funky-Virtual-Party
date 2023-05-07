using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Normal.Realtime;

public class NormcoreRoomConnector : MonoBehaviour
{
    [SerializeField]
    Realtime realtime;

    [SerializeField]
    TMP_InputField roomCodeInput;

    public void SubmitButtonPressed()
    {
        realtime.Connect(roomCodeInput.text);
    }
}