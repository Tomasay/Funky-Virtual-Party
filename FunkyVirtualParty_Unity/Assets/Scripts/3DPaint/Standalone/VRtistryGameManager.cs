using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaintIn3D;
using Autohand;
using Autohand.Demo;
using System.Linq;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using DG.Tweening;
#if !UNITY_WEBGL
using FMOD.Studio;
using FMODUnity;
#endif
using Normal.Realtime;
using NaughtyAttributes;

public class VRtistryGameManager : MonoBehaviour
{
    [SerializeField]
    VRtistryMainMenuManager mainMenuManager;

    [SerializeField]
    TextAsset promptList, clientThemedPromptList;

    [SerializeField]
    Canvas uiCanvas;

    [SerializeField]
    TMP_Text headerText, playerResultsHeaderText, timerText;

    [SerializeField]
    Slider countdownSlider;

    [SerializeField]
    P3dPaintableTexture paintTexture, gallerySecondMannequinPaintTexture, mannequinFaceTexture, gallerySecondMannequinFaceTexture;

    [SerializeField]
    MeshRenderer mannequinFaceMesh, gallerySecondMannequinFaceMesh;

    [SerializeField]
    GameObject leaderboardParent, leaderboardPlayerCardPrefab;
    List<GameObject> currentLeaderboardCards;

    [SerializeField]
    GameObject playerNameIconPrefab, playerNamesIconParent;
    List<GameObject> playerNameIcons;

    [SerializeField]
    Button finishedPaintingEarlyButton;

    [SerializeField]
    MannequinSolver solver;

    [SerializeField]
    P3dPaintableTexture mannequinUVs;

    [SerializeField]
    VRTutorial tutorial;

    [SerializeField]
    GameObject armature;

    [SerializeField]
    Material clientMat;

    [SerializeField]
    //GeminiFakeAnswersGenerator decoyAnswersGenerator;

    List<AnswerOptionButton> answerResults;

    [SerializeField]
    RectTransform canvasGalleryTransform;
    Vector3 canvasDefaultPos;
    Quaternion canvasDefaultRot;

    [SerializeField]
    Transform vrPlayerGallerySpawnPos;

    [SerializeField]
    MannequinHeightSlider mannequinHeightSlider;

    [SerializeField]
    Transform linesParent;

    [SerializeField]
    string musicEventPath;

    [SerializeField]
    GameObject promptOptionButtonsPrefab, promptOptionButtonsParent;

    List<GameObject> promptOptionButtons;

#if !UNITY_WEBGL
    EventInstance fmodInstance;
#endif

    [SerializeField]
    CanvasGroup outOfBoundsCanvasGroup;

    [SerializeField]
    BoxCollider paintingOutOfBounds;

    PaintBrush paintBrush;

    PaintPalette paintPalette;

    int currentRound = 1;

    const int ROAST_ROUND = 2;

    HandType toolHand = HandType.right;

    bool needToGrabPalette;

    private VRtistryVRPlayerController vrPlayer;

    private float pointerPreviewDrawDistance;

    const string DONT_SAY_WARNING = "<sprite=0> <size=0.1px><color=#F6AC70><u><b>DON'T SAY THIS OUTLOUD!</b></u></color></size>\n";

    RealtimeTransform[] armatureRTs;
    Vector3[] armaturePositions;
    Quaternion[] armatureRotations;

    RectTransform promptOptionRT;

    private void Awake()
    {
        answerResults = new List<AnswerOptionButton>();

        playerNameIcons = new List<GameObject>();
        currentLeaderboardCards = new List<GameObject>();

        promptOptionButtons = new List<GameObject>();

        armatureRTs = armature.transform.root.gameObject.GetComponentsInChildren<RealtimeTransform>();
        armaturePositions = new Vector3[armatureRTs.Length];
        armatureRotations = new Quaternion[armatureRTs.Length];
        for (int i = 0; i < armatureRTs.Length; i++)
        {
            armaturePositions[i] = armatureRTs[i].transform.position;
            armatureRotations[i] = armatureRTs[i].transform.rotation;
        }

        headerText.text = "";
        playerResultsHeaderText.text = "";
        timerText.text = "";
        headerText.enabled = false;

        finishedPaintingEarlyButton.gameObject.SetActive(false);

        canvasDefaultPos = (uiCanvas.transform as RectTransform).position;
        canvasDefaultRot = (uiCanvas.transform as RectTransform).rotation;

        promptOptionRT = (promptOptionButtonsParent.transform as RectTransform);

#if !UNITY_WEBGL
        fmodInstance = RuntimeManager.CreateInstance(musicEventPath);
        fmodInstance.start();
#endif
    }

    private void Start()
    {
        RealtimeSingleton.instance.Realtime.didConnectToRoom += Realtime_didConnectToRoom;

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarDestroyed += RealtimeAvatarManager_avatarDestroyed;
    }

    private void Realtime_didConnectToRoom(Realtime realtime)
    {
        //Spawn tools
        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = true;

        paintBrush = Realtime.Instantiate("PaintBrush", Vector3.zero, Quaternion.identity, options).GetComponent<PaintBrush>();
        paintBrush.LinesParent = linesParent;
        paintBrush.outOfBoundsCanvasGroup = outOfBoundsCanvasGroup;
        paintBrush.paintingOutOfBounds = paintingOutOfBounds;
#if !UNITY_WEBGL
        paintBrush.gm = this;
#endif

        paintPalette = Realtime.Instantiate("PaintPalette", Vector3.zero, Quaternion.identity, options).GetComponent<PaintPalette>();

        tutorial.SetTools(paintBrush, paintPalette);

#if !UNITY_WEBGL
        //Tool callbacks
        //Paint Brush
        Grabbable g = paintBrush.gameObject.GetComponent<Grabbable>();
        g.onGrab.AddListener(delegate {
            paintBrush.RealtimeTransform.RequestOwnership();
            paintBrush.Rb.isKinematic = false;
            paintBrush.IsInHand = true;
        });
        g.onRelease.AddListener(delegate {
            paintBrush.Rb.isKinematic = true;
            paintBrush.IsInHand = false;
        });
        g.onSqueeze.AddListener(paintBrush.OnTriggerPressed);
        g.onUnsqueeze.AddListener(paintBrush.OnTriggerReleased);

        //Paint palette
        g = paintPalette.gameObject.GetComponent<Grabbable>();
        g.onGrab.AddListener(delegate {
            paintPalette.RealtimeTransform.RequestOwnership();
            paintPalette.Rb.isKinematic = false;
        });
        g.onRelease.AddListener(delegate {
            paintPalette.Rb.isKinematic = true;
        });
#endif
    }

