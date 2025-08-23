using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using PaintIn3D;
using System.Runtime.InteropServices;
using System.Linq;
using Lean.Touch;
using Shapes;

public class ThreeDPaintGameManagerWeb : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
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
    TMP_Text inputHeaderText, blurHeaderText, guessingHeaderText;

    [SerializeField]
    TMP_Text blurTimerText, inputTimerText;

    [SerializeField]
    Image blurImage;

    [SerializeField]
    P3dPaintableTexture paintTexture;

    [SerializeField]
    Canvas inputCanvas, guessingCanvas, leaderboardCanvas;

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

    Quaternion drawingModelStartingRot;

    [SerializeField]
    Camera drawingPhaseCamera, guessingPhaseCamera, galleryCamera;

    [SerializeField]
    MannequinSolverClient mannequinSolver;

    [SerializeField]
    GameObject leaderboardPlayerCardPrefab, leaderboardParent, galleryLeaderboardParent;
    List<GameObject> currentLeaderboardCards;

    [SerializeField]
    LeanTouch leanTouch;

    [SerializeField]
    GameObject tapAndHoldRotateTutorial;
    bool tapAndHoldRotateLearned;

    bool typingAnswer = false; //Is player typing their answer?
    bool playersAnswering = false; //Are we still waiting for any player to submit their answer?
    bool guessing = false; //Are players guessing?

    List<AnswerOptionButton> answerButtons, answerResults;

    int answerOwnerIDPlayerIsGuessing;

    PaintBrush paintBrush;

    private void Awake()
    {
        currentLeaderboardCards = new List<GameObject>();
        answerButtons = new List<AnswerOptionButton>();
        answerResults = new List<AnswerOptionButton>();

        drawingModelStartingRot = drawingModel.transform.rotation;

        answerInputButton.onPointerDown.AddListener(ButtonPointerDown);
        answerInputButton.onPointerUp.AddListener(ButtonPointerUp);

        inputCanvas.enabled = true;
        guessingCanvas.enabled = false;

        LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;

        Draw.Position = linesParent.transform.position;

        RealtimeSingletonWeb.instance.LocalPlayerSpawned.AddListener(OnLocalPlayerSpawned);
    }

    void OnLocalPlayerSpawned()
    {
        VRtistryClientPlayer vcp = (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer);
        if (vcp && vcp.usingPhone == 0)
        {
            vcp.SetSitAnim();
            vcp.TogglePhone();
        }
    }

    private void LeanTouch_OnFingerDown(LeanFinger obj)
    {
        if (paintBrush.revealAnimationComplete)
        {
            tapAndHoldRotateLearned = true;
            tapAndHoldRotateTutorial.SetActive(false);
        }
    }

    private void Start()
    {
        VRtistrySyncer.instance.OnStateChangeEvent.AddListener(OnStateChange);
        VRtistrySyncer.instance.OnPromptChangedEvent.AddListener(SetNewPrompt);
        VRtistrySyncer.instance.OnPlayerAnswered.AddListener(PlayerSubmittedAnswer);
        VRtistrySyncer.instance.OnDecoyAnswersChanged.AddListener(SetDecoyAnswers);
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChange);
        VRtistrySyncer.instance.OnPromptChangedEvent.RemoveListener(SetNewPrompt);
        VRtistrySyncer.instance.OnPlayerAnswered.RemoveListener(PlayerSubmittedAnswer);

        answerInputButton.onPointerDown.RemoveListener(ButtonPointerDown);
        answerInputButton.onPointerUp.RemoveListener(ButtonPointerUp);

        LeanTouch.OnFingerDown -= LeanTouch_OnFingerDown;

        RealtimeSingletonWeb.instance.LocalPlayerSpawned.RemoveListener(OnLocalPlayerSpawned);
    }

    void Update()
    {
        if (typingAnswer)
        {
            answerInputField.ActivateInputField();
            answerInputField.caretPosition = answerInputField.text.Length;
        }

        if(guessing && paintBrush.revealAnimationComplete)
        {
            if(tapAndHoldRotateLearned)
            {
                Draw.Rotation = Quaternion.Euler(0, drawingModel.transform.localRotation.eulerAngles.z, 0);
            }
            else
            {
                drawingModel.transform.Rotate(0, 0, Time.deltaTime * 20);
                Draw.Rotate(0, Mathf.Deg2Rad * (Time.deltaTime * 20), 0);
            }
        }

        if (VRtistrySyncer.instance.State.Equals("vr painting"))
        {
            if (VRtistrySyncer.instance.DrawingTimer >= 0)
            {
                if (blurTimerText.enabled) blurTimerText.text = FormatTime(VRtistrySyncer.instance.DrawingTimer);
            }
        }
        else
        {
            if (VRtistrySyncer.instance.ClientAnswerTimer >= 0)
            {
                if (inputTimerText.enabled) inputTimerText.text = FormatTime(VRtistrySyncer.instance.ClientAnswerTimer);
                if (blurTimerText.enabled) blurTimerText.text = FormatTime(VRtistrySyncer.instance.ClientAnswerTimer);
            }
        }
    }

    void PlayerSubmittedAnswer(string answers)
    {
        //Check to see if all players have answered and if we're waiting on vr player
        string[] answersSeparated = answers.Split('\n');

        if (answersSeparated.Length == ClientPlayer.clients.Count && VRtistrySyncer.instance.VRCompletedTutorial == false)
        {
            inputTimerText.enabled = false;
            blurTimerText.enabled = false;
            blurHeaderText.text = "Waiting for VR player to complete tutorial...";
        }
        else if(answersSeparated.Length != ClientPlayer.clients.Count)
        {
            blurHeaderText.text = "Waiting for all players to submit their answer...";
        }
    }

    void SetNewPrompt(string p)
    {
        if(p.Equals(""))
        {
            return;
        }

        inputCanvas.enabled = true;
        answerInputField.text = "";
        blurHeaderText.text = "";
        inputHeaderText.text = p; //Set prompt text
        playerInputParent.SetActive(true); //Enable input
        playersAnswering = true;

        guessing = false;
        drawingModel.transform.rotation = drawingModelStartingRot;
        Draw.Rotation = Quaternion.identity;
        if(paintBrush) paintBrush.revealAnimationComplete = false;

        //Clear answers from previous round
        ClearPlayerAnswers();

        //Reset any painting from previous round
        paintTexture.Clear();
        DrawingsSyncer.instance.paintSyncer.ResetStoredHitLines();

        //Reset results from previous round
        ClearPlayerResults();
    }

    protected void OnStateChange(string s)
    {
        switch (s)
        {
            case "clients answering":
                ResetAnswerResultsBubbles();

                inputTimerText.enabled = true;

                //Enable phone anim for local player, which will then be synced for everyone else
                VRtistryClientPlayer vcp = (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer);
                //There is a chance that this code runs before client has spawned in, so have to check if vcp is null
                //There is also a check on player spawn to enable the client's phone animation when starting the game
                if (vcp && vcp.usingPhone == 0)
                {
                    vcp.TogglePhone();
                }

                blurImage.enabled = true;
                break;
            case "vr posing":
                if (!paintBrush)
                {
                    paintBrush = FindAnyObjectByType<PaintBrush>();
                    paintBrush.LinesParent = linesParent.transform;
                    paintBrush.paintTexture = paintTexture;
                    paintBrush.OnRevealAnimationComplete.AddListener(OnRevealAnimationComplete);
                }

                inputTimerText.enabled = false;
                blurTimerText.enabled = false;

                //Show blurred view
                inputCanvas.enabled = false;

                blurHeaderText.text = "Waiting for VR player to set a pose...";

                //Reset any painting from practicing
                paintTexture.Clear();
                DrawingsSyncer.instance.paintSyncer.ResetStoredHitLines();
                break;
            case "vr painting":
                //Bake mannequin IK
                mannequinSolver.SetPoseColliders();

                blurTimerText.enabled = true;
                blurTimerText.text = FormatTime(VRtistrySyncer.instance.DrawingTimer);
                blurHeaderText.text = "VR Player is creating a masterpiece...";

                drawingPhaseCamera.gameObject.SetActive(true);
                guessingPhaseCamera.gameObject.SetActive(false);
                break;
            case "clients guessing":
                guessingHeaderText.text = "What is it?";

                (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer).TogglePhone();

                blurTimerText.enabled = false;
                inputTimerText.enabled = false;

                //Make sure there's no lingering drawings
                paintBrush.CanPaintAir = false;

                playersAnswering = false;
                guessing = true;

                guessingCanvas.enabled = true;

                blurImage.enabled = false;

                drawingPhaseCamera.gameObject.SetActive(false);
                guessingPhaseCamera.gameObject.SetActive(true);

                string[] answersSeparated = VRtistrySyncer.instance.Answers.Split('\n');

                //Answer buttons
                answerButtonParent.SetActive(false);
                foreach (string a in answersSeparated)
                {
                    string[] ownerAndAnswer = a.Split(':');

                    GameObject ab = Instantiate(answerButtonPrefab, answerButtonParent.transform);
                    string guessGuessOwner = (RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf + ":" + ownerAndAnswer[0]);
                    ab.GetComponent<Button>().onClick.AddListener(delegate { SubmitArtGuess(RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf, ownerAndAnswer[0]); });
                    AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                    aob.SetText(ownerAndAnswer[1]);
                    aob.playerID = ownerAndAnswer[0];
                    answerButtons.Add(aob);
                }

                //Answer results
                foreach (string a in answersSeparated)
                {
                    string[] ownerAndAnswer = a.Split(':');

                    if (int.TryParse(ownerAndAnswer[0], out int id))
                    {
                        AnswerOptionButton aob = (ClientPlayer.GetClientByCurrentOwnerID(id) as VRtistryClientPlayer).playerAnswer;
                        aob.canvasGroup.alpha = 0;
                        aob.ResetPlayerIcons();

                        aob.SetText(ownerAndAnswer[1]);
                        aob.playerID = ownerAndAnswer[0];
                        if (int.TryParse(ownerAndAnswer[0], out int i))
                        {
                            //aob.SetBorderColor((i == VRtistrySyncer.instance.ChosenAnswerOwner) ? Color.green : Color.black);
                            aob.SetBorderColor(ClientPlayer.GetClientByCurrentOwnerID(id).syncer.Color);
                            aob.correctAnswerBanner.SetActive(i == VRtistrySyncer.instance.ChosenAnswerOwner);
                            (aob.correctAnswerBanner.transform as RectTransform).localScale = Vector3.zero;
                        }
                        answerResults.Add(aob);
                    }
                }

                paintBrush.AnimatePaintingReveal();
                break;
            case "vr guessing":
                string[] answersSeparated2 = VRtistrySyncer.instance.Answers.Split('\n');

                //If local player is the one who wrote the picked answer, show them someone else's answer
                if (VRtistrySyncer.instance.ChosenAnswerOwner == RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf)
                {
                    foreach (string a in answersSeparated2)
                    {
                        string[] ownerAndAnswer = a.Split(':');

                        if (int.TryParse(ownerAndAnswer[0], out int ownerID) && ownerID != RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf)
                        {
                            guessingHeaderText.text = "Who do you think wrote " + ownerAndAnswer[1] + "?";
                            answerOwnerIDPlayerIsGuessing = ownerID;
                            break;
                        }
                    }
                }
                //Else, show them the owner of the picked answer
                else
                {
                    foreach (string a in answersSeparated2)
                    {
                        string[] ownerAndAnswer = a.Split(':');

                        if (int.TryParse(ownerAndAnswer[0], out int ownerID) && ownerID == VRtistrySyncer.instance.ChosenAnswerOwner)
                        {
                            guessingHeaderText.text = "Who do you think wrote " + ownerAndAnswer[1] + "?";
                            answerOwnerIDPlayerIsGuessing = ownerID;
                            break;
                        }
                    }
                }

                //Player Answer buttons, show everyone but self
                foreach (string a in answersSeparated2)
                {
                    string[] ownerAndAnswer = a.Split(':');

                    if (int.TryParse(ownerAndAnswer[0], out int ownerID) && ownerID  != RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf)
                    {
                        GameObject ab = Instantiate(answerButtonPrefab, answerButtonParent.transform);
                        ab.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(SubmitPlayerGuess(RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf, ownerID)); });
                        AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                        aob.SetText(ClientPlayer.GetClientByCurrentOwnerID(ownerID).syncer.Name);
                        aob.playerID = ownerAndAnswer[0];
                        answerButtons.Add(aob);
                    }
                }

                blurHeaderText.text = "VR player is guessing who wrote the selected answer";

                leanTouch.gameObject.SetActive(false);
                tapAndHoldRotateTutorial.SetActive(false);

                //All players have guessed, so add guesses to results
                string[] guessesSeparated = VRtistrySyncer.instance.ArtGuesses.Split('\n');
                foreach (string g in guessesSeparated)
                {
                    string[] ownerAndGuess = g.Split(':');

                    if (int.TryParse(ownerAndGuess[0], out int i))
                    {
                        if (ownerAndGuess[1] != "decoy")
                        {
                            AddPlayerToResults(i, ownerAndGuess[1]);
                        }
                    }
                }
                break;
            case "results":
                guessingCanvas.enabled = false;

                drawingPhaseCamera.gameObject.SetActive(true);
                guessingPhaseCamera.gameObject.SetActive(false);

                blurHeaderText.text = "";
                guessingHeaderText.text = "";

                //Create duplicate list of answers, sorted by amount of players chose that answer
                List<AnswerOptionButton> answerResultsSorted = answerResults.OrderBy(o => o.GetNumberOfPlayers()).ToList();

                //Answers that no players chose, save these to show briefly at the end
                List<AnswerOptionButton> answersWithNoGuesses = new List<AnswerOptionButton>();

                //Animate players that chose each answer, ignoring answers that no players chose
                AnswerOptionButton correctAnswer = null;
                int k = 1;
                for (int i = 0; i < answerResultsSorted.Count; i++)
                {
                    //Save correct answer for last
                    if (!answerResultsSorted[i].correctAnswerBanner.activeSelf)
                    {
                        if (answerResultsSorted[i].GetNumberOfPlayers() > 0)
                        {
                            answerResultsSorted[i].AnimateAnswers(k * ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME);
                            k++;
                        }
                        else
                        {
                            answersWithNoGuesses.Add(answerResultsSorted[i]);
                        }
                    }
                    else
                    {
                        correctAnswer = answerResultsSorted[i];
                    }
                }

                int correctAnswerDelay = k * ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME;
                correctAnswer.AnimateAnswers(correctAnswerDelay);

                //Display answers that got no guesses
                foreach (AnswerOptionButton aob in answersWithNoGuesses)
                {
                    aob.AnimateAnswers(correctAnswerDelay + (ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME * 2));
                }

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
            case "gallery":
                DrawingsSyncer.instance.SetDrawingGalleryPositions();

                StartCoroutine("DisplayLeaderboard");

                drawingPhaseCamera.gameObject.SetActive(false);
                guessingPhaseCamera.gameObject.SetActive(false);
                galleryCamera.gameObject.SetActive(true);

                break;
            case "game over":
                break;
            default:
                break;
        }
    }

    void OnRevealAnimationComplete()
    {
        leanTouch.gameObject.SetActive(true);
        tapAndHoldRotateTutorial.SetActive(!tapAndHoldRotateLearned);
    }

    void SetDecoyAnswers(string decoyAnswers)
    {
        if (decoyAnswers.Equals("")) //Decoy answers were reset
        {
            return;
        }
        else if (!decoyAnswers.Contains("AI request failed"))
        {
            foreach (string d in decoyAnswers.Split(','))
            {
                GameObject ab = Instantiate(answerButtonPrefab, answerButtonParent.transform);
                ab.transform.SetSiblingIndex(Random.Range(0, ab.transform.childCount)); //Randomize sibling index so decoy answers are not always the last ones
                ab.GetComponent<Button>().onClick.AddListener(delegate { SubmitArtGuess(RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf, "decoy"); });
                AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                aob.SetText(d);
                aob.playerID = "decoy";
                answerButtons.Add(aob);
            }
        }
        
        answerButtonParent.SetActive(true);
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
#if UNITY_WEBGL && !UNITY_EDITOR
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
#if UNITY_WEBGL && !UNITY_EDITOR
        SetPointerDownOnButton(false);
#endif
    }

    public void CheckAnswerInput(string input)
    {
        submitButton.interactable = (input.Length > 0);
    }

    TMP_InputField currentField;
    public void SetField(TMP_InputField f)
    {
        currentField = f;
#if UNITY_WEBGL && !UNITY_EDITOR
        UpdateInputFieldText(currentField.text);
#endif
    }

    public void UpdateDummyInputText()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        UpdateInputFieldText(currentField.text);
