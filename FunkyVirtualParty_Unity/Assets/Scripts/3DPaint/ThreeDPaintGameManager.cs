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
using FMOD.Studio;
using FMODUnity;

public enum ThreeDPaintGameState
{
    ClientsAnswering,
    VRPosing,
    VRPainting,
    ClientsGuessing,
    VRGuessing,
    ShowingLeaderboard,
    GameOver
}

public class ThreeDPaintGameManager : GameManager
{
    private ThreeDPaintGameState state;
    public ThreeDPaintGameState State { get => state; set { state = value; ClientManager.instance.Manager.Socket.Emit("gameStateFromHost", "" + state); OnStateChange(); } }

    [SerializeField]
    TextAsset promptList;
    string currentPrompt;

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
    Hand leftHand, rightHand;
    [SerializeField]
    Transform leftHandGrabPoint, rightHandGrabPoint;

    [SerializeField]
    GameObject UIPointer;

    [SerializeField]
    GameObject leaderboardParent, leaderboardPlayerCardPrefab;
    List<GameObject> currentLeaderboardCards;

    [SerializeField]
    GameObject playerNameIconPrefab, playerNamesIconParent;
    List<GameObject> playerNameIcons;

    [SerializeField]
    XRControllerEvent YButtonEvent, BButtonEvent, AButtonEvent, XButtonEvent, RightTriggerEvent, LeftTriggerEvent, RightGripEvent, LeftGripEvent;

    [SerializeField]
    AutoHandPlayer ahp;

    [SerializeField]
    Button finishedPaintingEarlyButton;

    [SerializeField]
    MannequinSolver solver;

    [SerializeField]
    EventReference musicEvent;

    EventInstance fmodInstance;

    Dictionary<string, string> answers;

    private string chosenAnswer, chosenAnswerOwner;

    private int playersGuessed = 0;

    float drawTimeRemaining, clientAnswerTimeRemaining;

    float vrPlayerPoints;
    Dictionary<string, int> playerPoints;

    int currentRound = 1;

    HandType toolHand = HandType.right;

    bool vrTutorialCompleted;

    bool needToGrabPalette;

    private void Awake()
    {
        answers = new Dictionary<string, string>();
        playerPoints = new Dictionary<string, int>();

        playerNameIcons = new List<GameObject>();
        currentLeaderboardCards = new List<GameObject>();

        headerText.text = "";
        timerText.text = "";
        headerText.enabled = false;

        finishedPaintingEarlyButton.gameObject.SetActive(false);

        drawTimeRemaining = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;
        clientAnswerTimeRemaining = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;

        if (ClientManager.instance)
        {
            foreach (ClientPlayer cp in ClientManager.instance.Players)
            {
                playerPoints.Add(cp.PlayerSocketID, 0);
            }

            ClientManager.instance.Manager.Socket.On<string, string>("InfoToXR", InfoReceived);

            StartCoroutine("StartGame");
        }

        fmodInstance = RuntimeManager.CreateInstance(musicEvent);
        fmodInstance.start();
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("InfoToXR");
#endif
    }

    public void GrabToolsStart()
    {
        GrabTool(true);
        GrabPalette();

        YButtonEvent.Pressed.AddListener(ToggleToolHand);
        BButtonEvent.Pressed.AddListener(ToggleTool);
    }

