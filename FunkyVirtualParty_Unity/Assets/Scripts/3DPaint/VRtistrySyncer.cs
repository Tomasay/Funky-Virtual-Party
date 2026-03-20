using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Normal.Realtime;
using Normal.Realtime.Serialization;

public class VRtistrySyncer : RealtimeComponent<VRtistrySyncModel>
{
    public static VRtistrySyncer instance;

    public MyStringEvent OnStateChangeEvent, OnPromptChangedEvent, OnDecoyAnswersChanged;

    public UnityEvent StartedPainting, StoppedPainting, StartedDrawing, StoppedDrawing, PaletteMirrored;
    public UnityEvent<bool> brushEnabledChanged, paletteEnabledChanged;
    public UnityEvent<Color> brushColorChanged;
    public UnityEvent<int> chosenClientToRoastChanged;

    public string State { get => model.state; set => model.state = value; }
    public string DecoyAnswers { get => model.decoyAnswers; set => model.decoyAnswers = value; }
    public string CurrentPrompt { get => model.currentPrompt; set => model.currentPrompt = value; }
    public int VRPlayerPoints { get => model.vrPlayerPoints; set => model.vrPlayerPoints = value; }
    public int ChosenAnswerOwner { get => model.chosenAnswerOwner; set => model.chosenAnswerOwner = value; }
    public int VRPlayerGuess { get => model.vrPlayerGuess; set => model.vrPlayerGuess = value; }
    public float ClientAnswerTimer { get => model.clientAnswerTimer; set => model.clientAnswerTimer = value; }
    public float DrawingTimer { get => model.drawingTimer; set => model.drawingTimer = value; }
    public bool VRCompletedTutorial { get => model.vrCompletedTutorial; set => model.vrCompletedTutorial = value; }
    public bool IsPainting { get => model.isPainting; set => model.isPainting = value; }
    public bool IsDrawing { get => model.isDrawing; set => model.isDrawing = value; }
    public bool IsPaletteMirrored { get => model.isPaletteMirrored; set => model.isPaletteMirrored = value; }
    public bool IsBrushEnabled { get => model.isBrushEnabled; set => model.isBrushEnabled = value; }
    public bool IsPaletteEnabled { get => model.isPaletteEnabled; set => model.isPaletteEnabled = value; }
    public int ChosenClientToRoast { get => model.chosenClientToRoast; set => model.chosenClientToRoast = value; }

    public Color BrushColor { get => model.brushColor; set => model.brushColor = value; }

    private bool isWeb;

    private void Awake()
    {
        //Singleton
        instance = this;

        if (OnStateChangeEvent == null)
            OnStateChangeEvent = new MyStringEvent();

        if (OnPromptChangedEvent == null)
            OnPromptChangedEvent = new MyStringEvent();

        StartedPainting = new UnityEvent();
        StoppedPainting = new UnityEvent();
        StartedDrawing = new UnityEvent();
        StoppedDrawing = new UnityEvent();
        PaletteMirrored = new UnityEvent();

        brushEnabledChanged = new UnityEvent<bool>();
        paletteEnabledChanged = new UnityEvent<bool>();
        chosenClientToRoastChanged = new UnityEvent<int>();

        brushColorChanged = new UnityEvent<Color>();

#if UNITY_WEBGL
        isWeb = true;
#endif
    }

#if UNITY_ANDROID || UNITY_STANDALONE_WIN //Only host has to worry about triggering allPlayersReady event
    private void Start()
    {
        //Default states when entering scene
        State = "";
        DecoyAnswers = "";
        CurrentPrompt = "";
        VRPlayerGuess = -1;
        VRPlayerPoints = 0;
        DrawingTimer = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;
        ClientAnswerTimer = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;
        VRCompletedTutorial = false;
        IsPainting = false;
        IsDrawing = false;
        IsPaletteMirrored = false;
        IsPaletteEnabled = false;
        BrushColor = Color.black;
        IsBrushEnabled = false;

        //TutorialMenu.instance.allPlayersReady.AddListener(delegate { State = "countdown"; });
    }

    private void OnDestroy()
    {
        //TutorialMenu.instance.allPlayersReady.RemoveListener(delegate { State = "countdown"; });
    }
#endif

