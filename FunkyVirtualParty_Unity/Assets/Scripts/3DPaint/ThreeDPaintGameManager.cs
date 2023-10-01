using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaintIn3D;
using Autohand;
using Autohand.Demo;
using System.Linq;
using UnityEngine.Animations;
#if !UNITY_WEBGL
using FMOD.Studio;
using FMODUnity;
#endif
using Normal.Realtime;

public class ThreeDPaintGameManager : MonoBehaviour
{
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
    GameObject paintPalette;

    [SerializeField]
    GameObject UIPointer;

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

#if !UNITY_WEBGL
    [SerializeField]
    EventReference musicEvent;

    EventInstance fmodInstance;
#endif

    Dictionary<int, int> playerPoints;

    int currentRound = 1;

    HandType toolHand = HandType.right;

    bool needToGrabPalette;

    private VRtistryVRPlayerController vrPlayer;

    private void Awake()
    {
        playerPoints = new Dictionary<int, int>();

        playerNameIcons = new List<GameObject>();
        currentLeaderboardCards = new List<GameObject>();

        headerText.text = "";
        timerText.text = "";
        headerText.enabled = false;

        finishedPaintingEarlyButton.gameObject.SetActive(false);

        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            playerPoints.Add(cp.realtimeView.ownerIDSelf, 0);
        }

