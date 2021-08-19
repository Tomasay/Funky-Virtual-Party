using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClientPlayer : MonoBehaviour
{
    [SerializeField]
    TMP_Text playerNameText;

    protected string playerID, playerName;
    protected Color playerColor = Color.clear;
    protected Vector3 movement;
    protected float startingSpeed = 5, speed;
    protected bool canMove = true;

    public string PlayerID { get => playerID; set => playerID = value; }
    public string PlayerName { get => playerName; set { playerNameText.text = playerName = value; } }
    public Color PlayerColor { get => playerColor; set{ playerColor = value; GetComponent<Renderer>().material.SetColor("_BaseColor", value); } }

    public bool CanMove { get => canMove; set => canMove = value; }

    protected virtual void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        if(playerColor == Color.clear)
            playerColor = Random.ColorHSV();
        PlayerColor = playerColor;
        speed = startingSpeed;
    }
    
    protected virtual void Update()
    {
        transform.Translate(movement * Time.deltaTime);
    }

    public void Move(int x, int y)
    {
        if (canMove)
        {
            movement = new Vector3((x / 100.0f) * speed, 0, (y / 100.0f) * speed);
            //Debug.Log("SPEED: " + movement);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLIDING WITH: " + collision.gameObject.name + " WITH TAG " + collision.gameObject.tag);
    }
}