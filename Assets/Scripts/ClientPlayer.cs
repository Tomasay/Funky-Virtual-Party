using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayer : MonoBehaviour
{
    private string playerID;
    private Vector3 movement;
    private float speed = 5;

    public string PlayerID { get => playerID; set => playerID = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(movement * Time.deltaTime);
    }

    public void Move(int x, int y)
    {
        movement = new Vector3((x / 100.0f) * speed, 0, (y / 100.0f) * speed);
        Debug.Log("SPEED: " + movement);
    }
}