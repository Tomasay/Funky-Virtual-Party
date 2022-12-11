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

public enum ThreeDPaintGameState
{
    ClientsAnswering,
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
    XRControllerEvent YButtonEvent, BButtonEvent;

    [SerializeField]
    AutoHandPlayer ahp;

    Dictionary<string, string> answers;

    private string chosenAnswer, chosenAnswerOwner;

    private int playersGuessed = 0;

    float drawTimeRemaining;
    const int DRAW_TIME_AMOUNT = 120;

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

        drawTimeRemaining = DRAW_TIME_AMOUNT;

        if (ClientManager.instance)
        {
            foreach (ClientPlayer cp in ClientManager.instance.Players)
            {
                playerPoints.Add(cp.PlayerID, 0);
            }

            ClientManager.instance.Manager.Socket.On<string, string>("InfoToXR", InfoReceived);

            StartCoroutine("StartGame");
        }
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

    public void OnTutorialCompleted()
    {
        vrTutorialCompleted = true;

        if (ClientManager.instance)
        {
            if (answers.Count >= ClientManager.instance.Players.Count)
            {
                State = ThreeDPaintGameState.VRPainting;
            }

            ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "VRDoneWithTutorial", "");
        }

        headerText.enabled = true;
    }

    void OnStateChange()
    {
        switch (state)
        {
            case ThreeDPaintGameState.ClientsAnswering:
                //Display text that players are answering
                headerText.text = "Players are typing their answers \nUse this time to practice painting! \nTurn around for controls";

                //Animate client players to look like they are on their phones

                break;
            case ThreeDPaintGameState.VRPainting:
                timerText.enabled = true;

                //Enable VR tools
                pen.canPaint = true;
                sprayGun.canPaint = true;

                //Display answer
                headerText.text = "Paint: " + chosenAnswer;

                //Show blurred view to clients
                ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ShowBlurredView", "");

                //Clear practice painting
                pen.EraseAllLines();
                paintTexture.Clear();

                break;
            case ThreeDPaintGameState.ClientsGuessing:
                playersGuessed = 0;

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

        yield return new WaitForSeconds(5);

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
            newCard.GetComponentsInChildren<TMP_Text>()[0].text = ClientManager.instance.GetPlayerByID(entry.Key).PlayerName;
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
        leaderboardParent.SetActive(false);

        if(currentRound == ThreeDPaintGlobalVariables.NUMBER_OF_ROUNDS)
        {
            State = ThreeDPaintGameState.GameOver;
        }
        else
        {
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "SetNewPrompt", GetPrompt());
            State = ThreeDPaintGameState.ClientsAnswering;
            drawTimeRemaining = DRAW_TIME_AMOUNT;
            answers = new Dictionary<string, string>();

            currentRound++;
        }
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

                if (vrTutorialCompleted)
                {
                    State = ThreeDPaintGameState.VRPainting;
                }
            }
        }
        else if (State == ThreeDPaintGameState.ClientsGuessing)
        {
            //If player guessed right
            if (answers[info].Equals(answers[chosenAnswerOwner]))
            {
                if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "GuessedRight", playerID);
                AddPlayerToResults(playerID, true);

                vrPlayerPoints += ThreeDPaintGlobalVariables.POINTS_VR_CORRECT_GUESSES;
                playerPoints[playerID] += ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS;
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
                ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "VRGuessing", "");
            }
        }
    }

    string GetPrompt()
    {
        string[] prompts = promptList.ToString().Split('\n');
        currentPrompt = prompts[Random.Range(0, prompts.Length)];
        return currentPrompt;
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
            vrPlayerPoints += ThreeDPaintGlobalVariables.POINTS_VR_CORRECT_PLAYER;
            headerText.text = "Correct!";
        }
        else
        {
            headerText.text = "Wrong! " + ClientManager.instance.GetPlayerByID(chosenAnswerOwner).PlayerName + " wrote the answer";
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

    //Enable/Disable colliders that could get in the way of grabbing
    void SetColliders(bool active)
    {
        ahp.HeadPhysicsFollower.headCollider.enabled = active;
        ahp.capsuleColl.enabled = active;
    }
}