using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;

public enum GameState
{
    Tutorial,
    Countdown,
    GameLoop,
    VRPlayerLoses,
    VRPlayerWins,
    TimeEnded
}

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform[] playerSpawns;
    [SerializeField] GameObject VRPlayer;

    public Vector3 VRPlayerPos { get => VRPlayer.GetComponentInChildren<AutoHandPlayer>().transform.position; }

    private GameState state;
    public GameState State { get => state; set{ state = value; ClientManager.instance.Manager.Socket.Emit("gameStateFromHost", "" + state); } }

    protected virtual void Start()
    {
        if (ClientManager.instance)
        {
            ClientManager.instance.SpawnPlayers(playerPrefab, playerSpawns);
            State = GameState.Tutorial;

            //ClientManager.instance.Manager.Socket.On<string>("action", OnAction);
        }
        else
        {
            Debug.LogError("Client Manager Instance Not Found!");
        }
    }

    public virtual void OnAction(string id)
    {
        ClientManager.instance.GetPlayerByID(id).GetComponent<ClientPlayer>().Action();
    }

    public IEnumerator DisableMirroring()
    {
        yield return new WaitForEndOfFrame();
        //UnityEngine.VR.VRSettings.showDeviceView = false;
        XRSettings.showDeviceView = false;
    }

    protected void SetPlayerMovement(bool canPlayerMove)
    {
        if(!ClientManager.instance)
        {
            return;
        }

        foreach (ClientPlayer p in ClientManager.instance.Players)
        {
            p.CanMove = canPlayerMove;
        }
    }

    protected void SetVRPlayerMovement(bool canPlayerMove)
    {
        VRPlayer.GetComponentInChildren<AutoHandPlayer>().canMove = canPlayerMove;
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes*60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}