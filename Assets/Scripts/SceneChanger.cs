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

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.C))
        {
            cm.OnMinigameStart("ChaseGame");
            SceneManager.LoadScene("ChaseGame");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            cm.OnMinigameStart("MazeGame");
            SceneManager.LoadScene("MazeGame");
        }
        */
    }

    public void StartChaseGame()
    {
        cm.OnMinigameStart("ChaseGame");
        SceneManager.LoadScene("ChaseGame");
    }
}
