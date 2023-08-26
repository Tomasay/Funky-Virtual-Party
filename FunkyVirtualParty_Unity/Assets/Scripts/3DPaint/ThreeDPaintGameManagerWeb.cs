using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaintIn3D;
using System.Runtime.InteropServices;
using System.Linq;

public class ThreeDPaintGameManagerWeb : GameManagerWeb
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void CloseInputKeyboard();

    [DllImport("__Internal")]
    private static extern void OnKeyboardInput();

    [DllImport("__Internal")]
    private static extern void UpdateInputFieldText(string txt);

    [DllImport("__Internal")]
    private static extern void SetPointerDownOnButton(bool isDown);
#endif

    [SerializeField]
    TMP_Text headerText;

    [SerializeField]
    TMP_Text timerText, inputTimerText;
    float drawTimeRemaining;
    bool drawTimerCountingDown;
    float answerTimeRemaining;
    bool answerTimerCountingDown;

    [SerializeField]
    P3dPaintableTexture paintTexture;

    [SerializeField]
    ThreeDPen pen;

    [SerializeField]
    PaintSprayGun sprayGun;

    [SerializeField]
    Canvas inputCanvas, guessingCanvas, resultsCanvas, leaderboardCanvas;

    [SerializeField]
    GameObject playerInputParent;

    [SerializeField]
    TMP_InputField answerInputField;

    [SerializeField]
    ButtonEvents answerInputButton;

    [SerializeField]
    KeyboardActivator answerInputActivator;

    [SerializeField]
    Button submitButton;

    [SerializeField]
    GameObject answerButtonPrefab, answerButtonParent, answerResultsParent;

    [SerializeField]
    GameObject drawingModel, linesParent;

    [SerializeField]
    Camera drawingPhaseCamera, guessingPhaseCamera;

    [SerializeField]
    GameObject leaderboardPlayerCardPrefab, leaderboardParent;
    List<GameObject> currentLeaderboardCards;

    private ThreeDPaintGameState state;
    public ThreeDPaintGameState State { get => state; set => state = value; }

    bool typingAnswer = false; //Is player typing their answer?
    bool playersAnswering = false; //Are we still waiting for any player to submit their answer?
    bool guessing = false; //Are players guessing?

    string chosenAnswerOwner;
    Dictionary<string, string> answers;
    List<AnswerOptionButton> answerButtons, answerResults;

    float vrPlayerPoints;
    Dictionary<string, int> playerPoints;

    bool vrTutorialCompleted;

    private void Awake()
    {
        playerPoints = new Dictionary<string, int>();

        currentLeaderboardCards = new List<GameObject>();
        answerButtons = new List<AnswerOptionButton>();
        answerResults = new List<AnswerOptionButton>();

        answerInputButton.onPointerDown.AddListener(ButtonPointerDown);
        answerInputButton.onPointerUp.AddListener(ButtonPointerUp);

        if (ClientManagerWeb.instance)
        {
            if (ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
            if (ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.On<string, string>("InfoToXR", InfoReceived);

            foreach (ClientPlayer cp in ClientManagerWeb.instance.Players)
            {
                playerPoints.Add(cp.PlayerSocketID, 0);
            }
        }

        inputCanvas.enabled = true;
        guessingCanvas.enabled = false;
        resultsCanvas.enabled = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClient");
        ClientManagerWeb.instance.Manager.Socket.Off("InfoToXR");
#endif
    }

    void Update()
    {
        if (typingAnswer)
        {
            answerInputField.ActivateInputField();
            answerInputField.caretPosition = answerInputField.text.Length;
        }

        if(guessing)
        {
            drawingModel.transform.Rotate(0, Time.deltaTime * 20, 0);
            linesParent.transform.Rotate(0, Time.deltaTime * 20, 0);
        }

        if(drawTimerCountingDown && drawTimeRemaining >= 0)
        {
            drawTimeRemaining -= Time.deltaTime;
            timerText.text = FormatTime(drawTimeRemaining);
        }

        if (answerTimerCountingDown && answerTimeRemaining >= 0)
        {
            answerTimeRemaining -= Time.deltaTime;
            inputTimerText.text = FormatTime(answerTimeRemaining);
        }
    }

    void InfoReceived(string info, string playerID)
    {
        if (playersAnswering)
        {
            //Add answer to list
            answers.Add(playerID, info);

            //If all answers are in but VR player is still going through tutorial
            if (answers.Count == ClientManagerWeb.instance.Players.Count && vrTutorialCompleted == false)
            {
                headerText.text = "Waiting for VR player to complete tutorial...";
            }
        }
    }

    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("SetNewPrompt"))
        {
            inputCanvas.enabled = true;
            answerInputField.text = "";
            headerText.text = data; //Set prompt text
            playerInputParent.SetActive(true); //Enable input
            playersAnswering = true;

            guessing = false;
            drawingModel.transform.rotation = Quaternion.Euler(0, 180, 0);
            linesParent.transform.rotation = Quaternion.identity;

            //Clear answers from previous round
            answers = new Dictionary<string, string>();
            ClearPlayerAnswers();

            //Reset any painting from previous round
            paintTexture.Clear();
            pen.EraseAllLines();

            //Reset results from previous round
            ClearPlayerResults();
        }
        else if (methodName.Equals("ShowBlurredView"))
        {
            inputCanvas.enabled = false;

            drawTimeRemaining = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;
            timerText.text = FormatTime(drawTimeRemaining);
            drawTimerCountingDown = true;

            drawingPhaseCamera.gameObject.SetActive(true);
            guessingPhaseCamera.gameObject.SetActive(false);
        }
        else if(methodName.Equals("ChosenAnswerOwner"))
        {
            chosenAnswerOwner = data;
        }
        else if (methodName.Equals("DoneDrawing"))
        {
            playersAnswering = false;
            guessing = true;

            guessingCanvas.enabled = true;

            drawTimerCountingDown = false;

            drawingPhaseCamera.gameObject.SetActive(false);
            guessingPhaseCamera.gameObject.SetActive(true);

            //Answer buttons
            foreach (KeyValuePair<string, string> entry in answers)
            {
                GameObject ab = Instantiate(answerButtonPrefab, answerButtonParent.transform);
                ab.GetComponent<Button>().onClick.AddListener(delegate { SubmitGuess(entry.Value, entry.Key); });
                AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                aob.SetText(entry.Value);
                aob.playerID = entry.Key;
                answerButtons.Add(aob);
            }

            //Answer results
            foreach (KeyValuePair<string, string> entry in answers)
            {
                GameObject ab = Instantiate(answerButtonPrefab, answerResultsParent.transform);
                AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                aob.SetText(entry.Value);
                aob.playerID = entry.Key;
                answerResults.Add(aob);
            }
        }
        else if(methodName.Equals("PlayerGuessed"))
        {
            //PlayerID:AnswerPlayerID
            string[] info = data.Split(':');

            AddPlayerToResults(info[0], info[1]);
            if (info[1].Equals(chosenAnswerOwner))
            {
                playerPoints[info[0]] += ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS;
            }
        }
        else if (methodName.Equals("VRGuessing"))
        {
            foreach (AnswerOptionButton aob in answerResults)
            {
                aob.SetColor(aob.playerID.Equals(chosenAnswerOwner) ? Color.green : Color.red);
            }

            resultsCanvas.enabled = true;
        }
        else if (methodName.Equals("GameOver"))
        {
            StartCoroutine("EndGame");
        }
        else if (methodName.Equals("VRPlayerGuessed"))
        {
            if (int.TryParse(data, out int newPoints))
            {
                vrPlayerPoints = newPoints;
            }

            StartCoroutine("DisplayLeaderboard");
        }
        else if (methodName.Equals("VRDoneWithTutorial"))
        {
            vrTutorialCompleted = true;
        }
    }

    protected override void OnStateChange(string s)
    {
        if (System.Enum.TryParse(s, out ThreeDPaintGameState newGameState))
        {
            State = newGameState;

            switch (State)
            {
                case ThreeDPaintGameState.ClientsAnswering:
                    answerTimeRemaining = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;
                    answerTimerCountingDown = true;
                    break;
                case ThreeDPaintGameState.VRPosing:
                    headerText.text = "Waiting for VR player to set a pose...";
                    break;
                case ThreeDPaintGameState.VRPainting:
                    break;
                case ThreeDPaintGameState.ClientsGuessing:
                    //Make sure there's no lingering drawings
                    pen.CanPaint = false;
                    sprayGun.CanPaint = false;
                    break;
                case ThreeDPaintGameState.VRGuessing:
                    answerTimerCountingDown = false;
                    break;
                case ThreeDPaintGameState.ShowingLeaderboard:
                    break;
                case ThreeDPaintGameState.GameOver:
                    break;
                default:
                    break;
            }
        }
    }

    public void OpenKeyboard()
    {
        typingAnswer = true;
    }

    public void UpdateText(string input)
    {
        answerInputField.text = input;
    }

    public void ButtonPointerDown()
    {
#if UNITY_WEBGL
        SetPointerDownOnButton(true);
#endif
    }

    public void ButtonPointerUp()
    {
        StartCoroutine("ButtonDown");
    }

    IEnumerator ButtonDown()
    {
        yield return new WaitForSeconds(1);
#if UNITY_WEBGL
        SetPointerDownOnButton(false);
#endif
    }

    public void CheckAnswerInput(string input)
    {
        submitButton.interactable = (input.Length > 0);
    }

    public void SetField(TMP_InputField f)
    {
#if UNITY_WEBGL
        UpdateInputFieldText(f.text);
#endif
        
    }

    public void SubmitAnswer()
    {
        if (ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.Emit("InfoFromPlayer", answerInputField.text);

        typingAnswer = false;
#if UNITY_WEBGL
        CloseInputKeyboard();
#endif
        answerInputField.DeactivateInputField();

        headerText.text = "Waiting for all players to submit their answer...";
        playerInputParent.SetActive(false);
    }

    void SubmitGuess(string guess, string guessPlayerID)
    {
        if (ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.Emit("InfoFromPlayer", guessPlayerID);
        guessingCanvas.enabled = false;
    }

    void AddPlayerToResults(string playerID, string answerPlayerID)
    {
        //Check each answer
        foreach (AnswerOptionButton aob in answerResults)
        {
            //Find the answer that the player chose
            if (aob.playerID.Equals(answerPlayerID))
            {
                ClientManagerWeb inst = ClientManagerWeb.instance;
                aob.AddPlayerIcon(inst.GetPlayerBySocketID(playerID).syncer.Name, inst.GetPlayerBySocketID(playerID).syncer.Color);
                return;
            }
        }
    }

    void ClearPlayerResults()
    {
        foreach (Button b in answerResultsParent.GetComponentsInChildren<Button>())
        {
            Destroy(b.gameObject);
        }
    }

    void ClearPlayerAnswers()
    {
        foreach (AnswerOptionButton aob in answerButtons)
        {
            Destroy(aob.gameObject);
        }
        answerButtons = new List<AnswerOptionButton>();

        foreach (AnswerOptionButton aob in answerResults)
        {
            Destroy(aob.gameObject);
        }
        answerResults = new List<AnswerOptionButton>();
    }

    IEnumerator DisplayLeaderboard()
    {
        resultsCanvas.enabled = false;

        //Clear previous leaderboard
        foreach (GameObject g in currentLeaderboardCards)
        {
            Destroy(g);
        }
        currentLeaderboardCards = new List<GameObject>();

        leaderboardCanvas.enabled = true;

        //Sort player points
        IOrderedEnumerable<KeyValuePair<string, int>> sortedDict = from entry in playerPoints orderby entry.Value descending select entry;

        int vrPlayerPos = 0;
        //Add player cards
        foreach (KeyValuePair<string, int> entry in sortedDict)
        {
            GameObject newCard = Instantiate(leaderboardPlayerCardPrefab, leaderboardParent.transform);
            newCard.GetComponentsInChildren<TMP_Text>()[0].text = ClientManagerWeb.instance.GetPlayerBySocketID(entry.Key).syncer.Name;
            newCard.GetComponentsInChildren<TMP_Text>()[1].text = answers[entry.Key];
            newCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + entry.Value;

            currentLeaderboardCards.Add(newCard);

            if (vrPlayerPoints < entry.Value)
            {
                vrPlayerPos++;
            }
        }

        //Add VR Card
        GameObject vrCard = Instantiate(leaderboardPlayerCardPrefab, leaderboardParent.transform);
        vrCard.GetComponentsInChildren<TMP_Text>()[0].text = "VR Player";
        vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "";
        vrCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + vrPlayerPoints;
        vrCard.transform.SetSiblingIndex(vrPlayerPos);

        currentLeaderboardCards.Add(vrCard);

        yield return new WaitForSeconds(5);

        //Disable leaderboard
        leaderboardCanvas.enabled = false;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5);

        ClientManagerWeb.instance.LoadMainMenu();
    }
}