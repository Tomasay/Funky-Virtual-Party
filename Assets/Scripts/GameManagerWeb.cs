using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;

public class GameManagerWeb : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] GameObject VRPlayer;

    protected bool endingGame = false; //Set to true when coroutine has been started to load main menu. Used to prevent trying to load more than once

    public Vector3 VRPlayerPos { get => VRPlayer.GetComponentInChildren<AutoHandPlayer>().transform.position; }

    private GameState state;
    public GameState State { get => state; set => state = value; }

    protected virtual void Start()
    {
        ClientManagerWeb.instance.SpawnPlayers(playerPrefab);
        State = GameState.Tutorial;

        ClientManagerWeb.instance.Manager.Socket.On<string>("gameStateToClient", OnStateChange);
    }

    protected void SetPlayerMovement(bool canPlayerMove)
    {
        foreach (ClientPlayer p in ClientManagerWeb.instance.Players)
        {
            p.CanMove = canPlayerMove;
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    protected void OnStateChange(string s)
    {
        if(System.Enum.TryParse(s, out GameState newGameState))
        {
            State = newGameState;
        }
    }
}