    public bool gameSetup = false;
    public void SetupGame()
    {
        VRtistrySyncer.instance.OnStateChangeEvent.AddListener(OnStateChanged);
        ClientSync.OnAnyVrtistryAnswerChanged.AddListener(PlayerAnswered);
        ClientSync.OnAnyVrtistryTypedGuessChanged.AddListener(PlayerTypedGuess);
        ClientSync.OnAnyVrtistryArtGuessChanged.AddListener(PlayerGuessedArt);
        ClientSync.OnAnyVrtistryPlayerGuessChanged.AddListener(PlayerGuessedPlayer);

        InvokeRepeating("Test", 1, 1);

        gameSetup = true;
    }

    //Temporary solution because ownership seems to randomly get taken by VR player, preventing clients from changing any values
    void Test()
    {
        VRtistrySyncer.instance.realtimeView.ClearOwnership();
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChanged);
        ClientSync.OnAnyVrtistryAnswerChanged.RemoveListener(PlayerAnswered);
        ClientSync.OnAnyVrtistryTypedGuessChanged.RemoveListener(PlayerTypedGuess);
        ClientSync.OnAnyVrtistryArtGuessChanged.RemoveListener(PlayerGuessedArt);
        ClientSync.OnAnyVrtistryPlayerGuessChanged.RemoveListener(PlayerGuessedPlayer);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarDestroyed -= RealtimeAvatarManager_avatarDestroyed;