    private void Update()
    {
        switch (state)
        {
            case ThreeDPaintGameState.ClientsAnswering:
                clientAnswerTimeRemaining -= Time.deltaTime;

                if (clientAnswerTimeRemaining <= 0)
                {
                    State = ThreeDPaintGameState.VRPosing;
                }
                break;
            case ThreeDPaintGameState.VRPainting:
                //Draw timer
                drawTimeRemaining -= Time.deltaTime;
                timerText.text = FormatTime(drawTimeRemaining);

                if (drawTimeRemaining <= 15)
                {
                    fmodInstance.setParameterByName("VRtistryClock", 2);
                }
                else if (drawTimeRemaining <= ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT / 2)
                {
                    fmodInstance.setParameterByName("VRtistryClock", 1);
                }

                if (drawTimeRemaining <= 0)
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

    public void OnTutorialCompleted()
    {
        vrTutorialCompleted = true;

        if (ClientManager.instance)
        {
            if (answers.Count >= ClientManager.instance.Players.Count)
            {
                State = ThreeDPaintGameState.VRPosing;
            }

            ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "VRDoneWithTutorial", "");
        }

        headerText.enabled = true;
    }

    public void FinishedPaintingEarly()
    {
        State = ThreeDPaintGameState.ClientsGuessing;
    }

    void SetPose()
    {
        solver.SetPose();
        GrabToolsStart();
        State = ThreeDPaintGameState.VRPainting;
    }

    void OnStateChange()
    {
        switch (state)
        {
            case ThreeDPaintGameState.ClientsAnswering:
                clientAnswerTimeRemaining = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;

                //Enable VR tools
                pen.CanPaint = true;
                sprayGun.CanPaint = true;

                //Display text that players are answering
                headerText.text = "Players are typing their answers \nUse this time to practice painting!";

                //Animate client players to look like they are on their phones

                break;
            case ThreeDPaintGameState.VRPosing:
                fmodInstance.setParameterByName("VRtistryPhase", 1);

                solver.EnablePosing();

                //Disable VR tools
                pen.CanPaint = false;
                sprayGun.CanPaint = false;
                DropTool();
                DropPalette();

                //Get random answer
                string randomPlayer = answers.ElementAt(Random.Range(0, answers.Count)).Key;
                chosenAnswer = answers[randomPlayer];
                chosenAnswerOwner = randomPlayer;
                if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ChosenAnswerOwner", chosenAnswerOwner);

                //UI
                headerText.text = "<u>Paint: " + chosenAnswer + "</u>\nStart by posing your creation! Press any button on your controllers to lock in your pose";

                //Clear practice painting
                pen.EraseAllLines();
                paintTexture.Clear();

                //Inputs for setting pose
                YButtonEvent.Pressed.RemoveAllListeners();
                BButtonEvent.Pressed.RemoveAllListeners();

                YButtonEvent.Pressed.AddListener(SetPose);
                BButtonEvent.Pressed.AddListener(SetPose);
                AButtonEvent.Pressed.AddListener(SetPose);
                XButtonEvent.Pressed.AddListener(SetPose);
                RightTriggerEvent.Pressed.AddListener(SetPose);
                LeftTriggerEvent.Pressed.AddListener(SetPose);
                RightGripEvent.Pressed.AddListener(SetPose);
                LeftGripEvent.Pressed.AddListener(SetPose);

                break;
            case ThreeDPaintGameState.VRPainting:

                //Inputs
                YButtonEvent.Pressed.RemoveListener(SetPose);
                BButtonEvent.Pressed.RemoveListener(SetPose);
                AButtonEvent.Pressed.RemoveListener(SetPose);
                XButtonEvent.Pressed.RemoveListener(SetPose);
                RightTriggerEvent.Pressed.RemoveListener(SetPose);
                LeftTriggerEvent.Pressed.RemoveListener(SetPose);
                RightGripEvent.Pressed.RemoveListener(SetPose);
                LeftGripEvent.Pressed.RemoveListener(SetPose);

                //Enable VR tools
                pen.CanPaint = true;
                sprayGun.CanPaint = true;

                //UI
                headerText.text = "Paint: " + chosenAnswer;
                timerText.enabled = true;
                finishedPaintingEarlyButton.gameObject.SetActive(true);
                

                //Show blurred view to clients
                ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ShowBlurredView", "");
                break;
            case ThreeDPaintGameState.ClientsGuessing:
                fmodInstance.setParameterByName("VRtistryPhase", 0);
                fmodInstance.setParameterByName("VRtistryClock", 0);

                playersGuessed = 0;

                //Communicate to clients
                ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "DoneDrawing", "");

                //Disable VR tools
                pen.CanPaint = false;
                sprayGun.CanPaint = false;

                //Display all answers
                headerText.text = "Clients are guessing what your art is";
                timerText.enabled = false;
                playerNamesIconParent.SetActive(true);

                finishedPaintingEarlyButton.gameObject.SetActive(false);
                break;
            case ThreeDPaintGameState.VRGuessing:
                headerText.text = "The prompt was: " + currentPrompt + "\nWhich player do you think wrote the given answer?";
                foreach (GameObject g in playerNameIcons)
                {
                    g.GetComponentInChildren<Button>().interactable = true;
                }
                UIPointer.SetActive(true);
                leftHand.Release();
                rightHand.Release();

                break;
            case ThreeDPaintGameState.ShowingLeaderboard:
                StartCoroutine("ShowLeaderboard");
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

        fmodInstance.setParameterByName("VRtistryPhase", 2);

        yield return new WaitForSeconds(4);

        fmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        yield return new WaitForSeconds(1);

        ClientManager.instance.LoadMainMenu();
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
        IOrderedEnumerable<KeyValuePair<string, int>> sortedDict = from entry in playerPoints orderby entry.Value descending select entry;

        int vrPlayerPos = 0;
        //Add player cards
        foreach (KeyValuePair<string, int> entry in sortedDict)
        {
            GameObject newCard = Instantiate(leaderboardPlayerCardPrefab, leaderboardParent.transform);
            newCard.GetComponentsInChildren<TMP_Text>()[0].text = ClientManager.instance.GetPlayerBySocketID(entry.Key).PlayerName;
            newCard.GetComponentsInChildren<TMP_Text>()[1].text = answers.ContainsKey(entry.Key) ? answers[entry.Key] : ""; //Leave blank if player didn't provide answer
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
        leaderboardParent.SetActive(false);

        if(currentRound == ThreeDPaintGlobalVariables.NUMBER_OF_ROUNDS)
        {
            State = ThreeDPaintGameState.GameOver;
        }
        else
        {
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "SetNewPrompt", GetPrompt());
            State = ThreeDPaintGameState.ClientsAnswering;
            drawTimeRemaining = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;
            answers = new Dictionary<string, string>();

            currentRound++;
        }
    }

    void InfoReceived(string info, string playerID)
    {
        if (State == ThreeDPaintGameState.ClientsAnswering)
        {
            answers.Add(playerID, info);

            if (answers.Count >= ClientManager.instance.Players.Count && vrTutorialCompleted)
            {
                State = ThreeDPaintGameState.VRPosing;
            }
        }
        else if (State == ThreeDPaintGameState.ClientsGuessing)
        {
            if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PlayerGuessed", playerID + ":" + info);

            //If player guessed right
            if (answers[info].Equals(answers[chosenAnswerOwner]))
            {
                AddPlayerToResults(playerID, true);

                vrPlayerPoints += ThreeDPaintGlobalVariables.POINTS_VR_CORRECT_GUESSES;
                playerPoints[playerID] += ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS;
            }
            else
            {
                AddPlayerToResults(playerID, false);
            }

            playersGuessed++;
            if(playersGuessed >= ClientManager.instance.Players.Count)
            {
                State = ThreeDPaintGameState.VRGuessing;
                ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "VRGuessing", "");
            }
        }
    }

