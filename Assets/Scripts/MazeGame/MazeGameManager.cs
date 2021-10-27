using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;

public class MazeGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform[] playerSpawns;

    void Start()
    {
        ClientManager.instance.SpawnPlayers(playerPrefab, playerSpawns, true);
    }

    void Update()
    {
        
    }
}
