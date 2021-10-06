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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cm.OnMinigameStart("ChaseGame");
            SceneManager.LoadScene("ChaseGame");
        }
    }

    public void StartChaseGame()
    {
        cm.OnMinigameStart("ChaseGame");
        SceneManager.LoadScene("ChaseGame");
    }
}