    string GetPrompt()
    {
        //Split prompt text file into individual lines
        string[] prompts = promptList.ToString().Split('\n');

        //Get prompt at random, excluding comments
        do
        {
            currentPrompt = prompts[Random.Range(0, prompts.Length)];
        }
        while (currentPrompt.StartsWith("//"));

        return currentPrompt;
    }

    void AddPlayerToResults(string playerID, bool correct)
    {
        GameObject pi = Instantiate(playerNameIconPrefab, playerNamesIconParent.transform);
        pi.GetComponentInChildren<TMP_Text>(true).text = ClientManager.instance.GetPlayerBySocketID(playerID).PlayerName;
        pi.GetComponentInChildren<Button>(true).onClick.AddListener(delegate { GuessPlayerVR(playerID); });
        pi.GetComponentInChildren<Button>(true).interactable = false;
        //pi.GetComponent<Image>().color = correct ? Color.green : Color.red;
        pi.GetComponent<Image>().color = ClientManager.instance.GetPlayerBySocketID(playerID).PlayerColor;

        playerNameIcons.Add(pi);
    }

    /// <summary>
    /// Method called when VR player guesses which player wrote the selected prompt
    /// </summary>
    public void GuessPlayerVR(string playerID)
    {
        if(playerID.Equals(chosenAnswerOwner))
        {
            vrPlayerPoints += ThreeDPaintGlobalVariables.POINTS_VR_CORRECT_PLAYER;
            headerText.text = "Correct!";
        }
        else
        {
            headerText.text = "Wrong! " + ClientManager.instance.GetPlayerBySocketID(chosenAnswerOwner).PlayerName + " wrote the answer";
        }

        State = ThreeDPaintGameState.ShowingLeaderboard;

        ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "VRPlayerGuessed", ("" + vrPlayerPoints));
    }

    void GrabTool(bool isSprayGun)
    {
        SetColliders(false);

        bool isToolHandLeft = (toolHand == HandType.left);

        //Drop current tool
        if (isToolHandLeft)
        {
            leftHand.ForceReleaseGrab();
        }
        else
        {
            rightHand.ForceReleaseGrab();
        }

        //Enable proper tool
        if (isSprayGun)
        {
            sprayGun.SetActive(true);
            pen.SetActive(false);
            //sprayGun.transform.position = isToolHandLeft ? leftHandGrabPoint.transform.position : rightHandGrabPoint.transform.position;
        }
        else
        {
            sprayGun.SetActive(false);
            pen.SetActive(true);
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
            leftHand.Grab(GrabType.InstantGrab);
        }
        else
        {
            rightHand.Grab(GrabType.InstantGrab);
        }
    }

    void DropTool()
    {
        bool isToolHandLeft = (toolHand == HandType.left);

        //Drop current tool
        if (isToolHandLeft)
        {
            leftHand.ForceReleaseGrab();
        }
        else
        {
            rightHand.ForceReleaseGrab();
        }

        sprayGun.SetActive(false);
        pen.SetActive(false);
    }

    IEnumerator GrabDelayed(bool isToolHandLeft, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isToolHandLeft)
        {
            leftHand.Grab(GrabType.InstantGrab);
        }
        else
        {
            rightHand.Grab(GrabType.InstantGrab);
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
            src.sourceTransform = (toolHand == HandType.left) ? leftHandGrabPoint : rightHandGrabPoint;
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
        leftHand.ForceReleaseGrab();
        rightHand.ForceReleaseGrab();

        toolHand = (toolHand == HandType.left) ? HandType.right : HandType.left;

        pen.GetComponent<Grabbable>().handType = toolHand;
        sprayGun.GetComponent<Grabbable>().handType = toolHand;
        paintPalette.GetComponent<Grabbable>().handType = (toolHand == HandType.left) ? HandType.right : HandType.left;

        ParentConstraint pc = (sprayGun.active) ? sprayGun.GetComponent<ParentConstraint>() : pen.GetComponent<ParentConstraint>();
        pc.RemoveSource(0);
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = (toolHand == HandType.left) ? leftHandGrabPoint : rightHandGrabPoint;
        src.weight = 1;
        pc.AddSource(src);
        pc.constraintActive = true;

        //GrabTool(sprayGun.active);
        StartCoroutine(GrabDelayed(toolHand == HandType.left, 0.1f));
        needToGrabPalette = true;

        if (toolHand == HandType.left)
        {
            //Remove previous listener
            YButtonEvent.Pressed.RemoveListener(ToggleToolHand);
            BButtonEvent.Pressed.RemoveListener(ToggleTool);

            //Apply to opposite hands
            YButtonEvent.Pressed.AddListener(ToggleTool);
            BButtonEvent.Pressed.AddListener(ToggleToolHand);
        }
        else
        {
            //Remove previous listener
            YButtonEvent.Pressed.RemoveListener(ToggleTool);
            BButtonEvent.Pressed.RemoveListener(ToggleToolHand);

            //Apply to opposite hands
            YButtonEvent.Pressed.AddListener(ToggleToolHand);
            BButtonEvent.Pressed.AddListener(ToggleTool);
        }
    }

    void GrabPalette()
    {
        SetColliders(false);

        foreach (MeshRenderer mr in paintPalette.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = true;
        }
        
        bool isPaletteHandLeft = !(toolHand == HandType.left);

        PaintPalette pp = paintPalette.GetComponent<PaintPalette>();
        if(pp.currentMeshLeft != isPaletteHandLeft)
        {
            pp.Mirror();
        }

        ParentConstraint pc = paintPalette.GetComponent<ParentConstraint>();

        pc.RemoveSource(0);
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = isPaletteHandLeft ? leftHandGrabPoint : rightHandGrabPoint;
        src.weight = 1;
        pc.AddSource(src);
        pc.constraintActive = true;

        //Drop whatever's in hand
        if (isPaletteHandLeft)
        {
            leftHand.ForceReleaseGrab();
        }
        else
        {
            rightHand.ForceReleaseGrab();
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
            leftHand.Grab(GrabType.InstantGrab);
        }
        else
        {
            rightHand.Grab(GrabType.InstantGrab);
        }

        paintPalette.GetComponent<ParentConstraint>().constraintActive = false;

        needToGrabPalette = false;
    }

    void DropPalette()
    {
        bool isPaletteHandLeft = !(toolHand == HandType.left);

        if (isPaletteHandLeft)
        {
            leftHand.ForceReleaseGrab();
        }
        else
        {
            rightHand.ForceReleaseGrab();
        }

        foreach (MeshRenderer mr in paintPalette.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }

        paintPalette.GetComponent<MeshCollider>().enabled = false;
    }

    //Enable/Disable colliders that could get in the way of grabbing
    void SetColliders(bool active)
    {
        ahp.HeadPhysicsFollower.headCollider.enabled = active;
        ahp.capsuleColl.enabled = active;
    }
}