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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cm.OnMinigameStart("ChaseGame");
            SceneManager.LoadScene("ChaseGame");
        }
    }
}
