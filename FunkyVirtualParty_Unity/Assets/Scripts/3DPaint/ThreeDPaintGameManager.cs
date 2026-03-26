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

public class ThreeDPaintGameManager : MonoBehaviour
{
    [SerializeField]
    TextAsset promptList;

    [SerializeField]
    Canvas uiCanvas;

    [SerializeField]
    TMP_Text headerText, playerResultsHeaderText, timerText, poseCountdownText;

    [SerializeField]
    P3dPaintableTexture paintTexture;

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
    GeminiFakeAnswersGenerator decoyAnswersGenerator;

    List<AnswerOptionButton> answerResults;

    [SerializeField]
    RectTransform canvasGalleryTransform;

    [SerializeField]
    Transform vrPlayerGallerySpawnPos;

    [SerializeField]
    MannequinHeightSlider mannequinHeightSlider;

    [SerializeField]
    Transform linesParent;

#if !UNITY_WEBGL
    [SerializeField]
    EventReference musicEvent;

    EventInstance fmodInstance;
#endif

    PaintBrush paintBrush;

    PaintPalette paintPalette;

    int currentRound = 1;

    HandType toolHand = HandType.right;

    bool needToGrabPalette;

    private VRtistryVRPlayerController vrPlayer;

    private float pointerPreviewDrawDistance;

    List<Tween> clientPulsateTweens;

    int[] numOfPointersOnClients; //How many pointers are currently on a client? Used to determine highlighting effects

    const string dontSayWarning = "<sprite=0> <size=0.1px><color=#F6AC70><u><b>DON'T SAY THIS OUTLOUD!</b></u></color></size>\n";

    RealtimeTransform[] armatureRTs;
    Vector3[] armaturePositions;
    Quaternion[] armatureRotations;

    private void Awake()
    {
        answerResults = new List<AnswerOptionButton>();

        playerNameIcons = new List<GameObject>();
        currentLeaderboardCards = new List<GameObject>();

        clientPulsateTweens = new List<Tween>();

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

        Invoke("CreateClientPlayerButtons", 1);

        StartCoroutine("StartGame");

#if !UNITY_WEBGL
        fmodInstance = RuntimeManager.CreateInstance(musicEvent);
        fmodInstance.start();
#endif
    }

    private void Start()
    {
        //Spawn tools
        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = true;

        paintBrush = Realtime.Instantiate("PaintBrush", Vector3.zero, Quaternion.identity, options).GetComponent<PaintBrush>();
        paintBrush.LinesParent = linesParent;
#if !UNITY_WEBGL
        //TODO: CHANGED FOR STANDALONE
        //paintBrush.gm = this;
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

        VRtistrySyncer.instance.OnStateChangeEvent.AddListener(OnStateChanged);
        ClientSync.OnAnyVrtistryAnswerChanged.AddListener(PlayerAnswered);
        ClientSync.OnAnyVrtistryArtGuessChanged.AddListener(PlayerGuessedArt);
        ClientSync.OnAnyVrtistryPlayerGuessChanged.AddListener(PlayerGuessedPlayer);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarDestroyed += RealtimeAvatarManager_avatarDestroyed;

        InvokeRepeating("Test", 1, 1);
    }

    public void CreateClientPlayerButtons()
    {
        numOfPointersOnClients = new int[ClientPlayer.clients.Count];

        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            //Create a new button object
            GameObject newButton = new GameObject("GeneratedButton", typeof(RectTransform), typeof(Button), typeof(Image));
            Button b = newButton.GetComponent<Button>();
            (cp as VRtistryClientPlayer).playerButton = b;
            newButton.GetComponent<Image>().color = Color.clear;
            FaceCamera faceCamera = newButton.AddComponent<FaceCamera>();
            faceCamera.transformIsRect = true;

            //Set it as a child of the canvas
            newButton.transform.SetParent(uiCanvas.transform, false);

            //Configure RectTransform
            RectTransform buttonRect = newButton.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(1, 2); // Default size for a button
            buttonRect.position = cp.transform.position;

            //Callbacks
            EventTrigger eventTrigger = newButton.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };

            EventTrigger.Entry exit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };

            entry.callback.AddListener(delegate {
                //Pause all tweens. Keep highlighted player outlined, and remove outline from all other clients
                int highlightedClientIndex = ClientPlayer.clients.IndexOf(cp);
                numOfPointersOnClients[highlightedClientIndex]++;

                for (int i = 0; i < clientPulsateTweens.Count; i++)
                {
                    clientPulsateTweens[i].Pause();
                    ClientPlayer.clients[i].smr.material.SetColor("_OutlineColor", ClientPlayer.clients[i].outlineColor);
                }
                cp.smr.material.SetColor("_OutlineColor", Color.white);
            });
            exit.callback.AddListener(delegate {
                int highlightedClientIndex = ClientPlayer.clients.IndexOf(cp);
                numOfPointersOnClients[highlightedClientIndex]--;

                for (int i = 0; i < clientPulsateTweens.Count; i++)
                {
                    if (numOfPointersOnClients[i] <= 0)
                    {
                        ClientPlayer.clients[i].smr.material.SetColor("_OutlineColor", ClientPlayer.clients[i].outlineColor);
                        clientPulsateTweens[i].Play();
                    }
                }
            });

            eventTrigger.triggers.Add(entry);
            eventTrigger.triggers.Add(exit);

            newButton.SetActive(false);
        }
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
        ClientSync.OnAnyVrtistryArtGuessChanged.RemoveListener(PlayerGuessedArt);
        ClientSync.OnAnyVrtistryPlayerGuessChanged.RemoveListener(PlayerGuessedPlayer);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarDestroyed -= RealtimeAvatarManager_avatarDestroyed;

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

        if((toolHand == HandType.left && left) || (toolHand == HandType.right && !left))
        {
            if(VRtistrySyncer.instance.IsBrushEnabled)
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

    float timeVRPosingStarted;
    private void Update()
    {
        if (numOfPointersOnClients != null)
        {
            string s = "";
            foreach (int i in numOfPointersOnClients)
            {
                s += i + ", ";
            }
        }

        switch (VRtistrySyncer.instance.State)
        {
            case "clients answering":
                VRtistrySyncer.instance.ClientAnswerTimer -= Time.deltaTime;

                if (VRtistrySyncer.instance.ClientAnswerTimer <= 0 && VRtistrySyncer.instance.VRCompletedTutorial)
                {
                    VRtistrySyncer.instance.State = "vr posing";
                }

                break;
            case "vr posing":

                //Give a 2 second buffer for players to realize what's happening so they don't accidentally press a button too soon
                if (Time.time > timeVRPosingStarted + 2)
                {
                    //Set pose if any button is pressed
                    if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Two) ||
                       OVRInput.GetDown(OVRInput.Button.Three) || OVRInput.GetDown(OVRInput.Button.Four) ||
                       OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || OVRInput.Get(OVRInput.RawButton.RIndexTrigger) ||
                       OVRInput.Get(OVRInput.RawButton.LHandTrigger) || OVRInput.Get(OVRInput.RawButton.RHandTrigger))
                    {
                        //Invoke("SetPose", 0.1f);
                        SetPose();
                    }
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
                    VRtistrySyncer.instance.State = "clients guessing";
                }
                break;
            case "clients guessing":
                break;
            case "vr guessing":
                break;
            default:
                break;
        }

        if(!VRtistrySyncer.instance.State.Equals("vr posing"))
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
    public void PressSkipButton()
    {
        OnTutorialCompleted();
        tutorial.SkipButtonPressed();
    }

    public void OnTutorialCompleted()
    {
        vrPlayer.UIWarningArrow.SetActive(false);
        VRtistrySyncer.instance.VRCompletedTutorial = true;

        int answeredCount = ClientPlayer.clients.Count(cp => !string.IsNullOrEmpty(cp.syncer.VrtistryAnswer));
        if (answeredCount >= ClientPlayer.clients.Count)
        {
            VRtistrySyncer.instance.State = "vr posing";
        }

        if (VRtistrySyncer.instance.State == "" || VRtistrySyncer.instance.State == "clients answering")
        {
            //Enable VR tools
            paintBrush.CanPaintAir = true;

            GrabToolsStart();

            //Display text that players are answering
            headerText.text = "Players are typing their answers \nUse this time to practice painting!";

            paintBrush.CanPaintAir = true;
        }

        headerText.enabled = true;

        vrPlayer.leftUIPointerPreview.rayDrawDistance = 0;
        vrPlayer.rightUIPointerPreview.rayDrawDistance = 0;
    }

    [Button]
    public void FinishedPaintingEarly()
    {
        VRtistrySyncer.instance.State = "clients guessing";
    }

    [Button]
    void SetPose()
    {
        StartCoroutine("StartPoseCountdownTimer", 3);
    }

    IEnumerator StartPoseCountdownTimer(int countdown)
    {
        headerText.text = "Locking in your pose. Hold still!\n\n";
        poseCountdownText.enabled = true;

        for (int i = countdown; i > 0; i--)
        {
            poseCountdownText.text = "" + i;
            poseCountdownText.transform.localScale = new Vector3(1, 1, 1);
            poseCountdownText.transform.DOScale(2, 0.25f);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        poseCountdownText.enabled = false;

        solver.SetPose();

        /*
        while (armature.transform.localPosition != Vector3.zero)
        {
            Debug.Log("Setting Pose");
            solver.SetPose();
        }
        */

        GrabToolsStart();
        VRtistrySyncer.instance.State = "vr painting";
    }

    bool firstTimeClientsAnswering = true;
    void OnStateChanged(string state)
    {
        switch (state)
        {
            case "clients answering":
                if(VRtistrySyncer.instance.VRCompletedTutorial) vrPlayer.UIWarningArrow.SetActive(false);

                //Instantiate new drawing
                uint key = (uint)DrawingsSyncer.instance.Drawings.Count;
                Debug.Log("Key: " + key);
                DrawingsSyncer.instance.Drawings.Add(key, new DrawingModel());

                //Clear previous painting
                paintTexture.Clear();

                VRtistrySyncer.instance.VRPlayerGuess = -1;

                VRtistrySyncer.instance.ClientAnswerTimer = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;

                if (!firstTimeClientsAnswering)
                {
                    paintBrush.CanPaintAir = true;
                    vrPlayer.Ahp.useMovement = true;
                }

                //Display text that players are answering
                headerText.text = "Players are typing their answers \nUse this time to practice painting!";

                firstTimeClientsAnswering = false;

                break;
            case "vr posing":
#if !UNITY_WEBGL
                fmodInstance.setParameterByName("VRtistryPhase", 1);
#endif

                vrPlayer.UIWarningArrow.SetActive(true);

                solver.EnablePosing();

                //Disable VR tools
                paintBrush.CanPaintAir = false;
                DropTool();
                DropPalette();

                //Get random answer
                VRtistrySyncer.instance.ChosenAnswerOwner = ClientPlayer.clients[Random.Range(0, ClientPlayer.clients.Count)].realtimeView.ownerIDSelf;

                //UI
                headerText.text = dontSayWarning + "Your prompt is:\n <b>" + GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner) + "</b>\nStart by posing your creation! Press any button on your controllers to lock in your pose";

                //Clear practice painting
                paintTexture.Clear();

                timeVRPosingStarted = Time.time;

                break;
            case "vr painting":
                vrPlayer.UIWarningArrow.SetActive(false);

                //Enable VR tools
                paintBrush.CanPaintAir = true;

                //UI
                vrPlayer.leftUIPointerPreview.rayDrawDistance = pointerPreviewDrawDistance;
                vrPlayer.rightUIPointer.enabled = false;
                headerText.text = dontSayWarning + "Your prompt is: <b>" + GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner) + "</b>\n\n";
                timerText.enabled = true;
                finishedPaintingEarlyButton.gameObject.SetActive(true);
                break;
            case "clients guessing":
                //Network paint texture, pose data, and title
                DrawingsSyncer.instance.Drawings.Last().Value.paintTexture = paintTexture.GetPngData();
                DrawingsSyncer.instance.StorePoseData(armature);
                DrawingsSyncer.instance.Drawings.Last().Value.title = GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner);