        if (RealtimeSingleton.instance.RealtimeAvatarManager.localAvatar != null)
            RealtimeSingleton.instance.RealtimeAvatarManager.localAvatar.OnHandMeshVisibilityChanged.RemoveListener(OnHandVisibilityChanged);
    }

    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        avatar.OnHandMeshVisibilityChanged.AddListener(OnHandVisibilityChanged);

        vrPlayer = avatar.GetComponent<VRtistryVRPlayerController>();

        vrPlayer.leftHand.GetComponent<HandPublicEvents>().OnGrab.AddListener(OnGrabbed);
        vrPlayer.rightHand.GetComponent<HandPublicEvents>().OnGrab.AddListener(OnGrabbed);

        vrPlayer.leftHand.GetComponent<HandAdvancedOptions>().ignoreHandCollider.Add(mannequinUVs.GetComponent<MeshCollider>());
        vrPlayer.rightHand.GetComponent<HandAdvancedOptions>().ignoreHandCollider.Add(mannequinUVs.GetComponent<MeshCollider>());

        vrPlayer.SetCanvas(uiCanvas.gameObject);

        pointerPreviewDrawDistance = vrPlayer.leftUIPointerPreview.rayDrawDistance;
    }

    private void RealtimeAvatarManager_avatarDestroyed(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        //These have to go on the avatar's destriy event to ensure that the hands still exist
        vrPlayer.leftHand.GetComponent<HandPublicEvents>().OnGrab.RemoveListener(OnGrabbed);
        vrPlayer.rightHand.GetComponent<HandPublicEvents>().OnGrab.RemoveListener(OnGrabbed);
    }

    bool shouldToolsBeVisible = false; //Should tools be marked visible when hands reconnect?
    void OnHandVisibilityChanged(bool left, bool visible)
    {
        if (!shouldToolsBeVisible)
        {
            return;
        }

        if ((toolHand == HandType.left && left) || (toolHand == HandType.right && !left))
        {
            if (VRtistrySyncer.instance.IsBrushEnabled)
            {
                paintBrush.SetMeshVisibility(visible);
            }
        }
        else
        {
            paintPalette.gameObject.GetComponent<PaintPalette>().SetActive(visible);
        }
    }

    public void GrabToolsStart()
    {
        GrabTool();
        GrabPalette();
    }

    float poseHoldTimer;
    private void Update()
    {
        switch (VRtistrySyncer.instance.State)
        {
            case "clients answering":
                VRtistrySyncer.instance.ClientAnswerTimer -= Time.deltaTime;

                /*
                if (VRtistrySyncer.instance.ClientAnswerTimer <= 0 && VRtistrySyncer.instance.VRCompletedTutorial)
                {
                    VRtistrySyncer.instance.State = "vr picking prompt";
                }
                */

                break;
            case "vr posing":

                bool anyButtonHeld = OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Two) ||
                    OVRInput.Get(OVRInput.Button.Three) || OVRInput.Get(OVRInput.Button.Four) ||
                    OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || OVRInput.Get(OVRInput.RawButton.RIndexTrigger) ||
                    OVRInput.Get(OVRInput.RawButton.LHandTrigger) || OVRInput.Get(OVRInput.RawButton.RHandTrigger);

                if (anyButtonHeld)
                {
                    poseHoldTimer += Time.deltaTime;
                    if(countdownSlider.transform.parent.localScale == Vector3.zero) countdownSlider.transform.parent.DOScale(1, 0.25f);
                    countdownSlider.value = poseHoldTimer / 3.0f;
                    if (poseHoldTimer >= 3f)
                    {
                        SetPose();
                    }
                }
                else
                {
                    poseHoldTimer = 0f;
                    countdownSlider.value = poseHoldTimer;
                }

                break;
            case "vr painting":
                //Draw timer
                VRtistrySyncer.instance.DrawingTimer -= Time.deltaTime;
                timerText.text = FormatTime(VRtistrySyncer.instance.DrawingTimer);

#if !UNITY_WEBGL
                if (VRtistrySyncer.instance.DrawingTimer <= 15)
                {
                    fmodInstance.setParameterByName("VRtistryClock", 2);
                }
                else if (VRtistrySyncer.instance.DrawingTimer <= ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT / 2)
                {
                    fmodInstance.setParameterByName("VRtistryClock", 1);
                }
#endif

                if (VRtistrySyncer.instance.DrawingTimer <= 0)
                {
                    VRtistrySyncer.instance.State = "clients typing guess";
                }
                break;
            case "clients guessing":
                break;
            case "vr guessing":
                break;
            default:
                break;
        }

        if (!VRtistrySyncer.instance.State.Equals("vr posing"))
        {
            //Y button
            if (tutorial.yButtonEnabled && OVRInput.GetDown(OVRInput.Button.Four))
            {
                if (toolHand == HandType.right)
                {
                    ToggleToolHand();
                }
            }

            //B button
            if (tutorial.bButtonEnabled && OVRInput.GetDown(OVRInput.Button.Two))
            {
                if (toolHand == HandType.left)
                {
                    ToggleToolHand();
                }
            }
        }
    }

    void OnDecoyAnswersGenerated(string decoyAnswers)
    {
        VRtistrySyncer.instance.DecoyAnswers = decoyAnswers;
    }

    [Button]
    public void SimulateRound()
    {
        StartCoroutine("SimulateRoundCoroutine");
    }

    IEnumerator SimulateRoundCoroutine()
    {
        PressSkipButton();
        yield return new WaitForSeconds(0.5f);
        PickFirstPromptOption();
        yield return new WaitForSeconds(0.5f);
        SetPose();
        yield return new WaitForSeconds(3);
        FinishedPaintingEarly();
    }

    [Button]
    public void PressContinueButton()
    {
        GrabToolsStart();
        tutorial.ContinueButtonPressed();
    }

    [Button]
    public void PressSkipButton()
    {
        OnTutorialCompleted();
        tutorial.SkipButtonPressed();
    }

    [Button]
    public void PickFirstPromptOption()
    {
        if (promptOptionButtons.Count > 0)
            promptOptionButtons[0].GetComponent<Button>().onClick.Invoke();
    }

    public void OnTutorialCompleted()
    {
        vrPlayer.UIWarningArrow.SetActive(false);
        VRtistrySyncer.instance.VRCompletedTutorial = true;

        int answeredCount = ClientPlayer.clients.Count(cp => !string.IsNullOrEmpty(cp.syncer.VrtistryAnswer));
        int neededCount = (currentRound == ROAST_ROUND) ? ClientPlayer.clients.Count - 1 : ClientPlayer.clients.Count;
        if (answeredCount >= neededCount)
        {
            VRtistrySyncer.instance.State = "vr picking prompt";
        }

        if (VRtistrySyncer.instance.State == "" || VRtistrySyncer.instance.State == "clients answering")
        {
            //Enable VR tools
            paintBrush.CanPaintAir = true;

            GrabToolsStart();

            //Display text that players are answering
            headerText.text = "Players are typing their answers \nUse this time to practice painting!";

            paintBrush.CanPaintAir = true;

            vrPlayer.leftUIPointerPreview.rayDrawDistance = 0;
            vrPlayer.rightUIPointerPreview.rayDrawDistance = 0;
        }

        headerText.enabled = true;
    }

    [Button]
    public void FinishedPaintingEarly()
    {
        VRtistrySyncer.instance.State = "clients typing guess";
    }

    [Button]
    void SetPose()
    {
        solver.SetPose();

        GrabToolsStart();
        VRtistrySyncer.instance.State = "vr painting";

        countdownSlider.gameObject.SetActive(false);
        countdownSlider.transform.parent.localScale = Vector3.zero;
    }

    bool firstTimeClientsAnswering = true;
    int correctGuesses = 0;
    void OnStateChanged(string state)
    {
        switch (state)
        {
            case "main menu":
                VRtistrySyncer.instance.VRPlayerPoints = 0;

                paintTexture.Clear();

                RectTransform rt = (uiCanvas.transform as RectTransform);
                rt.position = canvasDefaultPos;
                rt.rotation = canvasDefaultRot;

                break;
            case "clients answering":
                if (VRtistrySyncer.instance.VRCompletedTutorial) vrPlayer.UIWarningArrow.SetActive(false);

                //Instantiate new drawing
                uint key = (uint)DrawingsSyncer.instance.Drawings.Count;
                DrawingsSyncer.instance.Drawings.Add(key, new DrawingModel());

                //Clear previous painting
                paintTexture.Clear();
                if (currentRound == 3) ClearRoast();

                VRtistrySyncer.instance.VRPlayerGuess = -1;

                VRtistrySyncer.instance.ClientAnswerTimer = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;

                if (!firstTimeClientsAnswering)
                {
                    paintBrush.CanPaintAir = true;
                    vrPlayer.Ahp.maxMoveSpeed = 3;
                }

                if (currentRound == ROAST_ROUND)
                {
                    ApplyRoast();

                    //Display text that players are answering
                    string clientChosenName = ClientPlayer.GetClientByCurrentOwnerID(VRtistrySyncer.instance.ChosenClientToRoast).syncer.Name;
                    headerText.text = "<u>Player Themed Round</u>\n\n" +
                       "Players are typing their answers about " + clientChosenName + "\n" +
                                "Use this time to practice painting!";
                }
                else
                {
                    //Display text that players are answering
                    headerText.text = "Players are typing their answers\n" +
                                      "Use this time to practice painting!";
                }

                firstTimeClientsAnswering = false;

                break;
            case "vr picking prompt":
                headerText.text = DONT_SAY_WARNING + "Pick a prompt:\n\n\n\n\n";

                //Disable VR tools
                paintBrush.CanPaintAir = false;
                DropTool();
                DropPalette();

                vrPlayer.leftUIPointerPreview.rayDrawDistance = pointerPreviewDrawDistance;
                vrPlayer.rightUIPointerPreview.rayDrawDistance = pointerPreviewDrawDistance;

                headerText.fontSize = 0.3f;
                (promptOptionRT.offsetMin, promptOptionRT.offsetMax) = (new Vector2(promptOptionRT.offsetMin.x, 0), new Vector2(promptOptionRT.offsetMax.x, -0.5f));

                //Instantiate option buttons
                foreach (ClientPlayer cp in ClientPlayer.clients)
                {
                    if(currentRound == ROAST_ROUND && VRtistrySyncer.instance.ChosenClientToRoast == cp.realtimeView.ownerIDSelf)
                    {
                        continue;
                    }

                    GameObject newOption = Instantiate(promptOptionButtonsPrefab, promptOptionButtonsParent.transform);
                    newOption.GetComponentInChildren<TMP_Text>().text = GetAnswerByOwnerID(cp.realtimeView.ownerIDSelf);
                    newOption.GetComponent<Button>().onClick.AddListener(delegate { promptOptionButtonClicked(cp.realtimeView.ownerIDSelf); });
                    promptOptionButtons.Add(newOption);
                }

                //Clear practice painting
                paintTexture.Clear();

                break;
            case "vr posing":
#if !UNITY_WEBGL
                fmodInstance.setParameterByName("VRtistryPhase", 1);
#endif
                vrPlayer.UIWarningArrow.SetActive(true);

                solver.EnablePosing();

                //UI
                countdownSlider.gameObject.SetActive(true);
                vrPlayer.leftUIPointerPreview.rayDrawDistance = 0;
                vrPlayer.rightUIPointerPreview.rayDrawDistance = 0;
                headerText.text = DONT_SAY_WARNING + "Your prompt is:\n <b>" + GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner) + "</b>\nPose your creation! Hold any button on your controllers to lock in your pose";

                poseHoldTimer = 0f;

                break;
            case "vr painting":
                vrPlayer.UIWarningArrow.SetActive(false);

                //Enable VR tools
                paintBrush.CanPaintAir = true;

                //UI
                vrPlayer.leftUIPointerPreview.rayDrawDistance = pointerPreviewDrawDistance;
                vrPlayer.rightUIPointer.enabled = false;
                headerText.text = DONT_SAY_WARNING + "Your prompt is: <b>" + GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner) + "</b>\n\n";
                timerText.enabled = true;
                finishedPaintingEarlyButton.gameObject.SetActive(true);
                break;
            case "clients typing guess":
                //Network paint texture, pose data, and title
                DrawingsSyncer.instance.Drawings.Last().Value.paintTexture = paintTexture.GetPngData();
                DrawingsSyncer.instance.StorePoseData(armature);
                DrawingsSyncer.instance.Drawings.Last().Value.title = GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner);

