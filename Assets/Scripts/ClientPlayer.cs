using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClientPlayer : MonoBehaviour
{
    [SerializeField]
    TMP_Text playerNameText;

    private string playerID, playerName;
    private Color playerColor = Color.clear;
    private Vector3 movement;
    private float speed = 5;
    private bool canMove = true;

    public string PlayerID { get => playerID; set => playerID = value; }
    public string PlayerName { get => playerName; set { playerNameText.text = playerName = value; } }
    public Color PlayerColor { get => playerColor; set{ playerColor = value; GetComponent<Renderer>().material.SetColor("_BaseColor", value); } }

    public bool CanMove { get => canMove; set => canMove = value; }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        if(playerColor == Color.clear)
            playerColor = Random.ColorHSV();
        PlayerColor = playerColor;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(movement * Time.deltaTime);
    }

    public void Move(int x, int y)
    {
        if (canMove)
        {
            movement = new Vector3((x / 100.0f) * speed, 0, (y / 100.0f) * speed);
            Debug.Log("SPEED: " + movement);
        }
    }
}