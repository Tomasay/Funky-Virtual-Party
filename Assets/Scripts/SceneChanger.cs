using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] ClientManager cm;

    private void Start()
    {
        cm = GameObject.Find("ClientManager").GetComponent<ClientManager>();
    }

    public void StartChaseGame()
    {
        cm.OnMinigameStart("ChaseGame");
        SceneManager.LoadScene("ChaseGame");
    }
}