    protected override void OnRealtimeModelReplaced(VRtistrySyncModel previousModel, VRtistrySyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.stateDidChange -= OnStateChange;
            previousModel.decoyAnswersDidChange -= OnDecoyAnswersDidChange;
            previousModel.isPaintingDidChange -= OnIsPaintingDidChange;
            previousModel.isDrawingDidChange -= OnIsDrawingDidChange;
            previousModel.isBrushEnabledDidChange -= OnIsBrushEnabledChanged;
            previousModel.isPaletteMirroredDidChange -= OnIsPaletteMirroredChanged;
            previousModel.isPaletteEnabledDidChange -= OnIsPaletteEnabledChanged;
            previousModel.brushColorDidChange -= OnBrushColorChanged;
            previousModel.currentPromptDidChange -= OnPromptChanged;
            previousModel.chosenAnswerOwnerDidChange -= OnChosenAnswerOwnerChanged;
            previousModel.chosenClientToRoastDidChange -= OnChosenClientToRoastChanged;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                //TODO: Changed for standalone
                currentModel.state = "main menu";
            }

            // Register for events
            currentModel.stateDidChange += OnStateChange;
            currentModel.decoyAnswersDidChange += OnDecoyAnswersDidChange;
            currentModel.isPaintingDidChange += OnIsPaintingDidChange;
            currentModel.isDrawingDidChange += OnIsDrawingDidChange;
            currentModel.isBrushEnabledDidChange += OnIsBrushEnabledChanged;
            currentModel.isPaletteMirroredDidChange += OnIsPaletteMirroredChanged;
            currentModel.isPaletteEnabledDidChange += OnIsPaletteEnabledChanged;
            currentModel.brushColorDidChange += OnBrushColorChanged;
            currentModel.currentPromptDidChange += OnPromptChanged;
            currentModel.chosenAnswerOwnerDidChange += OnChosenAnswerOwnerChanged;
            currentModel.chosenClientToRoastDidChange += OnChosenClientToRoastChanged;
        }
    }

    #region Variable Callbacks
    void OnStateChange(VRtistrySyncModel previousModel, string val)
    {
        OnStateChangeEvent.Invoke(val);
    }

    void OnDecoyAnswersDidChange(VRtistrySyncModel previousModel, string val)
    {
        OnDecoyAnswersChanged.Invoke(val);
    }

    private void OnIsPaintingDidChange(VRtistrySyncModel model, bool value)
    {
        if(value)
        {
            StartedPainting.Invoke();
        }
        else
        {
            StoppedPainting.Invoke();
        }
    }

    private void OnIsDrawingDidChange(VRtistrySyncModel model, bool value)
    {
        if (value)
        {
            StartedDrawing.Invoke();
        }
        else
        {
            StoppedDrawing.Invoke();
        }
    }

    private void OnIsBrushEnabledChanged(VRtistrySyncModel model, bool value)
    {
        brushEnabledChanged.Invoke(value);
    }

    private void OnIsPaletteMirroredChanged(VRtistrySyncModel model, bool value)
    {
        PaletteMirrored.Invoke();
    }

    private void OnIsPaletteEnabledChanged(VRtistrySyncModel model, bool value)
    {
        paletteEnabledChanged.Invoke(value);
    }

    private void OnBrushColorChanged(VRtistrySyncModel model, Color value)
    {
        brushColorChanged.Invoke(value);
    }

    private void OnPromptChanged(VRtistrySyncModel model, string value)
    {
        OnPromptChangedEvent.Invoke(value);
    }

    private void OnChosenAnswerOwnerChanged(VRtistrySyncModel model, int value)
    {
#if UNITY_WEBGL
        ClientPlayer cp = RealtimeSingletonWeb.instance.LocalPlayer;
        if (cp.realtimeView.ownerIDSelf == value)
        {
            cp.syncer.Score += ThreeDPaintGlobalVariables.POINTS_CLIENT_SELECTED_PROMPT;
        }
#endif
    }

    private void OnChosenClientToRoastChanged(VRtistrySyncModel model, int value)
    {
        chosenClientToRoastChanged.Invoke(value);
    }
    #endregion
}