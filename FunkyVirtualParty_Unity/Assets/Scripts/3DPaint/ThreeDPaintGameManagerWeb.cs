using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaintIn3D;
using System.Runtime.InteropServices;
using System.Linq;

public class ThreeDPaintGameManagerWeb : MonoBehaviour
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
    MannequinSolverClient mannequinSolver;

    [SerializeField]
    GameObject leaderboardPlayerCardPrefab, leaderboardParent;
    List<GameObject> currentLeaderboardCards;

    bool typingAnswer = false; //Is player typing their answer?
    bool playersAnswering = false; //Are we still waiting for any player to submit their answer?
    bool guessing = false; //Are players guessing?

    List<AnswerOptionButton> answerButtons, answerResults;

    float vrPlayerPoints;
    Dictionary<string, int> playerPoints;

    private void Awake()
    {
        playerPoints = new Dictionary<string, int>();

        currentLeaderboardCards = new List<GameObject>();
        answerButtons = new List<AnswerOptionButton>();
        answerResults = new List<AnswerOptionButton>();

        answerInputButton.onPointerDown.AddListener(ButtonPointerDown);
        answerInputButton.onPointerUp.AddListener(ButtonPointerUp);

        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            playerPoints.Add(cp.realtimeView.viewUUID, 0);
        }

        inputCanvas.enabled = true;
        guessingCanvas.enabled = false;
        resultsCanvas.enabled = false;
    }

    private void Start()
    {
        VRtistrySyncer.instance.OnPromptChangedEvent.AddListener(SetNewPrompt);
        VRtistrySyncer.instance.OnPlayerAnswered.AddListener(PlayerSubmittedAnswer);
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

    void PlayerSubmittedAnswer(string answers)
    {
        //Check to see if all players have answered and if we're waiting on vr player
        string[] answersSeparated = answers.Split('\n');

        if (answersSeparated.Length == ClientPlayer.clients.Count && VRtistrySyncer.instance.VRCompletedTutorial == false)
        {
            headerText.text = "Waiting for VR player to complete tutorial...";
        }
    }

    void SetNewPrompt(string p)
    {
        inputCanvas.enabled = true;
        answerInputField.text = "";
        headerText.text = p; //Set prompt text
        playerInputParent.SetActive(true); //Enable input
        playersAnswering = true;

        guessing = false;
        drawingModel.transform.rotation = Quaternion.Euler(0, 180, 0);
        linesParent.transform.rotation = Quaternion.identity;

        //Clear answers from previous round
        ClearPlayerAnswers();

        //Reset any painting from previous round
        paintTexture.Clear();
        pen.EraseAllLines();

        //Reset results from previous round
        ClearPlayerResults();
    }

    protected void OnStateChange(string s)
    {
        switch (s)
        {
            case "clients answering":
                answerTimeRemaining = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;
                answerTimerCountingDown = true;
                break;
            case "vr posing":
                headerText.text = "Waiting for VR player to set a pose...";
                break;
            case "vr painting":
                //Bake mannequin IK
                mannequinSolver.SetPoseColliders();

                //Show blurred view
                inputCanvas.enabled = false;

                drawTimeRemaining = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;
                timerText.text = FormatTime(drawTimeRemaining);
                drawTimerCountingDown = true;

                drawingPhaseCamera.gameObject.SetActive(true);
                guessingPhaseCamera.gameObject.SetActive(false);
                break;
            case "clients guessing":
                //Make sure there's no lingering drawings
                pen.CanPaint = false;
                sprayGun.CanPaint = false;

                playersAnswering = false;
                guessing = true;

                guessingCanvas.enabled = true;

                drawTimerCountingDown = false;

                drawingPhaseCamera.gameObject.SetActive(false);
                guessingPhaseCamera.gameObject.SetActive(true);

                
                string[] answersSeparated = VRtistrySyncer.instance.Answers.Split('\n');

                //Answer buttons
                foreach (string a in answersSeparated)
                {
                    string[] ownerAndAnswer = a.Split(':');

                    GameObject ab = Instantiate(answerButtonPrefab, answerButtonParent.transform);
                    ab.GetComponent<Button>().onClick.AddListener(delegate { SubmitGuess(RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.viewUUID, ownerAndAnswer[0]); });
                    AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                    aob.SetText(ownerAndAnswer[1]);
                    aob.playerID = ownerAndAnswer[0];
                    answerButtons.Add(aob);
                }

                //Answer results
                foreach (string a in answersSeparated)
                {
                    string[] ownerAndAnswer = a.Split(':');

                    GameObject ab = Instantiate(answerButtonPrefab, answerResultsParent.transform);
                    AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                    aob.SetText(ownerAndAnswer[1]);
                    aob.playerID = ownerAndAnswer[0];
                    answerResults.Add(aob);
                }
                break;
            case "vr guessing":
                //All players have guessed, so add guesses to results
                string[] guessesSeparated = VRtistrySyncer.instance.Guesses.Split('\n');
                foreach (string g in guessesSeparated)
                {
                    string[] guessOwnerAndGuess = g.Split(':');

                    AddPlayerToResults(guessOwnerAndGuess[0], guessOwnerAndGuess[1]);
                }

                //Display results
                foreach (AnswerOptionButton aob in answerResults)
                {
                    aob.SetColor(aob.playerID.Equals(VRtistrySyncer.instance.ChosenAnswerOwner) ? Color.green : Color.red);
                }

                resultsCanvas.enabled = true;

                answerTimerCountingDown = false;
                break;
            case "leaderboard":
                /*
                if (int.TryParse(data, out int newPoints))
                {
                    vrPlayerPoints = newPoints;
                }
                */

                StartCoroutine("DisplayLeaderboard");
                break;
            case "game over":
                break;
            default:
                break;
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
        if (VRtistrySyncer.instance.Answers.Equals(""))
        {
            VRtistrySyncer.instance.Answers = (RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.viewUUID + ":" + answerInputField.text);
        }
        else
        {
            VRtistrySyncer.instance.Answers += ("\n" + RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.viewUUID + ":" + answerInputField.text);
        }

        typingAnswer = false;
#if UNITY_WEBGL
        CloseInputKeyboard();
#endif
        answerInputField.DeactivateInputField();

        headerText.text = "Waiting for all players to submit their answer...";
        playerInputParent.SetActive(false);
    }

    void SubmitGuess(string clientID, string clientGuessID)
    {
        if(VRtistrySyncer.instance.Guesses.Equals(""))
        {
            VRtistrySyncer.instance.Guesses = clientID + ":" + clientGuessID;
        }
        else
        {
            VRtistrySyncer.instance.Guesses += "\n" + clientID + ":" + clientGuessID;
        }

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
                ClientPlayer cp = ClientPlayer.GetClientByViewID(playerID);
                aob.AddPlayerIcon(cp.syncer.Name, cp.syncer.Color);
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

    string GetAnswerByViewID(string ID)
    {
        string[] answersSeparated = VRtistrySyncer.instance.Answers.Split('\n');

        //Answer buttons
        foreach (string a in answersSeparated)
        {
            string[] ownerAndAnswer = a.Split(':');

            if(ID.Equals(ownerAndAnswer[0]))
            {
                return ownerAndAnswer[1];
            }
        }

        return "";
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
            newCard.GetComponentsInChildren<TMP_Text>()[0].text = ClientPlayer.GetClientByViewID(entry.Key).syncer.Name;
            newCard.GetComponentsInChildren<TMP_Text>()[1].text = GetAnswerByViewID(entry.Key);
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

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}