#if !UNITY_WEBGL
                fmodInstance.setParameterByName("VRtistryPhase", 0);
                fmodInstance.setParameterByName("VRtistryClock", 0);
#endif

#if !UNITY_WEBGL
                /*
                int numOfDecoysToGenerate = 8 - ClientPlayer.clients.Count;
                if (numOfDecoysToGenerate < 0 || numOfDecoysToGenerate > 7) numOfDecoysToGenerate = 0;
                decoyAnswersGenerator.GenerateFakeAnswers(VRtistrySyncer.instance.CurrentPrompt, GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner), numOfDecoysToGenerate, OnDecoyAnswersGenerated);
                */
#endif

                //Disable VR tools
                paintBrush.CanPaintAir = false;

                //UI
                vrPlayer.leftUIPointerPreview.rayDrawDistance = 0;
                vrPlayer.rightUIPointerPreview.rayDrawDistance = 0;
                vrPlayer.rightUIPointer.enabled = true;

                //Display all answers
                headerText.text = "Players are guessing what your art is";
                timerText.enabled = false;
                playerNamesIconParent.SetActive(true);

                finishedPaintingEarlyButton.gameObject.SetActive(false);

                SceneChangerSyncer.instance.FadeOutManual();
                vrPlayer.Ahp.maxMoveSpeed = 0;
                StartCoroutine(SetVRPlayerPos(vrPlayer.spawnPos, 1));
                break;
            case "clients guessing":
                break;
            case "vr guessing":
                vrPlayer.UIWarningArrow.SetActive(true);

                correctGuesses = 0;

                List<int> clientGuessOptions = ClientPlayer.GetRandomClientIdList(3, VRtistrySyncer.instance.ChosenAnswerOwner);

                headerText.fontSize = 0.225f;
                (promptOptionRT.offsetMin, promptOptionRT.offsetMax) = (new Vector2(promptOptionRT.offsetMin.x, -0.5f), new Vector2(promptOptionRT.offsetMax.x, -1));

                foreach (int g in clientGuessOptions)
                {
                    VRtistryClientPlayer vcp = (ClientPlayer.GetClientByCurrentOwnerID(g) as VRtistryClientPlayer);

                    if (currentRound == ROAST_ROUND && VRtistrySyncer.instance.ChosenClientToRoast == vcp.realtimeView.ownerIDSelf)
                    {
                        continue;
                    }

                    GameObject newOption = Instantiate(promptOptionButtonsPrefab, promptOptionButtonsParent.transform);
                    newOption.GetComponentInChildren<TMP_Text>().text = vcp.syncer.Name;
                    newOption.GetComponent<Image>().color = vcp.syncer.Color;
                    newOption.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(GuessPlayerVR(g)); });
                    promptOptionButtons.Add(newOption);
                }

                foreach (ClientPlayer cp in ClientPlayer.clients)
                {
                    int artGuess = cp.syncer.VrtistryArtGuess;
                    if (artGuess != -1 && GetAnswerByOwnerID(artGuess).Equals(GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner)))
                    {
                        correctGuesses++;
                    }
                }
                VRtistrySyncer.instance.VRPlayerPoints += ThreeDPaintGlobalVariables.calculatePointsVrCorrectGuesses(correctGuesses);

                //Header
                playerResultsHeaderText.text = "";
                headerText.text = "The prompt was:\n<b>" + VRtistrySyncer.instance.CurrentPrompt + "</b>\nClick on the player you think wrote:\n<b>" + GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner) + "\n\n\n";
                foreach (GameObject g in playerNameIcons)
                {
                    g.GetComponentInChildren<Button>().interactable = true;
                }

                //UX
                vrPlayer.leftUIPointerPreview.rayDrawDistance = pointerPreviewDrawDistance;
                vrPlayer.rightUIPointerPreview.rayDrawDistance = pointerPreviewDrawDistance;
                vrPlayer.leftHand.Release();
                vrPlayer.rightHand.Release();

                break;
            case "results":
                SetPlayerNamesVisibility(false);

                vrPlayer.UIWarningArrow.SetActive(false);

                headerText.text = "Displaying results";

                vrPlayer.leftUIPointerPreview.rayDrawDistance = 0;
                vrPlayer.rightUIPointerPreview.rayDrawDistance = 0;

                //Answer results — typed guesses (non-drawer clients)
                foreach (ClientPlayer cp in ClientPlayer.clients)
                {
                    string typedGuess = cp.syncer.VrtistryTypedGuess;
                    if (string.IsNullOrEmpty(typedGuess)) continue;

                    int id = cp.realtimeView.ownerIDSelf;
                    AnswerOptionButton aob = (cp as VRtistryClientPlayer).playerAnswer;
                    aob.ResetPlayerIcons();
                    aob.canvasGroup.alpha = 0;
                    aob.SetTextWithPlayerName(typedGuess, cp);
                    aob.playerID = "" + id;
                    aob.SetBorderColor(cp.syncer.Color);
                    aob.correctAnswerBanner.SetActive(id == VRtistrySyncer.instance.ChosenAnswerOwner);
                    (aob.correctAnswerBanner.transform as RectTransform).localScale = Vector3.zero;
                    answerResults.Add(aob);
                }

                //Correct answer (drawer's original prompt answer)
                {
                    int drawerId = VRtistrySyncer.instance.ChosenAnswerOwner;
                    ClientPlayer drawer = ClientPlayer.GetClientByCurrentOwnerID(drawerId);
                    if (drawer != null)
                    {
                        AnswerOptionButton aob = (drawer as VRtistryClientPlayer).playerAnswer;
                        aob.ResetPlayerIcons();
                        aob.canvasGroup.alpha = 0;
                        aob.SetTextWithPlayerName(drawer.syncer.VrtistryAnswer, drawer);
                        aob.playerID = "" + drawerId;
                        aob.SetBorderColor(drawer.syncer.Color);
                        aob.correctAnswerBanner.SetActive(true);
                        (aob.correctAnswerBanner.transform as RectTransform).localScale = Vector3.zero;
                        answerResults.Add(aob);
                    }
                }

                //Add player icons to the answer each client chose
                foreach (ClientPlayer cp in ClientPlayer.clients)
                {
                    int artGuess = cp.syncer.VrtistryArtGuess;
                    if (artGuess != -1)
                    {
                        AddPlayerToResults(cp.realtimeView.ownerIDSelf, "" + artGuess);
                    }
                }

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

                //Animate correct answer
                int correctAnswerDelay = k * ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME;
                correctAnswer.AnimateAnswers(correctAnswerDelay);

                //4 seconds are added for first-correct-guess bonus points
                if (correctGuesses > 0)
                {
                    correctAnswerDelay += 4;
                }

                //Display answers that got no guesses
                foreach (AnswerOptionButton aob in answersWithNoGuesses)
                {
                    aob.AnimateAnswers(correctAnswerDelay + (ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME * 2));
                }

                //3 Seconds are added to view answers that no one chose
                if (answersWithNoGuesses.Count > 0)
                {
                    Invoke("SetLeaderboardState", (correctAnswerDelay + (ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME * 2) + 3));
                }
                else
                {
                    Invoke("SetLeaderboardState", (correctAnswerDelay + (ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME * 2)));
                }

                //Pick client to roast for next round, if it is round 1
                if(currentRound == 1)
                {
                    VRtistrySyncer.instance.ChosenClientToRoast = ClientPlayer.GetRandomClientID();
                }

                break;
            case "leaderboard":
                StartCoroutine("ShowLeaderboard");

                break;
            case "gallery":
                vrPlayer.UIWarningArrow.SetActive(false);

                //Move canvas to gallery
                RectTransform rtt = (uiCanvas.transform as RectTransform);
                rtt.position = canvasGalleryTransform.position;
                rtt.rotation = canvasGalleryTransform.rotation;

                mannequinHeightSlider.gameObject.SetActive(false);

                DrawingsSyncer.instance.SetDrawingGalleryPositions();

                StartCoroutine("ShowLeaderboard");

                StartCoroutine(SetVRPlayerPos(vrPlayerGallerySpawnPos.position, 0));

                Invoke("ResetArmatureTransforms", 1);

                break;
            case "game over":
                StartCoroutine("EndGame");
                break;
            default:
                break;
        }
    }

    void ApplyRoast()
    {
        ClientPlayer cp = ClientPlayer.GetClientByCurrentOwnerID(VRtistrySyncer.instance.ChosenClientToRoast);
        mannequinFaceTexture.LoadFromData(cp.syncer.FaceDrawing);
        gallerySecondMannequinFaceTexture.LoadFromData(cp.syncer.FaceDrawing);

        Color.RGBToHSV(cp.syncer.Color, out float H, out float S, out float V);
        string col = ColorUtility.ToHtmlStringRGBA(cp.syncer.Color);

        Color fillColor;
        //Orange and Cyan appear too dark for some reason, idk man
        if (col.Equals("FF7F00FF"))
            fillColor = Color.HSVToRGB(H + 0.035f, S, V);
        else if (col.Equals("007F7FFF"))
            fillColor = Color.HSVToRGB(H, S, V + 0.2f);
        else
            fillColor = cp.syncer.Color;

        paintTexture.Color = fillColor;
        paintTexture.Clear();
        gallerySecondMannequinPaintTexture.Color = fillColor;
        gallerySecondMannequinPaintTexture.Clear();

        Material mat = paintTexture.gameObject.GetComponent<SkinnedMeshRenderer>().material;
        Material galMat = gallerySecondMannequinPaintTexture.gameObject.GetComponent<SkinnedMeshRenderer>().material;
        mat.SetColor("_ColorDim", Color.HSVToRGB(H, S, V - 0.2f));
        mat.SetColor("_OutlineColor", Color.HSVToRGB(H, S, V - 0.75f));
        galMat.SetColor("_ColorDim", Color.HSVToRGB(H, S, V - 0.2f));
        galMat.SetColor("_OutlineColor", Color.HSVToRGB(H, S, V - 0.75f));

        Invoke("ApplyFaceMat", 0.5f);
    }

    void ApplyFaceMat()
    {
        Material[] mats = mannequinFaceMesh.materials;
        Texture prevTex = mats[0].mainTexture;
        mats[0] = new Material(Shader.Find("Custom/UnlitAlphaOnTop")) { mainTexture = prevTex };
        mannequinFaceMesh.materials = mats;

        mats = gallerySecondMannequinFaceMesh.materials;
        prevTex = mats[0].mainTexture;
        mats[0] = new Material(Shader.Find("Custom/UnlitAlphaOnTop")) { mainTexture = prevTex };
        gallerySecondMannequinFaceMesh.materials = mats;
    }

    void ClearRoast()
    {
        mannequinFaceTexture.Clear();

        paintTexture.Color = Color.white;
        paintTexture.Clear();
        gallerySecondMannequinPaintTexture.Color = Color.white;
        gallerySecondMannequinPaintTexture.Clear();

        Material mat = paintTexture.gameObject.GetComponent<SkinnedMeshRenderer>().material;
        mat.SetColor("_ColorDim", new Color(0.7f, 0.7f, 0.7f));
        mat.SetColor("_OutlineColor", Color.black);
    }

    void promptOptionButtonClicked(int answerOwnerID)
    {
        VRtistrySyncer.instance.ChosenAnswerOwner = answerOwnerID;
        foreach (GameObject pob in promptOptionButtons)
        {
            Destroy(pob);
        }
        promptOptionButtons = new List<GameObject>();

        VRtistrySyncer.instance.State = "vr posing";
    }

    void SetPlayerNamesVisibility(bool visible)
    {
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            cp.SetPlayerNameVisibility(visible, true);
        }
    }

    public IEnumerator SetVRPlayerPos(Vector3 pos, int delay = 0)
    {
        yield return new WaitForSeconds(delay);

        vrPlayer.Ahp.SetPosition(pos);
        vrPlayer.trackerOffsetsParent.localRotation = Quaternion.Euler(0, 0, 0);
        SceneChangerSyncer.instance.FadeInManual();
    }

    void SetLeaderboardState()
    {
        if (currentRound == ThreeDPaintGlobalVariables.NUMBER_OF_ROUNDS)
        {
            VRtistrySyncer.instance.State = "gallery";
        }
        else
        {
            VRtistrySyncer.instance.State = "leaderboard";
        }

    }

    void ClearPlayerAnswers()
    {
        foreach (AnswerOptionButton aob in answerResults)
        {
            aob.canvasGroup.alpha = 0;
        }
        answerResults = new List<AnswerOptionButton>();
    }

    string GetAnswerByOwnerID(int ID)
    {
        ClientPlayer cp = ClientPlayer.GetClientByCurrentOwnerID(ID);
        return cp != null ? cp.syncer.VrtistryAnswer : "";
    }

    public void StartGame()
    {
        VRtistrySyncer.instance.CurrentPrompt = GetPrompt();
        VRtistrySyncer.instance.State = "clients answering";
    }

    IEnumerator EndGame()
    {
#if !UNITY_WEBGL
        fmodInstance.setParameterByName("VRtistryPhase", 2);
#endif

        yield return new WaitForSeconds(4);

#if !UNITY_WEBGL
        fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
#endif

        StartCoroutine(SetVRPlayerPos(vrPlayer.spawnPos, 0));

        //Disable leaderboard
        leaderboardParent.SetActive(false);
        ClearPlayerAnswers();

        VRtistrySyncer.instance.State = "main menu";
        mainMenuManager.ReturnToMainMenu();
        DrawingsSyncer.instance.ResetDrawingsSyncer();

#if !UNITY_WEBGL
        yield return new WaitForSeconds(2);
        fmodInstance.setParameterByName("VRtistryPhase", 0);
        fmodInstance.setParameterByName("VRtistryClock", 0);
        fmodInstance.setTimelinePosition(0);

        fmodInstance.start();
#endif
    }

    IEnumerator ShowLeaderboard()
    {
        //Clear the player buttons
        foreach (GameObject g in playerNameIcons)
        {
            Destroy(g);
        }
        playerNameIcons = new List<GameObject>();

        //Clear previous leaderboard
        foreach (GameObject g in currentLeaderboardCards)
        {
            Destroy(g);
        }
        currentLeaderboardCards = new List<GameObject>();

        //Need to wait a frame to allow card objects to be destroyed so sibling index parameter works
        yield return new WaitForEndOfFrame();

        //Show leaderboard
        headerText.text = "";
        playerResultsHeaderText.text = "";
        timerText.text = "";
        leaderboardParent.SetActive(true);

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
            GameObject newCard = Instantiate(leaderboardPlayerCardPrefab, leaderboardParent.transform);
            (newCard.transform as RectTransform).localScale = new Vector3(0, 1, 1);
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
        GameObject vrCard = Instantiate(leaderboardPlayerCardPrefab, leaderboardParent.transform);
        (vrCard.transform as RectTransform).localScale = new Vector3(0, 1, 1);
        vrCard.GetComponentsInChildren<TMP_Text>()[0].text = "VR Player";
        //vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "";
        vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "" + VRtistrySyncer.instance.VRPlayerPoints;
        vrCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + (vrPlayerPos + 1);
        vrCard.transform.SetSiblingIndex(vrPlayerPos);

        currentLeaderboardCards.Add(vrCard);

        //Animate cards in
        foreach (RectTransform rt in leaderboardParent.GetComponentsInChildren<RectTransform>())
        {
            if (rt.gameObject.name.Contains("LeaderboardPlayerCardVR"))
            {
                rt.DOScaleX(1, 0.25f);
                yield return new WaitForSeconds(0.125f);
            }
        }

        yield return new WaitForSeconds(ThreeDPaintGlobalVariables.LEADERBOARD_DISPLAY_TIME);

        if (currentRound == ThreeDPaintGlobalVariables.NUMBER_OF_ROUNDS)
        {
            currentRound = 1;
            VRtistrySyncer.instance.State = "game over";
        }
        else
        {
            currentRound++;

            //Disable leaderboard
            leaderboardParent.SetActive(false);
            ClearPlayerAnswers();

            //Setup next round
            VRtistrySyncer.instance.CurrentPrompt = currentRound == ROAST_ROUND ? GetClientThemedPrompt() : GetPrompt();
            VRtistrySyncer.instance.State = "clients answering";
            VRtistrySyncer.instance.DrawingTimer = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;
        }

        SetPlayerNamesVisibility(true);
    }

    void PlayerAnswered()
    {
        int clientAnswersNeededToProceed = (currentRound == ROAST_ROUND) ? ClientPlayer.clients.Count - 1 : ClientPlayer.clients.Count;
        int answeredCount = ClientPlayer.clients.Count(cp => !string.IsNullOrEmpty(cp.syncer.VrtistryAnswer));
        if (answeredCount >= clientAnswersNeededToProceed && VRtistrySyncer.instance.VRCompletedTutorial)
        {
            VRtistrySyncer.instance.State = "vr picking prompt";
        }
    }

    void PlayerTypedGuess()
    {
        int typedGuessCount = ClientPlayer.clients.Count(cp => !string.IsNullOrEmpty(cp.syncer.VrtistryTypedGuess));
        if (typedGuessCount >= (ClientPlayer.clients.Count - 1) && VRtistrySyncer.instance.State.Equals("clients typing guess"))
        {
            VRtistrySyncer.instance.State = "clients guessing";
        }
    }

    void PlayerGuessedArt()
    {
        int artGuessCount = ClientPlayer.clients.Count(cp => cp.syncer.VrtistryArtGuess != -1);
        if (artGuessCount >= (ClientPlayer.clients.Count - 1) && VRtistrySyncer.instance.State.Equals("clients guessing"))
        {
            VRtistrySyncer.instance.State = "vr guessing";
        }
    }

    void PlayerGuessedPlayer()
    {
        int playerGuessCount = ClientPlayer.clients.Count(cp => cp.syncer.VrtistryPlayerGuess != -1);
        Debug.Log("VRPlayerGuess: " + VRtistrySyncer.instance.VRPlayerGuess + "  Player guesses: " + playerGuessCount + "/" + ClientPlayer.clients.Count + "   state: " + VRtistrySyncer.instance.State);
        if (VRtistrySyncer.instance.VRPlayerGuess != -1 && playerGuessCount >= ClientPlayer.clients.Count && VRtistrySyncer.instance.State.Equals("vr guessing"))
        {
            VRtistrySyncer.instance.State = "results";
        }
    }

    string GetPrompt()
    {
        //TEMP custom prompts for testing
        /*
        if(currentRound == 1)
        {
            return "What sport could you totally beat VR player in?";
        }
        else if (currentRound == ROAST_ROUND)
        {
            return "The worst costume you could wear to a halloween party";
        }
        else
        {
            return "Who is the most powerful videogame character you can think of?";
        }
        */

        //Split prompt text file into individual lines
        string[] prompts = promptList.ToString().Split('\n');

        //Get prompt at random, excluding comments
        string newPrompt;
        do
        {
            newPrompt = prompts[Random.Range(0, prompts.Length)];
        }
        while (newPrompt.StartsWith("//"));

        return newPrompt;
    }

    string GetClientThemedPrompt()
    {
        //Split prompt text file into individual lines
        string[] prompts = clientThemedPromptList.ToString().Split('\n');

        //Get prompt at random, excluding comments
        string newPrompt;
        do
        {
            newPrompt = prompts[Random.Range(0, prompts.Length)];
        }
        while (newPrompt.StartsWith("//"));

        //Replace PLAYER_NAME with client's name
        newPrompt = newPrompt.Replace("PLAYER_NAME", ClientPlayer.GetClientByCurrentOwnerID(VRtistrySyncer.instance.ChosenClientToRoast).syncer.Name);

        return newPrompt;
    }

    void AddPlayerToResults(int playerID, bool correct)
    {
        GameObject pi = Instantiate(playerNameIconPrefab, playerNamesIconParent.transform);
        pi.GetComponentInChildren<TMP_Text>(true).text = ClientPlayer.GetClientByCurrentOwnerID(playerID).syncer.Name;
        pi.GetComponentInChildren<Button>(true).onClick.AddListener(delegate { StartCoroutine(GuessPlayerVR(playerID)); });
        pi.GetComponentInChildren<Button>(true).interactable = false;
        //pi.GetComponent<Image>().color = correct ? Color.green : Color.red;
        pi.GetComponent<Image>().color = ClientPlayer.GetClientByCurrentOwnerID(playerID).syncer.Color;

        playerNameIcons.Add(pi);
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

    /// <summary>
    /// Method called when VR player guesses which player wrote the selected prompt
    /// </summary>
    IEnumerator GuessPlayerVR(int playerID)
    {
        VRtistrySyncer.instance.VRPlayerGuess = playerID;

        if (playerID.Equals(VRtistrySyncer.instance.ChosenAnswerOwner))
        {
            VRtistrySyncer.instance.VRPlayerPoints += ThreeDPaintGlobalVariables.POINTS_VR_CORRECT_PLAYER;
            headerText.text = "Correct!";
        }
        else
        {
            headerText.text = "Wrong! " + ClientPlayer.GetClientByCurrentOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner).syncer.Name + " wrote the answer";
        }

        //Remove client buttons
        foreach (GameObject pob in promptOptionButtons)
        {
            Destroy(pob);
        }
        promptOptionButtons = new List<GameObject>();

        yield return new WaitForSeconds(3);

        if (!VRtistrySyncer.instance.State.Equals("results"))
        {
            headerText.text = "Waiting for players to submit guesses";

            //If all clients have also guessed, move to results phase
            int playerGuessCount = ClientPlayer.clients.Count(cp => cp.syncer.VrtistryPlayerGuess != -1);
            if (playerGuessCount >= ClientPlayer.clients.Count && VRtistrySyncer.instance.State.Equals("vr guessing"))
            {
                VRtistrySyncer.instance.State = "results";
            }
        }
    }

    [Button]
    public void GuessFirstPlayer()
    {
        if (promptOptionButtons.Count > 0)
            promptOptionButtons[0].GetComponent<Button>().onClick.Invoke();
    }

    void GrabTool()
    {
        SetColliders(false);

        bool isToolHandLeft = (toolHand == HandType.left);

        //Drop current tool
        if (isToolHandLeft)
        {
            vrPlayer.leftHand.ForceReleaseGrab();
        }
        else
        {
            vrPlayer.rightHand.ForceReleaseGrab();
        }

        //Enable tool
        VRtistrySyncer.instance.IsBrushEnabled = true;
        //pen.transform.position = isToolHandLeft ? leftHandGrabPoint.transform.position : rightHandGrabPoint.transform.position;

        //Temporarily disable palette so it doesn't get in the way of grabbing
        foreach (Collider c in paintPalette.gameObject.GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }

        //Setup constraint
        ParentConstraint pc = paintBrush.constraint;
        if (pc.sourceCount > 0) pc.RemoveSource(0);
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = isToolHandLeft ? vrPlayer.leftHandGrabPoint : vrPlayer.rightHandGrabPoint;
        src.weight = 1;
        pc.AddSource(src);
        pc.constraintActive = true;
        paintBrush.transform.SetPositionAndRotation(src.sourceTransform.position, src.sourceTransform.rotation);
        Physics.SyncTransforms();

        //Grab proper tool
        if (isToolHandLeft)
        {
            vrPlayer.leftHand.Grab(GrabType.InstantGrab);
        }
        else
        {
            vrPlayer.rightHand.Grab(GrabType.InstantGrab);
        }

        shouldToolsBeVisible = true;
    }

    void DropTool()
    {
        bool isToolHandLeft = (toolHand == HandType.left);

        //Drop current tool
        if (isToolHandLeft)
        {
            vrPlayer.leftHand.ForceReleaseGrab();
        }
        else
        {
            vrPlayer.rightHand.ForceReleaseGrab();
        }

        VRtistrySyncer.instance.IsBrushEnabled = false;
        paintBrush.SetActive(false);

        shouldToolsBeVisible = false;
    }

    IEnumerator GrabDelayed(bool isToolHandLeft, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isToolHandLeft)
        {
            vrPlayer.leftHand.Grab(GrabType.InstantGrab);
        }
        else
        {
            vrPlayer.rightHand.Grab(GrabType.InstantGrab);
        }
    }

    public void OnGrabbed(Hand h, Grabbable g)
    {
        //Debug.Log("Successfully grabbed: " + g.gameObject.name);

        if (g.gameObject.name.Equals("PaintBrush(Clone)"))
        {
            //Re-enable palette colliders
            foreach (Collider c in paintPalette.gameObject.GetComponentsInChildren<Collider>())
            {
                c.enabled = true;
            }

            //Grab palette once tool has been grabbed if necessary
            if (needToGrabPalette)
            {
                GrabPalette();
            }
            else
            {
                SetColliders(true);
            }

            //Handle constraints
            if (VRtistrySyncer.instance.IsBrushEnabled)
            {
                paintBrush.constraint.constraintActive = false;
            }

            //Ensure mesh visibility
            if (VRtistrySyncer.instance.IsBrushEnabled)
            {
                paintBrush.SetMeshVisibility(true);
            }
        }
        else if (g.gameObject.name.Equals("PaintPalette(Clone)"))
        {
            if (VRtistrySyncer.instance.IsBrushEnabled)
            {
                paintBrush.col.enabled = true;
                paintBrush.tipCol.enabled = true;
            }

            SetColliders(true);
        }
    }

    int toggleToolHandCooldown = 1;
    float timeLastToggled;
    public void ToggleToolHand()
    {
        //Short cooldown for toggling tool hand to help prevent grabbables from getting stuck
        if (Time.time < (timeLastToggled + toggleToolHandCooldown))
            return;

        vrPlayer.leftHand.ForceReleaseGrab();
        vrPlayer.rightHand.ForceReleaseGrab();

        toolHand = (toolHand == HandType.left) ? HandType.right : HandType.left;

        paintBrush.GetComponent<Grabbable>().handType = toolHand;
        paintPalette.gameObject.GetComponent<Grabbable>().handType = (toolHand == HandType.left) ? HandType.right : HandType.left;

        needToGrabPalette = true;
        GrabTool();

        timeLastToggled = Time.time;
    }

    void GrabPalette()
    {
        SetColliders(false);

        VRtistrySyncer.instance.IsPaletteEnabled = true;

        bool isPaletteHandLeft = !(toolHand == HandType.left);

        if (paintPalette.currentMeshLeft != isPaletteHandLeft)
        {
            VRtistrySyncer.instance.IsPaletteMirrored = !VRtistrySyncer.instance.IsPaletteMirrored;
        }

        ParentConstraint pc = paintPalette.constraint;

        pc.RemoveSource(0);
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = isPaletteHandLeft ? vrPlayer.leftHandGrabPoint : vrPlayer.rightHandGrabPoint;
        src.weight = 1;
        pc.AddSource(src);
        pc.constraintActive = true;

        //Drop whatever's in hand
        if (isPaletteHandLeft)
        {
            vrPlayer.leftHand.ForceReleaseGrab();
        }
        else
        {
            vrPlayer.rightHand.ForceReleaseGrab();
        }

        if (VRtistrySyncer.instance.IsBrushEnabled)
        {
            paintBrush.GetComponent<Collider>().enabled = false;
        }

        paintPalette.gameObject.GetComponent<MeshCollider>().enabled = true;

        //Grab palette
        if (isPaletteHandLeft)
        {
            vrPlayer.leftHand.Grab(GrabType.InstantGrab);
        }
        else
        {
            vrPlayer.rightHand.Grab(GrabType.InstantGrab);
        }

        paintPalette.constraint.constraintActive = false;

        needToGrabPalette = false;
    }

    void DropPalette()
    {
        bool isPaletteHandLeft = !(toolHand == HandType.left);

        if (isPaletteHandLeft)
        {
            vrPlayer.leftHand.ForceReleaseGrab();
        }
        else
        {
            vrPlayer.rightHand.ForceReleaseGrab();
        }

        VRtistrySyncer.instance.IsPaletteEnabled = false;

        paintPalette.gameObject.GetComponent<MeshCollider>().enabled = false;

        ParentConstraint pc = paintPalette.constraint;

        pc.RemoveSource(0);
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = isPaletteHandLeft ? vrPlayer.leftHandGrabPoint : vrPlayer.rightHandGrabPoint;
        src.weight = 1;
        pc.AddSource(src);
        pc.constraintActive = true;
    }

    //Enable/Disable colliders that could get in the way of grabbing
    void SetColliders(bool active)
    {
        vrPlayer.Ahp.HeadPhysicsFollower.headCollider.enabled = active;
        vrPlayer.Ahp.capsuleColl.enabled = active;
    }

    void ResetArmatureTransforms()
    {
        //Start at index 3 to avoid armature parent + hips
        for (int i = 3; i < armatureRTs.Length; i++)
        {
            armatureRTs[i].transform.position = armaturePositions[i];
            armatureRTs[i].transform.rotation = armatureRotations[i];
        }
        solver.SetPose(); //Make sure collider is reset
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}