#endif
    }

    public void UpdateDummyInputTextWithCharacterLimit(int limit)
    {
        if (currentField.text.Length > limit)
        {
            currentField.text = currentField.text.Substring(0, limit);
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        UpdateInputFieldText(currentField.text);
#endif
    }

    public void SubmitAnswer()
    {
        if (VRtistrySyncer.instance.Answers.Equals(""))
        {
            VRtistrySyncer.instance.Answers = (RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf + ":" + answerInputField.text);
        }
        else
        {
            VRtistrySyncer.instance.Answers += ("\n" + RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf + ":" + answerInputField.text);
        }

        typingAnswer = false;
        (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer).TogglePhone();
#if UNITY_WEBGL && !UNITY_EDITOR
        CloseInputKeyboard();
#endif
        answerInputField.DeactivateInputField();

        inputCanvas.enabled = false;
        playerInputParent.SetActive(false);
    }

    void SubmitArtGuess(int clientID, string clientGuessID)
    {
        if(VRtistrySyncer.instance.ArtGuesses.Equals(""))
        {
            VRtistrySyncer.instance.ArtGuesses = clientID + ":" + clientGuessID;
        }
        else
        {
            VRtistrySyncer.instance.ArtGuesses += "\n" + clientID + ":" + clientGuessID;
        }

        if (int.TryParse(clientGuessID, out int j) && j == VRtistrySyncer.instance.ChosenAnswerOwner)
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.Score += ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS;
        }

        guessingHeaderText.text = "Waiting for other players to answer";

        ClearPlayerAnswers();
    }

    IEnumerator SubmitPlayerGuess(int clientID, int clientGuessID)
    {
        //Tell user if they were right/wrong
        if (clientGuessID == answerOwnerIDPlayerIsGuessing)
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.Score += ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_PLAYER;
            guessingHeaderText.text = "Correct!";
        }
        else
        {
            guessingHeaderText.text = "Wrong! " + ClientPlayer.GetClientByCurrentOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner).syncer.Name + " wrote \n" + GetAnswerByOwnerID(answerOwnerIDPlayerIsGuessing);
        }

        ClearPlayerAnswers();

        yield return new WaitForSeconds(3);

        guessingHeaderText.text = "Waiting for other players to answer";

        //Sync their answer once they have had 3 seconds to see if they were right
        if (VRtistrySyncer.instance.PlayerGuesses.Equals(""))
        {
            VRtistrySyncer.instance.PlayerGuesses = clientID + ":" + clientGuessID;
        }
        else
        {
            VRtistrySyncer.instance.PlayerGuesses += "\n" + clientID + ":" + clientGuessID;
        }

        (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer).TogglePhone();
    }

    void AddPlayerToResults(int playerID, string answerPlayerID)
    {
        //Check each answer
        foreach (AnswerOptionButton aob in answerResults)
        {
            //Find the answer that the player chose
            if (aob.playerID.Equals(answerPlayerID))
            {
                aob.AddPlayerIcon(playerID);
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
    }

    void ResetAnswerResultsBubbles()
    {
        foreach (AnswerOptionButton aob in answerResults)
        {
            aob.canvasGroup.alpha = 0;
        }
        answerResults = new List<AnswerOptionButton>();
    }

    string GetAnswerByOwnerID(int ID)
    {
        string[] answersSeparated = VRtistrySyncer.instance.Answers.Split('\n');

        //Answer buttons
        foreach (string a in answersSeparated)
        {
            string[] ownerAndAnswer = a.Split(':');

            if(int.TryParse(ownerAndAnswer[0], out int i) &&  ID == i)
            {
                return ownerAndAnswer[1];
            }
        }

        return "";
    }

    IEnumerator DisplayLeaderboard()
    {
        //Clear previous leaderboard
        foreach (GameObject g in currentLeaderboardCards)
        {
            Destroy(g);
        }
        currentLeaderboardCards = new List<GameObject>();

        //Need to wait a frame to allow card objects to be destroyed so sibling index parameter works
        yield return new WaitForEndOfFrame();

        leaderboardCanvas.enabled = VRtistrySyncer.instance.State.Equals("leaderboard");

        //Sort player points
        Dictionary<int, int> unsortedDict = new Dictionary<int, int>();
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            unsortedDict.Add(cp.realtimeView.ownerIDSelf, cp.syncer.Score);
        }

        IOrderedEnumerable<KeyValuePair<int, int>> sortedDict = from entry in unsortedDict orderby entry.Value descending select entry;

        int vrPlayerPos = 0;
        //Add player cards
        foreach (KeyValuePair<int, int> entry in sortedDict)
        {
            GameObject newCard = Instantiate(leaderboardPlayerCardPrefab, (VRtistrySyncer.instance.State.Equals("gallery")) ? galleryLeaderboardParent.transform : leaderboardParent.transform);
            newCard.GetComponent<Image>().color = ClientPlayer.GetClientByCurrentOwnerID(entry.Key).syncer.Color;
            newCard.GetComponentsInChildren<TMP_Text>()[0].text = ClientPlayer.GetClientByCurrentOwnerID(entry.Key).syncer.Name;
            //newCard.GetComponentsInChildren<TMP_Text>()[1].text = GetAnswerByOwnerID(entry.Key);
            newCard.GetComponentsInChildren<TMP_Text>()[1].text = "" + entry.Value;

            currentLeaderboardCards.Add(newCard);

            if (VRtistrySyncer.instance.VRPlayerPoints < entry.Value)
            {
                vrPlayerPos++;
                newCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + (newCard.transform.GetSiblingIndex() + 1);
            }
            else
            {
                newCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + (newCard.transform.GetSiblingIndex() + 2);
            }
        }

        //Add VR Card
        GameObject vrCard = Instantiate(leaderboardPlayerCardPrefab, (VRtistrySyncer.instance.State.Equals("gallery")) ? galleryLeaderboardParent.transform : leaderboardParent.transform);
        vrCard.GetComponentsInChildren<TMP_Text>()[0].text = "VR Player";
        //vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "";
        vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "" + VRtistrySyncer.instance.VRPlayerPoints;
        vrCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + (vrPlayerPos + 1);
        vrCard.transform.SetSiblingIndex(vrPlayerPos);

        currentLeaderboardCards.Add(vrCard);

        yield return new WaitForSeconds(ThreeDPaintGlobalVariables.LEADERBOARD_DISPLAY_TIME);

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