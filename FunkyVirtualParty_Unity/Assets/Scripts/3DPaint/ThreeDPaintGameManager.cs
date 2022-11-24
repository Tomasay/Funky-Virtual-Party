using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaintIn3D;

public enum ThreeDPaintGameState
{
    ClientsAnswering,
    VRPainting,
    ClientsGuessing,
    VRGuessing,
    GameOver
}

public class ThreeDPaintGameManager : GameManager
{
    private ThreeDPaintGameState state;
    public ThreeDPaintGameState State { get => state; set { state = value; ClientManager.instance.Manager.Socket.Emit("gameStateFromHost", "" + state); OnStateChange(); } }

    [SerializeField]
    TextAsset promptList;

    [SerializeField]
    TMP_Text headerText, timerText;

    [SerializeField]
    P3dPaintableTexture paintTexture;

    [SerializeField]
    ThreeDPen pen;

    [SerializeField]
    PaintSprayGun sprayGun;

    [SerializeField]
    GameObject playerNameIconPrefab, playerNamesIconParent;

    Dictionary<string, string> answers;

    private string chosenAnswer, chosenAnswerOwner;

    private int playersGuessed = 0;

    float drawTimeRemaining;
    const int DRAW_TIME_AMOUNT = 30;

    private void Awake()
    {
        answers = new Dictionary<string, string>();

        headerText.text = "";
        timerText.text = "";

        drawTimeRemaining = DRAW_TIME_AMOUNT;

        if (ClientManager.instance) ClientManager.instance.Manager.Socket.On<string, string>("InfoToXR", InfoReceived);

        StartCoroutine("StartGame");
    }

    private void Update()
    {
        switch (state)
        {
            case ThreeDPaintGameState.ClientsAnswering:
                break;
            case ThreeDPaintGameState.VRPainting:
                //Draw timer
                drawTimeRemaining -= Time.deltaTime;
                timerText.text = FormatTime(drawTimeRemaining);

                if(drawTimeRemaining <= 0)
                {
                    State = ThreeDPaintGameState.ClientsGuessing;
                }
                break;
            case ThreeDPaintGameState.ClientsGuessing:
                break;
            case ThreeDPaintGameState.VRGuessing:
                break;
            default:
                break;
        }
    }

    void OnStateChange()
    {
        switch (state)
        {
            case ThreeDPaintGameState.ClientsAnswering:
                //Display text that players are answering
                headerText.text = "Players are typing their answers \nUse this time to practice painting!";

                //Animate client players to look like they are on their phones

                break;
            case ThreeDPaintGameState.VRPainting:
                //Display answer
                headerText.text = "Paint: " + chosenAnswer;

                //Show blurred view to clients
                ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ShowBlurredView", "");

                //Clear practice painting
                pen.EraseAllLines();
                paintTexture.Clear();

                break;
            case ThreeDPaintGameState.ClientsGuessing:
                //Communicate to clients
                ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "DoneDrawing", "");

                //Disable VR tools
                pen.canPaint = false;
                sprayGun.canPaint = false;

                //Display all answers
                headerText.enabled = false;
                timerText.enabled = false;
                playerNamesIconParent.SetActive(true);
                break;
            case ThreeDPaintGameState.VRGuessing:
                //Show each answer over the appropriate player

                break;
            case ThreeDPaintGameState.GameOver:
                StartCoroutine("EndGame");
                break;
            default:
                break;
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);

        if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "SetNewPrompt", GetPrompt());
        State = ThreeDPaintGameState.ClientsAnswering;
    }

    IEnumerator EndGame()
    {
        if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "GameOver", "");

        yield return new WaitForSeconds(5);

        ClientManager.instance.LoadMainMenu();
    }

    void InfoReceived(string info, string playerID)
    {
        if (State == ThreeDPaintGameState.ClientsAnswering)
        {
            answers.Add(playerID, info);

            if (answers.Count >= ClientManager.instance.Players.Count)
            {
                string randomPlayer = ClientManager.instance.Players[Random.Range(0, ClientManager.instance.Players.Count)].PlayerID;
                chosenAnswer = answers[randomPlayer];
                chosenAnswerOwner = randomPlayer;

                State = ThreeDPaintGameState.VRPainting;
            }
        }
        else if (State == ThreeDPaintGameState.ClientsGuessing)
        {
            //If player guessed right
            if (answers[info].Equals(answers[chosenAnswerOwner]))
            {
                if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "GuessedRight", playerID);
                AddPlayerToResults(playerID, true);
            }
            else
            {
                if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "GuessedWrong", playerID);
                AddPlayerToResults(playerID, false);
            }

            playersGuessed++;
            if(playersGuessed >= ClientManager.instance.Players.Count)
            {
                State = ThreeDPaintGameState.GameOver;
            }
        }
    }

    string GetPrompt()
    {
        string[] prompts = promptList.ToString().Split('\n');
        return prompts[Random.Range(0, prompts.Length)];
    }

    void AddPlayerToResults(string playerID, bool correct)
    {
        GameObject pi = Instantiate(playerNameIconPrefab, playerNamesIconParent.transform);
        pi.GetComponentInChildren<TMP_Text>(true).text = ClientManager.instance.GetPlayerByID(playerID).PlayerName;
        pi.GetComponentInChildren<Button>(true).interactable = false;
        pi.GetComponent<Image>().color = correct ? Color.green : Color.red;
    }
}