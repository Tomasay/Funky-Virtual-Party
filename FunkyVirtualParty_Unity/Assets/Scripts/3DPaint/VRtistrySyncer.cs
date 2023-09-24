using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Normal.Realtime;

public class VRtistrySyncer : RealtimeComponent<VRtistrySyncModel>
{
    public static VRtistrySyncer instance;

    public MyStringEvent OnStateChangeEvent, OnPromptChangedEvent, OnPlayerAnswered, OnPlayerGuessed;

    public UnityEvent StartedPainting, StoppedPainting, StartedDrawing, StoppedDrawing, PaletteMirrored;
    public UnityEvent<bool> penEnabledChanged, paletteEnabledChanged;
    public UnityEvent<Color> penColorChanged, spraygunColorChanged;

    public string State { get => model.state; set => model.state = value; }
    public string Answers { get => model.answers; set => model.answers = value; }
    public string Guesses { get => model.guesses; set => model.guesses = value; }
    public string CurrentPrompt { get => model.currentPrompt; set => model.currentPrompt = value; }
    public int VRPlayerPoints { get => model.vrPlayerPoints; set => model.vrPlayerPoints = value; }
    public int ChosenAnswerOwner { get => model.chosenAnswerOwner; set => model.chosenAnswerOwner = value; }
    public int VRPlayerGuess { get => model.vrPlayerGuess; set => model.vrPlayerGuess = value; }
    public float ClientAnswerTimer { get => model.clientAnswerTimer; set => model.clientAnswerTimer = value; }
    public float DrawingTimer { get => model.drawingTimer; set => model.drawingTimer = value; }
    public bool VRCompletedTutorial { get => model.vrCompletedTutorial; set => model.vrCompletedTutorial = value; }
    public bool IsPainting { get => model.isPainting; set => model.isPainting = value; }
    public bool IsDrawing { get => model.isDrawing; set => model.isDrawing = value; }
    public bool IsPenEnabled { get => model.isPenEnabled; set => model.isPenEnabled = value; }
    public bool IsPaletteMirrored { get => model.isPaletteMirrored; set => model.isPaletteMirrored = value; }
    public bool IsPaletteEnabled { get => model.isPaletteEnabled; set => model.isPaletteEnabled = value; }

    public Color PenColor { get => model.penColor; set => model.penColor = value; }
    public Color SprayGunColor { get => model.sprayGunColor; set => model.sprayGunColor = value; }

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

        penEnabledChanged = new UnityEvent<bool>();
        paletteEnabledChanged = new UnityEvent<bool>();

        penColorChanged = new UnityEvent<Color>();
        spraygunColorChanged = new UnityEvent<Color>();

#if UNITY_WEBGL
        isWeb = true;
#endif
    }

#if UNITY_ANDROID //Only host has to worry about triggering allPlayersReady event
    private void Start()
    {
        //Default states when entering scene
        State = "clients answering";
        Answers = "";
        Guesses = "";
        CurrentPrompt = "";
        VRPlayerPoints = 0;
        DrawingTimer = ThreeDPaintGlobalVariables.DRAW_TIME_AMOUNT;
        ClientAnswerTimer = ThreeDPaintGlobalVariables.CLIENT_ANSWER_TIME_AMOUNT;
        VRCompletedTutorial = false;
        IsPainting = false;
        IsDrawing = false;
        IsPenEnabled = true;
        IsPaletteMirrored = false;
        IsPaletteEnabled = false;
        PenColor = Color.black;
        SprayGunColor = Color.black;

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
            previousModel.answersDidChange -= OnAnswersDidChange;
            previousModel.guessesDidChange -= OnGuessesDidChange;
            previousModel.isPaintingDidChange -= OnIsPaintingDidChange;
            previousModel.isDrawingDidChange -= OnIsDrawingDidChange;
            previousModel.isPenEnabledDidChange -= OnIsPenEnabledChanged;
            previousModel.isPaletteMirroredDidChange -= OnIsPaletteMirroredChanged;
            previousModel.isPaletteEnabledDidChange -= OnIsPaletteEnabledChanged;
            previousModel.penColorDidChange -= OnPenColorChanged;
            previousModel.sprayGunColorDidChange -= OnSprayGunColorChanged;
            previousModel.currentPromptDidChange -= OnPromptChanged;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                currentModel.state = "clients answering";
                currentModel.isPenEnabled = true;
            }

            // Register for events
            currentModel.stateDidChange += OnStateChange;
            currentModel.answersDidChange += OnAnswersDidChange;
            currentModel.guessesDidChange += OnGuessesDidChange;
            currentModel.isPaintingDidChange += OnIsPaintingDidChange;
            currentModel.isDrawingDidChange += OnIsDrawingDidChange;
            currentModel.isPenEnabledDidChange += OnIsPenEnabledChanged;
            currentModel.isPaletteMirroredDidChange += OnIsPaletteMirroredChanged;
            currentModel.isPaletteEnabledDidChange += OnIsPaletteEnabledChanged;
            currentModel.penColorDidChange += OnPenColorChanged;
            currentModel.sprayGunColorDidChange += OnSprayGunColorChanged;
            currentModel.currentPromptDidChange += OnPromptChanged;
        }
    }

    #region Variable Callbacks
    void OnStateChange(VRtistrySyncModel previousModel, string val)
    {
        OnStateChangeEvent.Invoke(val);
    }

    void OnAnswersDidChange(VRtistrySyncModel previousModel, string val)
    {
        if(!val.Equals(""))
        {
            OnPlayerAnswered.Invoke(val);
        }
    }

    void OnGuessesDidChange(VRtistrySyncModel previousModel, string val)
    {
        if (!val.Equals(""))
        {
            OnPlayerGuessed.Invoke(val);
        }
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

    private void OnIsPenEnabledChanged(VRtistrySyncModel model, bool value)
    {
        penEnabledChanged.Invoke(value);
    }

    private void OnIsPaletteMirroredChanged(VRtistrySyncModel model, bool value)
    {
        PaletteMirrored.Invoke();
    }

    private void OnIsPaletteEnabledChanged(VRtistrySyncModel model, bool value)
    {
        paletteEnabledChanged.Invoke(value);
    }

    private void OnPenColorChanged(VRtistrySyncModel model, Color value)
    {
        penColorChanged.Invoke(value);
    }

    private void OnSprayGunColorChanged(VRtistrySyncModel model, Color value)
    {
        spraygunColorChanged.Invoke(value);
    }

    private void OnPromptChanged(VRtistrySyncModel model, string value)
    {
        OnPromptChangedEvent.Invoke(value);
    }
    #endregion
}