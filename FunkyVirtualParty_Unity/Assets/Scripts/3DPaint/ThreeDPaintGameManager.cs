using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaintIn3D;
using Autohand;

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
    Hand leftHand, rightHand;

    [SerializeField]
    GameObject UIPointer;

    [SerializeField]
    GameObject playerNameIconPrefab, playerNamesIconParent;
    List<GameObject> playerNameIcons;

    Dictionary<string, string> answers;

    private string chosenAnswer, chosenAnswerOwner;

    private int playersGuessed = 0;

    float drawTimeRemaining;
    const int DRAW_TIME_AMOUNT = 30;

    float vrPlayerPoints;
    Dictionary<string, int> playerPoints;

    const int POINTS_VR_CORRECT_GUESSES = 50; //Every player that correctly guesses what the VR player's drawing is
    const int POINTS_VR_CORRECT_PLAYER = 50; //Correctly guessing which player wrote the chosen answer
    const int POINTS_CLIENT_CORRECT_GUESS = 100; //Correctly guessing what the VR player drew
    const int POINTS_CLIENT_OTHER_CORRECT_GUESSES = 50; //Every player the successfully chose the chosen player's answer

    private void Awake()
    {
        answers = new Dictionary<string, string>();
        playerPoints = new Dictionary<string, int>();

        playerNameIcons = new List<GameObject>();

        headerText.text = "";
        timerText.text = "";

        drawTimeRemaining = DRAW_TIME_AMOUNT;

        if (ClientManager.instance)
        {
            foreach (ClientPlayer cp in ClientManager.instance.Players)
            {
                playerPoints.Add(cp.PlayerID, 0);
            }

            ClientManager.instance.Manager.Socket.On<string, string>("InfoToXR", InfoReceived);
        }

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
                headerText.text = "Clients are guessing what your art is";
                timerText.enabled = false;
                playerNamesIconParent.SetActive(true);
                break;
            case ThreeDPaintGameState.VRGuessing:
                headerText.text = "Which player do you think wrote that answer?";
                foreach (GameObject g in playerNameIcons)
                {
                    g.GetComponentInChildren<Button>().interactable = true;
                }
                UIPointer.SetActive(true);
                leftHand.Release();
                rightHand.Release();

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

                vrPlayerPoints += POINTS_VR_CORRECT_GUESSES;
                playerPoints[playerID] += POINTS_CLIENT_CORRECT_GUESS;
            }
            else
            {
                if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "GuessedWrong", playerID);
                AddPlayerToResults(playerID, false);
            }

            playersGuessed++;
            if(playersGuessed >= ClientManager.instance.Players.Count)
            {
                State = ThreeDPaintGameState.VRGuessing;
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
        pi.GetComponentInChildren<Button>(true).onClick.AddListener(delegate { GuessPlayerVR(playerID); });
        pi.GetComponentInChildren<Button>(true).interactable = false;
        pi.GetComponent<Image>().color = correct ? Color.green : Color.red;

        playerNameIcons.Add(pi);
    }

    /// <summary>
    /// Method called when VR player guesses which player wrote the selected prompt
    /// </summary>
    public void GuessPlayerVR(string playerID)
    {
        if(playerID.Equals(chosenAnswerOwner))
        {
            vrPlayerPoints += POINTS_VR_CORRECT_PLAYER;
            headerText.text = "Correct!";
        }
        else
        {
            headerText.text = "Wrong! " + ClientManager.instance.GetPlayerByID(chosenAnswerOwner).PlayerName + " wrote the answer";
        }

        State = ThreeDPaintGameState.GameOver;
    }
}