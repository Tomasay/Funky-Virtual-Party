using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] Transform[] playerSpawns;

    private const int COUNTDOWN_AMOUNT = 5;
    [SerializeField] private TMP_Text countdownText;

    private enum GameState
    {
        Countdown,
        GameLoop,
        PlayerCaptured,
        TimeEnded
    }

    private GameState state;

    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        ClientManager.instance.SpawnPlayers(playerPrefab, playerSpawns);

        state = GameState.Countdown;
        SetPlayerMovement(false);
        StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.Countdown:
                break;
            case GameState.GameLoop:
                break;
            case GameState.PlayerCaptured:
                break;
            case GameState.TimeEnded:
                break;
            default:
                break;
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        for (int i = countdown; i > 0; i--)
        {
            countdownText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1);
        countdownText.enabled = false;

        state = GameState.GameLoop;
        SetPlayerMovement(true);
    }

    IEnumerator DisableMirroring()
    {
        yield return new WaitForEndOfFrame();
        //UnityEngine.VR.VRSettings.showDeviceView = false;
        XRSettings.showDeviceView = false;
    }

    private void SetPlayerMovement(bool canPlayerMove)
    {
        foreach (KeyValuePair<string, GameObject> p in ClientManager.instance.Players)
        {
            ClientManager.instance.Players[p.Key].GetComponent<ClientPlayer>().CanMove = canPlayerMove;
        }
    }
}