#if !UNITY_WEBGL
                fmodInstance.setParameterByName("VRtistryPhase", 0);
                fmodInstance.setParameterByName("VRtistryClock", 0);
#endif

#if !UNITY_WEBGL
                int numOfDecoysToGenerate = 8 - ClientPlayer.clients.Count;
                if (numOfDecoysToGenerate < 0 || numOfDecoysToGenerate > 7) numOfDecoysToGenerate = 0;
                decoyAnswersGenerator.GenerateFakeAnswers(VRtistrySyncer.instance.CurrentPrompt, GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner), numOfDecoysToGenerate, OnDecoyAnswersGenerated);
#endif

                //Disable VR tools
                paintBrush.CanPaintAir = false;
                vrPlayer.leftUIPointerPreview.rayDrawDistance = 0;
                vrPlayer.rightUIPointerPreview.rayDrawDistance = 0;
                vrPlayer.rightUIPointer.enabled = true;

                //Display all answers
                headerText.text = "Clients are guessing what your art is";
                timerText.enabled = false;
                playerNamesIconParent.SetActive(true);

                finishedPaintingEarlyButton.gameObject.SetActive(false);

                SceneChangerSyncer.instance.FadeOutManual();
                vrPlayer.Ahp.useMovement = false;
                StartCoroutine(SetVRPlayerPos(vrPlayer.spawnPos, 1));
                break;
            case "vr guessing":
                vrPlayer.UIWarningArrow.SetActive(true);

                //Show guesses
                int correctGuesses = 0;
                foreach (ClientPlayer cp in ClientPlayer.clients)
                {
                    int artGuess = cp.syncer.VrtistryArtGuess;
                    int guesserID = cp.realtimeView.ownerIDSelf;

                    VRtistryClientPlayer vcp = (ClientPlayer.GetClientByCurrentOwnerID(guesserID) as VRtistryClientPlayer);
                    vcp.playerButton.onClick.AddListener(delegate { StartCoroutine(GuessPlayerVR(guesserID)); vcp.smr.material = clientMat; });

                    if (artGuess != -1 && GetAnswerByOwnerID(artGuess).Equals(GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner)))
                    {
                        correctGuesses++;
                    }
                }
                VRtistrySyncer.instance.VRPlayerPoints += ThreeDPaintGlobalVariables.calculatePointsVrCorrectGuesses(correctGuesses);

                //Header
                playerResultsHeaderText.text = "";
                headerText.text = "The prompt was:\n<b>" + VRtistrySyncer.instance.CurrentPrompt + "</b>\nClick on the player you think wrote:\n<b>" + GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner);
                foreach (GameObject g in playerNameIcons)
                {
                    g.GetComponentInChildren<Button>().interactable = true;
                }

                //UX
                vrPlayer.leftUIPointerPreview.rayDrawDistance = pointerPreviewDrawDistance;
                vrPlayer.rightUIPointerPreview.rayDrawDistance = pointerPreviewDrawDistance;
                vrPlayer.leftHand.Release();
                vrPlayer.rightHand.Release();

                foreach (ClientPlayer cp in ClientPlayer.clients)
                {
                    (cp as VRtistryClientPlayer).SetButtonInteractable(true);
                    clientPulsateTweens.Add(cp.smr.material.DOColor(Color.gray, "_OutlineColor", 1).SetLoops(9999, LoopType.Yoyo));
                }

                break;
            case "results":
                vrPlayer.UIWarningArrow.SetActive(false);

                headerText.text = "Displaying results";

                vrPlayer.leftUIPointerPreview.rayDrawDistance = 0;
                vrPlayer.rightUIPointerPreview.rayDrawDistance = 0;

                //Answer results
                foreach (ClientPlayer cp in ClientPlayer.clients)
                {
                    string answer = cp.syncer.VrtistryAnswer;
                    if (string.IsNullOrEmpty(answer)) continue;
                    int id = cp.realtimeView.ownerIDSelf;

                    AnswerOptionButton aob = (cp as VRtistryClientPlayer).playerAnswer;
                    aob.ResetPlayerIcons();
                    aob.canvasGroup.alpha = 0;
                    aob.SetText(answer);
                    aob.playerID = "" + id;
                    aob.SetBorderColor(cp.syncer.Color);
                    aob.correctAnswerBanner.SetActive(id == VRtistrySyncer.instance.ChosenAnswerOwner);
                    (aob.correctAnswerBanner.transform as RectTransform).localScale = Vector3.zero;
                    answerResults.Add(aob);
                }

                //All players have guessed, so add guesses to results
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

                //Display answers that got no guesses
                foreach (AnswerOptionButton aob in answersWithNoGuesses)
                {
                    aob.AnimateAnswers(correctAnswerDelay + (ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME * 2));
                }

                //3 Seconds are added to view answers to view answers that no one chose
                if (answersWithNoGuesses.Count > 0)
                {
                    Invoke("SetLeaderboardState", (correctAnswerDelay + (ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME * 2) + 3));
                }
                else
                {
                    Invoke("SetLeaderboardState", (correctAnswerDelay + (ThreeDPaintGlobalVariables.PLAYER_ANSWER_ANIMATION_TIME * 2)));
                }

                break;
            case "leaderboard":
                StartCoroutine("ShowLeaderboard");

                break;
            case "gallery":
                vrPlayer.UIWarningArrow.SetActive(false);

                //Move canvas to gallery
                RectTransform rt = (uiCanvas.transform as RectTransform);
                rt.position = canvasGalleryTransform.position;
                rt.rotation = canvasGalleryTransform.rotation;

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

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);

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

        yield return new WaitForSeconds(1);

        //Destroy tools
        Realtime.Destroy(paintBrush.gameObject);
        Realtime.Destroy(paintPalette.gameObject);

        SceneChangerSyncer.instance.CurrentScene = "MainMenu";
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
        //if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ShowLeaderboard", "");

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
        vrCard.GetComponentsInChildren<TMP_Text>()[0].text = "VR Player";
        //vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "";
        vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "" + VRtistrySyncer.instance.VRPlayerPoints;
        vrCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + (vrPlayerPos + 1);
        vrCard.transform.SetSiblingIndex(vrPlayerPos);

        currentLeaderboardCards.Add(vrCard);

        yield return new WaitForSeconds(ThreeDPaintGlobalVariables.LEADERBOARD_DISPLAY_TIME);

        if (currentRound == ThreeDPaintGlobalVariables.NUMBER_OF_ROUNDS)
        {
            VRtistrySyncer.instance.State = "game over";
        }
        else
        {
            //Disable leaderboard
            leaderboardParent.SetActive(false);
            ClearPlayerAnswers();

            //Setup next round
            VRtistrySyncer.instance.CurrentPrompt = GetPrompt();
            VRtistrySyncer.instance.State = "clients answering";
            VRtistrySyncer.instance.DrawingTimer = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;

            currentRound++;
        }
    }

    void PlayerAnswered()
    {
        int answeredCount = ClientPlayer.clients.Count(cp => !string.IsNullOrEmpty(cp.syncer.VrtistryAnswer));
        if (answeredCount >= ClientPlayer.clients.Count && VRtistrySyncer.instance.VRCompletedTutorial)
        {
            VRtistrySyncer.instance.State = "vr posing";
        }
    }

    void PlayerGuessedArt()
    {
        int artGuessCount = ClientPlayer.clients.Count(cp => cp.syncer.VrtistryArtGuess != -1);
        if (artGuessCount >= ClientPlayer.clients.Count && VRtistrySyncer.instance.State.Equals("clients guessing"))
        {
            VRtistrySyncer.instance.State = "vr guessing";
        }
    }

    void PlayerGuessedPlayer()
    {
        int playerGuessCount = ClientPlayer.clients.Count(cp => cp.syncer.VrtistryPlayerGuess != -1);
        if (VRtistrySyncer.instance.VRPlayerGuess != -1 && playerGuessCount >= ClientPlayer.clients.Count && VRtistrySyncer.instance.State.Equals("vr guessing"))
        {
            VRtistrySyncer.instance.State = "results";
        }
    }

    string GetPrompt()
    {
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

        //Reset all client mats and pulsate tweens
        for (int i = 0; i < ClientPlayer.clients.Count; i++)
        {
            clientPulsateTweens[i].Kill();
            ClientPlayer.clients[i].smr.material.SetColor("_OutlineColor", ClientPlayer.clients[i].outlineColor);
        }
        clientPulsateTweens = new List<Tween>();
        numOfPointersOnClients = new int[ClientPlayer.clients.Count];

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
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            VRtistryClientPlayer vcp = (cp as VRtistryClientPlayer);
            vcp.SetButtonInteractable(false);
            vcp.playerButton.onClick.RemoveAllListeners();
        }

        yield return new WaitForSeconds(3);

        if (!VRtistrySyncer.instance.State.Equals("results"))
        {
            headerText.text = "Waiting for clients to submit guesses";

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
        StartCoroutine(GuessPlayerVR(1));
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

        if(g.gameObject.name.Equals("PaintBrush(Clone)"))
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
        else if(g.gameObject.name.Equals("PaintPalette(Clone)"))
        {
            if(VRtistrySyncer.instance.IsBrushEnabled)
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

        if(paintPalette.currentMeshLeft != isPaletteHandLeft)
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
        //Start at index 2 to avoid armature parent + hips
        for (int i = 2; i < armatureRTs.Length; i++)
        {
            armatureRTs[i].transform.position = armaturePositions[i];
            armatureRTs[i].transform.rotation = armatureRotations[i];
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}