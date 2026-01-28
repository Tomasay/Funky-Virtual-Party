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
using NaughtyAttributes;
using DG.Tweening;

public class VRtistryGameManagerWeb : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CloseInputKeyboard();

    [DllImport("__Internal")]
    private static extern void OnKeyboardInput();

    [DllImport("__Internal")]
    private static extern void SetPointerDownOnButton(bool isDown);

    [DllImport("__Internal")]
    private static extern void TriggerHaptic(int hapticTime);

    [DllImport("__Internal")]
    private static extern void ManuallyOpenKeyboard();
#endif

    [SerializeField]
    KeyboardController keyboardController;

    [SerializeField]
    VRtistryMainMenuManagerWeb mainMenuManager;

    [SerializeField]
    TMP_Text inputHeaderText, blurHeaderText, guessingHeaderText, thisIsYourPromptText;

    [SerializeField]
    TMP_Text blurTimerText;

    [SerializeField]
    RectTransform blurWipUI;

    [SerializeField]
    P3dPaintableTexture paintTexture;

    [SerializeField]
    Canvas joinedAndWaitingCanvas, inputCanvas, guessingCanvas, leaderboardCanvas;

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
    GameObject answerButtonPrefab, answerButtonParent, answerButtonBG, answerResultsParent;

    [SerializeField]
    GameObject drawingModel, linesParent;

    Quaternion drawingModelStartingRot;

    [SerializeField]
    Camera mainMenuCamera, drawingPhaseCamera, guessingPhaseCamera, galleryCamera;

    [SerializeField]
    MannequinSolverClient mannequinSolver;

    [SerializeField]
    GameObject leaderboardPlayerCardPrefab, leaderboardParent, galleryLeaderboardParent;
    List<GameObject> currentLeaderboardCards;

    [SerializeField]
    LeanTouch leanTouch;

    [SerializeField]
    LeanDragRotate leanDrag;

    [SerializeField]
    GameObject tapAndHoldRotateTutorial;
    bool tapAndHoldRotateLearned;

    [SerializeField]
    GameObject sculptStand;

    [SerializeField]
    GameObject initialEaselCanvas;

    [SerializeField]
    Slider leaderboardTimerSlider;

    [SerializeField]
    GameObject lastPlayerAnsweringWarning;

    [SerializeField]
    Animator correctPlayerGuessAnimation;

    bool typingAnswer = false; //Is player typing their answer?
    bool playersAnswering = false; //Are we still waiting for any player to submit their answer?

    List<AnswerOptionButton> answerButtons, answerResults;

    int answerOwnerIDPlayerIsGuessing;

    PaintBrush paintBrush;

    List<float> leaderboardHeightsPrevRound = null;

    private void Awake()
    {
        currentLeaderboardCards = new List<GameObject>();
        answerButtons = new List<AnswerOptionButton>();
        answerResults = new List<AnswerOptionButton>();

        leaderboardHeightsPrevRound = new List<float>();

        drawingModelStartingRot = drawingModel.transform.rotation;

        answerInputButton.onPointerDown.AddListener(ButtonPointerDown);
        answerInputButton.onPointerUp.AddListener(ButtonPointerUp);

        guessingCanvas.enabled = false;

        LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;

        Draw.Position = linesParent.transform.position;
    }

    private void Start()
    {
        VRtistrySyncer.instance.OnStateChangeEvent.AddListener(OnStateChange);
        VRtistrySyncer.instance.OnPromptChangedEvent.AddListener(SetNewPrompt);
        VRtistrySyncer.instance.OnPlayerAnswered.AddListener(PlayerSubmittedAnswer);
        VRtistrySyncer.instance.OnDecoyAnswersChanged.AddListener(SetDecoyAnswers);
        VRtistrySyncer.instance.OnPlayerGuessedPlayer.AddListener(PlayerGuessedPlayer);
    }

    private void LeanTouch_OnFingerDown(LeanFinger obj)
    {
        if (paintBrush.RevealAnimationComplete)
        {
            tapAndHoldRotateLearned = true;
            tapAndHoldRotateTutorial.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChange);
        VRtistrySyncer.instance.OnPromptChangedEvent.RemoveListener(SetNewPrompt);
        VRtistrySyncer.instance.OnPlayerAnswered.RemoveListener(PlayerSubmittedAnswer);
        VRtistrySyncer.instance.OnPlayerGuessedPlayer.RemoveListener(PlayerGuessedPlayer);

        answerInputButton.onPointerDown.RemoveListener(ButtonPointerDown);
        answerInputButton.onPointerUp.RemoveListener(ButtonPointerUp);

        LeanTouch.OnFingerDown -= LeanTouch_OnFingerDown;
    }

    void Update()
    {
        if (typingAnswer)
        {
            answerInputField.ActivateInputField();
            answerInputField.caretPosition = answerInputField.text.Length;
        }

        if ((VRtistrySyncer.instance.State.Equals("clients guessing") || VRtistrySyncer.instance.State.Equals("vr guessing")) && paintBrush.RevealAnimationComplete)
        {
            if (tapAndHoldRotateLearned)
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

        if (VRtistrySyncer.instance.State.Equals("clients answering") && VRtistrySyncer.instance.ClientAnswerTimer <= 0 && typingAnswer && !lastPlayerAnsweringWarning.activeSelf)
        {
            string[] answersSeparated = VRtistrySyncer.instance.Answers.Split('\n');
            if (!VRtistrySyncer.instance.Answers.Equals("") && answersSeparated.Length == ClientPlayer.clients.Count - 1)
            {
                lastPlayerAnsweringWarning.SetActive(true);
            }
        }
    }

    void PlayerSubmittedAnswer(string answers)
    {
        //Check to see if all players have answered and if we're waiting on vr player
        string[] answersSeparated = answers.Split('\n');

        if (answersSeparated.Length == ClientPlayer.clients.Count && VRtistrySyncer.instance.VRCompletedTutorial == false)
        {
            blurHeaderText.text = "Waiting for VR player to complete tutorial...";
        }
        else if (answersSeparated.Length != ClientPlayer.clients.Count)
        {
            blurHeaderText.text = "Waiting for all players to submit their answer...";
        }
    }

    void SetNewPrompt(string p)
    {
        if (p.Equals(""))
        {
            return;
        }

        inputCanvas.enabled = true;
        answerInputField.text = "";
        blurHeaderText.text = "";
        inputHeaderText.text = p; //Set prompt text
        playerInputParent.SetActive(true); //Enable input
        playersAnswering = true;

        drawingModel.transform.rotation = drawingModelStartingRot;
        Draw.Rotation = Quaternion.identity;
        if (paintBrush) paintBrush.ResetRevealAnimation();

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
            case "main menu":
                RealtimeSingletonWeb.instance.LocalPlayer.syncer.Score = 0;

                //Reset any painting from previous round
                paintTexture.Clear();
                drawingModel.transform.rotation = drawingModelStartingRot;
                DrawingsSyncer.instance.paintSyncer.ResetStoredHitLines();

                ResetAnswerResultsBubbles();

                joinedAndWaitingCanvas.enabled = true;

                mainMenuCamera.gameObject.SetActive(true);
                galleryCamera.gameObject.SetActive(false);
                break;
            case "clients answering":
                joinedAndWaitingCanvas.enabled = false;
                initialEaselCanvas.SetActive(false);
                ToggleSculptStand(true);
                mainMenuManager.HideMainMenuUI();
                ResetAnswerResultsBubbles();

                //Enable phone anim for local player, which will then be synced for everyone else
                VRtistryClientPlayer vcp = (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer);
                if (vcp && vcp.usingPhone == 0)
                {
                    vcp.TogglePhone();
                }

                answerInputButton.onClick.Invoke();
#if UNITY_WEBGL && !UNITY_EDITOR
                ManuallyOpenKeyboard();
                TriggerHaptic(200);
#endif
                break;

            case "vr picking prompt":
                lastPlayerAnsweringWarning.SetActive(false);
                blurTimerText.enabled = false;
                //Show blurred view
                inputCanvas.enabled = false;
                
                blurHeaderText.text = "Waiting for VR player to pick a prompt...";

                //Reset any painting from practicing
                paintTexture.Clear();
                DrawingsSyncer.instance.paintSyncer.ResetStoredHitLines();

                break;
            case "vr posing":
                if (!paintBrush)
                {
                    paintBrush = FindAnyObjectByType<PaintBrush>();
                    paintBrush.LinesParent = linesParent.transform;
                    paintBrush.paintTexture = paintTexture;
                    paintBrush.OnRevealAnimationComplete.AddListener(OnRevealAnimationComplete);
                }

                blurHeaderText.text = "Waiting for VR player to set a pose...";
                break;
            case "vr painting":
                blurWipUI.DOScale(1, 0.5f);

                //Bake mannequin IK
                mannequinSolver.SetPoseColliders();

                blurTimerText.enabled = true;
                blurTimerText.text = FormatTime(VRtistrySyncer.instance.DrawingTimer);
                blurHeaderText.text = "VR Player is creating a masterpiece...";

                mainMenuCamera.gameObject.SetActive(false);
                drawingPhaseCamera.gameObject.SetActive(true);
                guessingPhaseCamera.gameObject.SetActive(false);
                break;
            case "clients guessing":
                tapAndHoldRotateLearned = false;
                bool isLocalClientsPrompt = VRtistrySyncer.instance.ChosenAnswerOwner == RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf;

                thisIsYourPromptText.enabled = isLocalClientsPrompt;

                guessingHeaderText.text = isLocalClientsPrompt ? "Waiting for other players to answer" : "What is it?";

                (RealtimeSingletonWeb.instance.LocalPlayer as VRtistryClientPlayer).TogglePhone();

                blurTimerText.enabled = false;

                //Make sure there's no lingering drawings
                paintBrush.CanPaintAir = false;

                playersAnswering = false;

                guessingCanvas.enabled = true;

                blurWipUI.DOScale(0, 0.5f);

                drawingPhaseCamera.gameObject.SetActive(false);
                guessingPhaseCamera.gameObject.SetActive(true);

                string[] answersSeparated = VRtistrySyncer.instance.Answers.Split('\n');

                
                if (!isLocalClientsPrompt)
                {
                    //Answer buttons
                    answerButtonParent.SetActive(false);
                    answerButtonBG.SetActive(false);
                    foreach (string a in answersSeparated)
                    {
                        string[] ownerAndAnswer = a.Split(':');

                        GameObject ab = Instantiate(answerButtonPrefab, answerButtonParent.transform);
                        ab.transform.localScale = Vector3.zero;
                        string guessGuessOwner = (RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf + ":" + ownerAndAnswer[0]);
                        ab.GetComponent<Button>().onClick.AddListener(delegate { SubmitArtGuess(RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf, ownerAndAnswer[0]); });
                        AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                        aob.SetText(ownerAndAnswer[1]);
                        aob.playerID = ownerAndAnswer[0];
                        answerButtons.Add(aob);
                    }
                }

                //Answer results
                foreach (string a in answersSeparated)
                {
                    string[] ownerAndAnswer = a.Split(':');

                    if (int.TryParse(ownerAndAnswer[0], out int id))
                    {
                        ClientPlayer cp = ClientPlayer.GetClientByCurrentOwnerID(id);
                        AnswerOptionButton aob = (cp as VRtistryClientPlayer).playerAnswer;
                        aob.canvasGroup.alpha = 0;
                        aob.ResetPlayerIcons();

                        aob.SetTextWithPlayerName(ownerAndAnswer[1], ClientPlayer.GetClientByCurrentOwnerID(id));
                        aob.playerID = ownerAndAnswer[0];

                        //aob.SetBorderColor((i == VRtistrySyncer.instance.ChosenAnswerOwner) ? Color.green : Color.black);
                        aob.SetBorderColor(cp.syncer.Color);
                        aob.correctAnswerBanner.SetActive(id == VRtistrySyncer.instance.ChosenAnswerOwner);
                        (aob.correctAnswerBanner.transform as RectTransform).localScale = Vector3.zero;

                        answerResults.Add(aob);
                    }
                }

                paintBrush.AnimatePaintingReveal();
                break;
            case "vr guessing":
                Invoke("CreatePlayerGuessButtons", 0.5f);

                blurHeaderText.text = "VR player is guessing who wrote the selected answer";

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
                SetPlayerNamesVisibility(false);

                leanDrag.enabled = false;
                leanTouch.gameObject.SetActive(false);
                tapAndHoldRotateTutorial.SetActive(false);

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
                SetPlayerNamesVisibility(true);

                DrawingsSyncer.instance.SetDrawingGalleryPositions();
                Draw.Rotation = Quaternion.identity;

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

    void SetPlayerNamesVisibility(bool visible)
    {
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            cp.playerNameText.gameObject.SetActive(visible);
        }
    }

    void CreatePlayerGuessButtons()
    {
        thisIsYourPromptText.enabled = false;

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

        //Player Answer buttons, show 3 options, excluding self
        List<int> clientGuessOptions = ClientPlayer.GetRandomClientIdList(3, VRtistrySyncer.instance.ChosenAnswerOwner, RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf);
        foreach (int g in clientGuessOptions)
        {
            VRtistryClientPlayer vcp = (ClientPlayer.GetClientByCurrentOwnerID(g) as VRtistryClientPlayer);

            GameObject ab = Instantiate(answerButtonPrefab, answerButtonParent.transform);
            ab.transform.localScale = Vector3.zero;
            ab.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(SubmitPlayerGuess(RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf, g)); });
            AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
            aob.SetText(ClientPlayer.GetClientByCurrentOwnerID(g).syncer.Name);
            aob.playerID = "" + g;
            answerButtons.Add(aob);
        }

        StartCoroutine("AnimateAnswerButtonsIn");
    }

    void OnRevealAnimationComplete()
    {
        leanDrag.enabled = true;
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
                ab.transform.localScale = Vector3.zero;
                ab.transform.SetSiblingIndex(Random.Range(0, ab.transform.childCount)); //Randomize sibling index so decoy answers are not always the last ones
                ab.GetComponent<Button>().onClick.AddListener(delegate { SubmitArtGuess(RealtimeSingletonWeb.instance.LocalPlayer.realtimeView.ownerIDSelf, "decoy"); });
                AnswerOptionButton aob = ab.GetComponent<AnswerOptionButton>();
                aob.SetText(d);
                aob.playerID = "decoy";
                answerButtons.Add(aob);
            }
        }

        answerButtonParent.SetActive(true);
        answerButtonParent.transform.localScale = Vector3.one;
        StartCoroutine("AnimateAnswerButtonsIn");
    }

    IEnumerator AnimateAnswerButtonsIn()
    {
        //Animate buttons in
        answerButtonParent.transform.localScale = Vector3.one;
        answerButtonBG.SetActive(true);
        answerButtonBG.transform.localScale = Vector3.zero;
        (answerButtonBG.transform as RectTransform).DOScale(1, 0.25f);
        yield return new WaitForSeconds(0.25f);
        foreach (AnswerOptionButton aob in answerButtonParent.GetComponentsInChildren<AnswerOptionButton>())
        {
            (aob.transform as RectTransform).DOScale(answerButtonPrefab.transform.localScale, 0.25f);
            yield return new WaitForSeconds(0.125f);
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
        keyboardController.SetField(f);
#endif
    }

    public void RestrictCharacterLimit(int limit)
    {
        if (currentField.text.Length > limit)
        {
            currentField.text = currentField.text.Substring(0, limit);
        }
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
        bool firstGuess = VRtistrySyncer.instance.ArtGuesses.Equals("");

        if (firstGuess)
        {
            VRtistrySyncer.instance.ArtGuesses = clientID + ":" + clientGuessID;
        }
        else
        {
            VRtistrySyncer.instance.ArtGuesses += "\n" + clientID + ":" + clientGuessID;
        }

        if (int.TryParse(clientGuessID, out int j) && j == VRtistrySyncer.instance.ChosenAnswerOwner)
        {
            //Award points if player is correct, also add first guess bonus if applicable
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.Score += (firstGuess ? 
            ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS + ThreeDPaintGlobalVariables.POINTS_CLIENT_FIRST_CORRECT_GUESS :
            ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS);
        }

        guessingHeaderText.text = "Waiting for other players to answer";

        (answerButtonParent.transform as RectTransform).DOScale(0, 0.25f);
        ClearPlayerAnswers();
    }

    void PlayerGuessedPlayer(string guesses)
    {
        //Check to see if VR is only player left to guess
        if (VRtistrySyncer.instance.VRPlayerGuess == -1 && guesses.Split('\n').Length >= ClientPlayer.clients.Count && VRtistrySyncer.instance.State.Equals("vr guessing"))
        {
            guessingHeaderText.text = "Waiting for VR player to answer";
        }
    }

    IEnumerator SubmitPlayerGuess(int clientID, int clientGuessID)
    {
        //Tell user if they were right/wrong
        if (clientGuessID == answerOwnerIDPlayerIsGuessing)
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.Score += ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_PLAYER;

            correctPlayerGuessAnimation.gameObject.SetActive(true);
            correctPlayerGuessAnimation.SetTrigger("play");
            guessingHeaderText.text = "";
        }
        else
        {
            guessingHeaderText.text = "Wrong! " + ClientPlayer.GetClientByCurrentOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner).syncer.Name + " wrote \n" + GetAnswerByOwnerID(answerOwnerIDPlayerIsGuessing);
        }

        (answerButtonParent.transform as RectTransform).DOScale(0, 0.25f);
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
        StartCoroutine("ClearPlayerAnswersAnimation");
    }

    IEnumerator ClearPlayerAnswersAnimation()
    {
        (answerButtonBG.transform as RectTransform).DOScale(0, 0.25f);

        yield return new WaitForSeconds(0.25f);

        foreach (AnswerOptionButton aob in answerButtons)
        {
            Destroy(aob.gameObject);
        }
        answerButtons = new List<AnswerOptionButton>();
        answerButtonBG.SetActive(false);
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

            if (int.TryParse(ownerAndAnswer[0], out int i) && ID == i)
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
        unsortedDict.Add(-1, VRtistrySyncer.instance.VRPlayerPoints);

        IOrderedEnumerable<KeyValuePair<int, int>> sortedDict = from entry in unsortedDict orderby entry.Value descending select entry;

        int vrPlayerPos = 1;
        int index = 1;
        //Add player cards
        foreach (KeyValuePair<int, int> entry in sortedDict)
        {
            if (entry.Key != -1)
            {
                GameObject newCard = Instantiate(leaderboardPlayerCardPrefab, (VRtistrySyncer.instance.State.Equals("gallery")) ? galleryLeaderboardParent.transform : leaderboardParent.transform);
                (newCard.transform as RectTransform).localScale = new Vector3(0, 1, 1);
                newCard.GetComponent<Image>().color = ClientPlayer.GetClientByCurrentOwnerID(entry.Key).syncer.Color;
                newCard.GetComponentsInChildren<TMP_Text>()[0].text = ClientPlayer.GetClientByCurrentOwnerID(entry.Key).syncer.Name;
                //newCard.GetComponentsInChildren<TMP_Text>()[1].text = GetAnswerByOwnerID(entry.Key);
                newCard.GetComponentsInChildren<TMP_Text>()[1].text = "" + entry.Value;

                currentLeaderboardCards.Add(newCard);

                newCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + (index);
            }
            else
            {
                vrPlayerPos = index;
            }

            index++;
        }

        //Add VR Card
        GameObject vrCard = Instantiate(leaderboardPlayerCardPrefab, (VRtistrySyncer.instance.State.Equals("gallery")) ? galleryLeaderboardParent.transform : leaderboardParent.transform);
        (vrCard.transform as RectTransform).localScale = new Vector3(0, 1, 1);
        vrCard.GetComponentsInChildren<TMP_Text>()[0].text = "VR Player";
        //vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "";
        vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "" + VRtistrySyncer.instance.VRPlayerPoints;
        vrCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + (vrPlayerPos);
        vrCard.transform.SetSiblingIndex(vrPlayerPos);

        currentLeaderboardCards.Add(vrCard);

        //Animate cards in
        RectTransform[] rts = (VRtistrySyncer.instance.State.Equals("gallery")) ? galleryLeaderboardParent.GetComponentsInChildren<RectTransform>() : leaderboardParent.GetComponentsInChildren<RectTransform>();
        foreach (RectTransform rt in rts)
        {
            if (rt.gameObject.name.Contains("LeaderboardPlayerCardClient"))
            {
                rt.DOScaleX(1, 0.25f);
                yield return new WaitForSeconds(0.125f);
            }
        }

        /*TODO:Finish leaderboard lead change animations
        //Animate lead changes from previous round
        if (leaderboardHeightsPrevRound.Count > 0)
        {
            for (int i = 0; i < currentLeaderboardCards.Count(); i++)
            {
                float currentHeight = currentLeaderboardCards[i].transform.localPosition.y;

                LayoutElement le = currentLeaderboardCards[i].AddComponent<LayoutElement>();
                le.ignoreLayout = true;

                Vector3 pos = currentLeaderboardCards[i].transform.localPosition;
                pos.y = leaderboardHeightsPrevRound[i];
                currentLeaderboardCards[i].transform.localPosition = pos;

                currentLeaderboardCards[i].transform.DOLocalMoveY(currentHeight, 1);
            }
            yield return new WaitForSeconds(1);
        }

        //Store prev heights for next round
        leaderboardHeightsPrevRound = new List<float>();
        foreach (GameObject c in currentLeaderboardCards)
        {
            leaderboardHeightsPrevRound.Add(c.transform.localPosition.y);
        }
        */

        leaderboardTimerSlider.DOValue(1, ThreeDPaintGlobalVariables.LEADERBOARD_DISPLAY_TIME);

        yield return new WaitForSeconds(ThreeDPaintGlobalVariables.LEADERBOARD_DISPLAY_TIME);

        //Disable leaderboard
        leaderboardCanvas.enabled = false;
        leaderboardTimerSlider.value = 0;
    }

    [Button]
    public void HideSculptStand()
    {
        ToggleSculptStand(false);
    }

    [Button]
    public void ShowSculptStand()
    {
        ToggleSculptStand(true);
    }

    void ToggleSculptStand(bool visible)
    {
        foreach (MeshRenderer mr in sculptStand.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = visible;
        }
        foreach (SkinnedMeshRenderer smr in sculptStand.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            smr.enabled = visible;
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}