        StartCoroutine("StartGame");

#if !UNITY_WEBGL
        fmodInstance = RuntimeManager.CreateInstance(musicEvent);
        fmodInstance.start();
#endif
    }

    private void Start()
    {
        VRtistrySyncer.instance.OnStateChangeEvent.AddListener(OnStateChanged);
        VRtistrySyncer.instance.OnPlayerAnswered.AddListener(PlayerAnswered);
        VRtistrySyncer.instance.OnPlayerGuessed.AddListener(PlayerGuessed);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;

        InvokeRepeating("Test", 1, 1);
    }

    //Temporary solution because ownership seems to randomly get taken by VR player, preventing clients from changing any values
    void Test()
    {
        VRtistrySyncer.instance.realtimeView.ClearOwnership();
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChanged);
        VRtistrySyncer.instance.OnPlayerAnswered.RemoveListener(PlayerAnswered);
        VRtistrySyncer.instance.OnPlayerGuessed.RemoveListener(PlayerGuessed);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        vrPlayer = avatar.GetComponent<VRtistryVRPlayerController>();

        vrPlayer.leftHand.GetComponent<HandPublicEvents>().OnGrab.AddListener(OnGrabbed);
        vrPlayer.rightHand.GetComponent<HandPublicEvents>().OnGrab.AddListener(OnGrabbed);

        vrPlayer.leftHand.GetComponent<HandAdvancedOptions>().ignoreHandCollider.Add(mannequinUVs.GetComponent<MeshCollider>());
        vrPlayer.rightHand.GetComponent<HandAdvancedOptions>().ignoreHandCollider.Add(mannequinUVs.GetComponent<MeshCollider>());
    }

    public void GrabToolsStart()
    {
        GrabTool(true);
        GrabPalette();

        vrPlayer.YButtonEvent.Pressed.AddListener(ToggleToolHand);
        vrPlayer.BButtonEvent.Pressed.AddListener(ToggleTool);
    }

    private void Update()
    {
        switch (VRtistrySyncer.instance.State)
        {
            case "clients answering":
                VRtistrySyncer.instance.ClientAnswerTimer -= Time.deltaTime;

                if (VRtistrySyncer.instance.ClientAnswerTimer <= 0 && VRtistrySyncer.instance.VRCompletedTutorial)
                {
                    VRtistrySyncer.instance.State = "vr posing";
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
    }

    public void OnTutorialCompleted()
    {
        VRtistrySyncer.instance.VRCompletedTutorial = true;

        if (!VRtistrySyncer.instance.Answers.Equals(""))
        {
            string[] answersSeparated = VRtistrySyncer.instance.Answers.Split('\n');

            if (answersSeparated.Length >= ClientPlayer.clients.Count)
            {
                VRtistrySyncer.instance.State = "vr posing";
            }
        }

        headerText.enabled = true;
    }

    public void FinishedPaintingEarly()
    {
        VRtistrySyncer.instance.State = "clients guessing";
    }

    void SetPose()
    {
        solver.SetPose();
        GrabToolsStart();
        VRtistrySyncer.instance.State = "vr painting";
    }

    void OnStateChanged(string state)
    {
        switch (state)
        {
            case "clients answering":
                VRtistrySyncer.instance.ClientAnswerTimer = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;

                //Enable VR tools
                pen.CanPaint = true;
                sprayGun.CanPaint = true;

                //Display text that players are answering
                headerText.text = "Players are typing their answers \nUse this time to practice painting!";

                //Animate client players to look like they are on their phones

                break;
            case "vr posing":
#if !UNITY_WEBGL
                fmodInstance.setParameterByName("VRtistryPhase", 1);
#endif

                solver.EnablePosing();

                //Disable VR tools
                pen.CanPaint = false;
                sprayGun.CanPaint = false;
                DropTool();
                DropPalette();

                //Get random answer
                VRtistrySyncer.instance.ChosenAnswerOwner = ClientPlayer.clients[Random.Range(0, ClientPlayer.clients.Count)].realtimeView.ownerIDSelf;

                //UI
                headerText.text = "<u>Paint: " + GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner) + "</u>\nStart by posing your creation! Press any button on your controllers to lock in your pose";

                //Clear practice painting
                pen.EraseAllLines();
                paintTexture.Clear();

                //Inputs for setting pose
                vrPlayer.YButtonEvent.Pressed.RemoveAllListeners();
                vrPlayer.BButtonEvent.Pressed.RemoveAllListeners();

                //Give a 2 second buffer for players to realize what's happening so they don't accidentally press a button too soon
                Invoke("EnablePoseCallbacks", 2);

                break;
            case "vr painting":

                //Inputs
                vrPlayer.YButtonEvent.Pressed.RemoveListener(SetPose);
                vrPlayer.BButtonEvent.Pressed.RemoveListener(SetPose);
                vrPlayer.AButtonEvent.Pressed.RemoveListener(SetPose);
                vrPlayer.XButtonEvent.Pressed.RemoveListener(SetPose);
                vrPlayer.RightTriggerEvent.Pressed.RemoveListener(SetPose);
                vrPlayer.LeftTriggerEvent.Pressed.RemoveListener(SetPose);
                vrPlayer.RightGripEvent.Pressed.RemoveListener(SetPose);
                vrPlayer.LeftGripEvent.Pressed.RemoveListener(SetPose);

                //Enable VR tools
                pen.CanPaint = true;
                sprayGun.CanPaint = true;

                //UI
                headerText.text = "Paint: " + GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner);
                timerText.enabled = true;
                finishedPaintingEarlyButton.gameObject.SetActive(true);
                break;
            case "clients guessing":
#if !UNITY_WEBGL
                fmodInstance.setParameterByName("VRtistryPhase", 0);
                fmodInstance.setParameterByName("VRtistryClock", 0);
#endif

                VRtistrySyncer.instance.Guesses = "";

                //Disable VR tools
                pen.CanPaint = false;
                sprayGun.CanPaint = false;

                //Display all answers
                headerText.text = "Clients are guessing what your art is";
                timerText.enabled = false;
                playerNamesIconParent.SetActive(true);

                finishedPaintingEarlyButton.gameObject.SetActive(false);
                break;
            case "vr guessing":
                //Show guesses
                string[] guessesSeparated = VRtistrySyncer.instance.Guesses.Split('\n');
                foreach (string g in guessesSeparated)
                {
                    string[] ownerAndGuess = g.Split(':');

                    if (int.TryParse(ownerAndGuess[1], out int i) && GetAnswerByOwnerID(i).Equals(GetAnswerByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner)))
                    {
                        if (int.TryParse(ownerAndGuess[0], out int j))
                        {
                            AddPlayerToResults(j, true);

                            VRtistrySyncer.instance.VRPlayerPoints += ThreeDPaintGlobalVariables.POINTS_VR_CORRECT_GUESSES;

                            playerPoints[i] += ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS;
                        }
                    }
                    else
                    {
                        if (int.TryParse(ownerAndGuess[0], out int j))
                        {
                            AddPlayerToResults(j, false);
                        }
                    }
                }

                //Header
                headerText.text = "The prompt was: " + VRtistrySyncer.instance.CurrentPrompt + "\nWhich player do you think wrote the given answer?";
                foreach (GameObject g in playerNameIcons)
                {
                    g.GetComponentInChildren<Button>().interactable = true;
                }

                //UX
                UIPointer.SetActive(true);
                vrPlayer.leftHand.Release();
                vrPlayer.rightHand.Release();

                break;
            case "leaderboard":
                StartCoroutine("ShowLeaderboard");

                VRtistrySyncer.instance.Guesses = "";

                break;
            case "game over":
                StartCoroutine("EndGame");
                break;
            default:
                break;
        }
    }

    void EnablePoseCallbacks()
    {
        vrPlayer.YButtonEvent.Pressed.AddListener(SetPose);
        vrPlayer.BButtonEvent.Pressed.AddListener(SetPose);
        vrPlayer.AButtonEvent.Pressed.AddListener(SetPose);
        vrPlayer.XButtonEvent.Pressed.AddListener(SetPose);
        vrPlayer.RightTriggerEvent.Pressed.AddListener(SetPose);
        vrPlayer.LeftTriggerEvent.Pressed.AddListener(SetPose);
        vrPlayer.RightGripEvent.Pressed.AddListener(SetPose);
        vrPlayer.LeftGripEvent.Pressed.AddListener(SetPose);
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

        //Show leaderboard
        headerText.text = "";
        timerText.text = "";
        leaderboardParent.SetActive(true);
        //if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ShowLeaderboard", "");

        //Sort player points
        IOrderedEnumerable<KeyValuePair<int, int>> sortedDict = from entry in playerPoints orderby entry.Value descending select entry;

        int vrPlayerPos = 0;
        //Add player cards
        foreach (KeyValuePair<int, int> entry in sortedDict)
        {
            GameObject newCard = Instantiate(leaderboardPlayerCardPrefab, leaderboardParent.transform);
            newCard.GetComponentsInChildren<TMP_Text>()[0].text = ClientPlayer.GetClientByOwnerID(entry.Key).syncer.Name;
            newCard.GetComponentsInChildren<TMP_Text>()[1].text = GetAnswerByOwnerID(entry.Key);
            newCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + entry.Value;

            currentLeaderboardCards.Add(newCard);

            if (VRtistrySyncer.instance.VRPlayerPoints < entry.Value)
            {
                vrPlayerPos++;
            }
        }

        //Add VR Card
        GameObject vrCard = Instantiate(leaderboardPlayerCardPrefab, leaderboardParent.transform);
        vrCard.GetComponentsInChildren<TMP_Text>()[0].text = "VR Player";
        vrCard.GetComponentsInChildren<TMP_Text>()[1].text = "";
        vrCard.GetComponentsInChildren<TMP_Text>()[2].text = "" + VRtistrySyncer.instance.VRPlayerPoints;
        vrCard.transform.SetSiblingIndex(vrPlayerPos);

        currentLeaderboardCards.Add(vrCard);

        yield return new WaitForSeconds(5);

        //Disable leaderboard
        leaderboardParent.SetActive(false);

        if(currentRound == ThreeDPaintGlobalVariables.NUMBER_OF_ROUNDS)
        {
            VRtistrySyncer.instance.State = "game over";
        }
        else
        {
            VRtistrySyncer.instance.CurrentPrompt = GetPrompt();
            VRtistrySyncer.instance.State = "clients answering";
            VRtistrySyncer.instance.DrawingTimer = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;
            VRtistrySyncer.instance.Answers = "";

            currentRound++;
        }
    }

    void PlayerAnswered(string answers)
    {
        //Check to see if all players have answered, if so move to next state
        string[] answersSeparated = answers.Split('\n');

        if (answersSeparated.Length >= ClientPlayer.clients.Count && VRtistrySyncer.instance.VRCompletedTutorial)
        {
            VRtistrySyncer.instance.State = "vr posing";
        }
    }

    void PlayerGuessed(string guesses)
    {
        //Check to see if all players have guessed, if so move to next state
        string[] guessesSeparated = guesses.Split('\n');

        if (guessesSeparated.Length >= ClientPlayer.clients.Count && VRtistrySyncer.instance.State.Equals("clients guessing"))
        {
            VRtistrySyncer.instance.State = "vr guessing";
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
        pi.GetComponentInChildren<TMP_Text>(true).text = ClientPlayer.GetClientByOwnerID(playerID).syncer.Name;
        pi.GetComponentInChildren<Button>(true).onClick.AddListener(delegate { GuessPlayerVR(playerID); });
        pi.GetComponentInChildren<Button>(true).interactable = false;
        //pi.GetComponent<Image>().color = correct ? Color.green : Color.red;
        pi.GetComponent<Image>().color = ClientPlayer.GetClientByOwnerID(playerID).syncer.Color;

        playerNameIcons.Add(pi);
    }

    /// <summary>
    /// Method called when VR player guesses which player wrote the selected prompt
    /// </summary>
    public void GuessPlayerVR(int playerID)
    {
        VRtistrySyncer.instance.VRPlayerGuess = playerID;

        if (playerID.Equals(VRtistrySyncer.instance.ChosenAnswerOwner))
        {
            VRtistrySyncer.instance.VRPlayerPoints += ThreeDPaintGlobalVariables.POINTS_VR_CORRECT_PLAYER;
            headerText.text = "Correct!";
        }
        else
        {
            headerText.text = "Wrong! " + ClientPlayer.GetClientByOwnerID(VRtistrySyncer.instance.ChosenAnswerOwner).syncer.Name + " wrote the answer";
        }

        VRtistrySyncer.instance.State = "leaderboard";
    }

    void GrabTool(bool isSprayGun)
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

        //Enable proper tool
        if (isSprayGun)
        {
            VRtistrySyncer.instance.IsPenEnabled = false;
            //sprayGun.transform.position = isToolHandLeft ? leftHandGrabPoint.transform.position : rightHandGrabPoint.transform.position;
        }
        else
        {
            VRtistrySyncer.instance.IsPenEnabled = true;
            //pen.transform.position = isToolHandLeft ? leftHandGrabPoint.transform.position : rightHandGrabPoint.transform.position;
        }

        //Temporarily disable palette so it doesn't get in the way of grabbing
        foreach (Collider c in paintPalette.GetComponent<MeshCollider>().GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }

        //Grab proper tool
        if (isToolHandLeft)
        {
            vrPlayer.leftHand.Grab(GrabType.InstantGrab);
        }
        else
        {
            vrPlayer.rightHand.Grab(GrabType.InstantGrab);
        }
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

        sprayGun.SetActive(false);
        pen.SetActive(false);
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
        if(g.gameObject.name.Equals("3DPen") || g.gameObject.name.Equals("Paint Spray Gun"))
        {
            //Re-enable palette colliders
            foreach (Collider c in paintPalette.GetComponent<MeshCollider>().GetComponentsInChildren<Collider>())
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
            ParentConstraint pc;

            if (sprayGun.active)
            {
                sprayGun.GetComponent<ParentConstraint>().constraintActive = false;
                pc = pen.GetComponent<ParentConstraint>();
            }
            else
            {
                pen.GetComponent<ParentConstraint>().constraintActive = false;
                pc = sprayGun.GetComponent<ParentConstraint>();
            }

            pc.RemoveSource(0);
            ConstraintSource src = new ConstraintSource();
            src.sourceTransform = (toolHand == HandType.left) ? vrPlayer.leftHandGrabPoint : vrPlayer.rightHandGrabPoint;
            src.weight = 1;
            pc.AddSource(src);
            pc.constraintActive = true;
        }
        else if(g.gameObject.name.Equals("PaintPalette"))
        {
            if(sprayGun.active)
            {
                sprayGun.GetComponent<Collider>().enabled = true;
            }
            else
            {
                pen.GetComponent<Collider>().enabled = true;
            }

            SetColliders(true);
        }
    }

    public void ToggleTool()
    {
        GrabTool(!sprayGun.active);
    }

    public void ToggleToolHand()
    {
        vrPlayer.leftHand.ForceReleaseGrab();
        vrPlayer.rightHand.ForceReleaseGrab();

        toolHand = (toolHand == HandType.left) ? HandType.right : HandType.left;

        pen.GetComponent<Grabbable>().handType = toolHand;
        sprayGun.GetComponent<Grabbable>().handType = toolHand;
        paintPalette.GetComponent<Grabbable>().handType = (toolHand == HandType.left) ? HandType.right : HandType.left;

        ParentConstraint pc = (sprayGun.active) ? sprayGun.GetComponent<ParentConstraint>() : pen.GetComponent<ParentConstraint>();
        pc.RemoveSource(0);
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = (toolHand == HandType.left) ? vrPlayer.leftHandGrabPoint : vrPlayer.rightHandGrabPoint;
        src.weight = 1;
        pc.AddSource(src);
        pc.constraintActive = true;

        //GrabTool(sprayGun.active);
        StartCoroutine(GrabDelayed(toolHand == HandType.left, 0.1f));
        needToGrabPalette = true;

        if (toolHand == HandType.left)
        {
            //Remove previous listener
            vrPlayer.YButtonEvent.Pressed.RemoveListener(ToggleToolHand);
            vrPlayer.BButtonEvent.Pressed.RemoveListener(ToggleTool);

            //Apply to opposite hands
            vrPlayer.YButtonEvent.Pressed.AddListener(ToggleTool);
            vrPlayer.BButtonEvent.Pressed.AddListener(ToggleToolHand);
        }
        else
        {
            //Remove previous listener
            vrPlayer.YButtonEvent.Pressed.RemoveListener(ToggleTool);
            vrPlayer.BButtonEvent.Pressed.RemoveListener(ToggleToolHand);

            //Apply to opposite hands
            vrPlayer.YButtonEvent.Pressed.AddListener(ToggleToolHand);
            vrPlayer.BButtonEvent.Pressed.AddListener(ToggleTool);
        }
    }

    void GrabPalette()
    {
        SetColliders(false);

        VRtistrySyncer.instance.IsPaletteEnabled = true;

        bool isPaletteHandLeft = !(toolHand == HandType.left);

        PaintPalette pp = paintPalette.GetComponent<PaintPalette>();
        if(pp.currentMeshLeft != isPaletteHandLeft)
        {
            VRtistrySyncer.instance.IsPaletteMirrored = !VRtistrySyncer.instance.IsPaletteMirrored;
        }

        ParentConstraint pc = paintPalette.GetComponent<ParentConstraint>();

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

        if (sprayGun.active)
        {
            sprayGun.GetComponent<Collider>().enabled = false;
        }
        else
        {
            pen.GetComponent<Collider>().enabled = false;
        }

        paintPalette.GetComponent<MeshCollider>().enabled = true;

        //Grab palette
        if (isPaletteHandLeft)
        {
            vrPlayer.leftHand.Grab(GrabType.InstantGrab);
        }
        else
        {
            vrPlayer.rightHand.Grab(GrabType.InstantGrab);
        }

        paintPalette.GetComponent<ParentConstraint>().constraintActive = false;

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

        paintPalette.GetComponent<MeshCollider>().enabled = false;

        ParentConstraint pc = paintPalette.GetComponent<ParentConstraint>();

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

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}