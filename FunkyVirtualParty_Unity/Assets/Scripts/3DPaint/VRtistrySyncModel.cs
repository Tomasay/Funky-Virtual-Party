using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class VRtistrySyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _state;

    [RealtimeProperty(2, true, true)]
    private int _timer;

    [RealtimeProperty(3, true, true)]
    private string _currentPrompt; //The current prompt that was chosen from list of answers

    [RealtimeProperty(4, true, true)]
    private string _answers; //list of client answers separated by \n, format is "CLIENT_ID:CLIENT_ANSWER"

    [RealtimeProperty(5, true, true)]
    private int _playersGuessed; //The amount of players who have guessed what the drawing is in the current phase

    [RealtimeProperty(6, true, true)]
    private string _chosenAnswerOwner; //Client ID of who's answer was chosen to give to the VR player as a prompt

    [RealtimeProperty(7, true, true)]
    private bool _isPainting; //Is the VR player spraying paint

    [RealtimeProperty(8, true, true)]
    private bool _isDrawing; //Is the VR player drawing with the pen

    [RealtimeProperty(9, true, true)]
    private Vector3 _drawingIncrement; //The latest point from pen drawing lines

    [RealtimeProperty(10, true, true)]
    private bool _isPenEnabled; //Is the pen enabled and in the VR player's hand? If false, the spray gun is

    [RealtimeProperty(11, true, true)]
    private Color _penColor;

    [RealtimeProperty(12, true, true)]
    private Color _sprayGunColor;

    [RealtimeProperty(13, true, true)]
    private bool _isPaletteMirrored;

    [RealtimeProperty(14, true, true)]
    private string _vrPlayerGuess; //The client ID that the vr player thinks wrote the answer

    [RealtimeProperty(15, true, true)]
    private int _vrPlayerPoints;

    [RealtimeProperty(16, true, true)]
    private bool _vrCompletedTutorial;

    [RealtimeProperty(17, true, true)]
    private string _guesses; //list of client guesses separated by \n, format is "CLIENT_ID:CLIENT_ANSWERID"
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class VRtistrySyncModel : RealtimeModel {
    public string state {
        get {
            return _stateProperty.value;
        }
        set {
            if (_stateProperty.value == value) return;
            _stateProperty.value = value;
            InvalidateReliableLength();
            FireStateDidChange(value);
        }
    }
    
    public int timer {
        get {
            return _timerProperty.value;
        }
        set {
            if (_timerProperty.value == value) return;
            _timerProperty.value = value;
            InvalidateReliableLength();
            FireTimerDidChange(value);
        }
    }
    
    public string currentPrompt {
        get {
            return _currentPromptProperty.value;
        }
        set {
            if (_currentPromptProperty.value == value) return;
            _currentPromptProperty.value = value;
            InvalidateReliableLength();
            FireCurrentPromptDidChange(value);
        }
    }
    
    public string answers {
        get {
            return _answersProperty.value;
        }
        set {
            if (_answersProperty.value == value) return;
            _answersProperty.value = value;
            InvalidateReliableLength();
            FireAnswersDidChange(value);
        }
    }
    
    public int playersGuessed {
        get {
            return _playersGuessedProperty.value;
        }
        set {
            if (_playersGuessedProperty.value == value) return;
            _playersGuessedProperty.value = value;
            InvalidateReliableLength();
            FirePlayersGuessedDidChange(value);
        }
    }
    
    public string chosenAnswerOwner {
        get {
            return _chosenAnswerOwnerProperty.value;
        }
        set {
            if (_chosenAnswerOwnerProperty.value == value) return;
            _chosenAnswerOwnerProperty.value = value;
            InvalidateReliableLength();
            FireChosenAnswerOwnerDidChange(value);
        }
    }
    
    public bool isPainting {
        get {
            return _isPaintingProperty.value;
        }
        set {
            if (_isPaintingProperty.value == value) return;
            _isPaintingProperty.value = value;
            InvalidateReliableLength();
            FireIsPaintingDidChange(value);
        }
    }
    
    public bool isDrawing {
        get {
            return _isDrawingProperty.value;
        }
        set {
            if (_isDrawingProperty.value == value) return;
            _isDrawingProperty.value = value;
            InvalidateReliableLength();
            FireIsDrawingDidChange(value);
        }
    }
    
    public UnityEngine.Vector3 drawingIncrement {
        get {
            return _drawingIncrementProperty.value;
        }
        set {
            if (_drawingIncrementProperty.value == value) return;
            _drawingIncrementProperty.value = value;
            InvalidateReliableLength();
            FireDrawingIncrementDidChange(value);
        }
    }
    
    public bool isPenEnabled {
        get {
            return _isPenEnabledProperty.value;
        }
        set {
            if (_isPenEnabledProperty.value == value) return;
            _isPenEnabledProperty.value = value;
            InvalidateReliableLength();
            FireIsPenEnabledDidChange(value);
        }
    }
    
    public UnityEngine.Color penColor {
        get {
            return _penColorProperty.value;
        }
        set {
            if (_penColorProperty.value == value) return;
            _penColorProperty.value = value;
            InvalidateReliableLength();
            FirePenColorDidChange(value);
        }
    }
    
    public UnityEngine.Color sprayGunColor {
        get {
            return _sprayGunColorProperty.value;
        }
        set {
            if (_sprayGunColorProperty.value == value) return;
            _sprayGunColorProperty.value = value;
            InvalidateReliableLength();
            FireSprayGunColorDidChange(value);
        }
    }
    
    public bool isPaletteMirrored {
        get {
            return _isPaletteMirroredProperty.value;
        }
        set {
            if (_isPaletteMirroredProperty.value == value) return;
            _isPaletteMirroredProperty.value = value;
            InvalidateReliableLength();
            FireIsPaletteMirroredDidChange(value);
        }
    }
    
    public string vrPlayerGuess {
        get {
            return _vrPlayerGuessProperty.value;
        }
        set {
            if (_vrPlayerGuessProperty.value == value) return;
            _vrPlayerGuessProperty.value = value;
            InvalidateReliableLength();
            FireVrPlayerGuessDidChange(value);
        }
    }
    
    public int vrPlayerPoints {
        get {
            return _vrPlayerPointsProperty.value;
        }
        set {
            if (_vrPlayerPointsProperty.value == value) return;
            _vrPlayerPointsProperty.value = value;
            InvalidateReliableLength();
            FireVrPlayerPointsDidChange(value);
        }
    }
    
    public bool vrCompletedTutorial {
        get {
            return _vrCompletedTutorialProperty.value;
        }
        set {
            if (_vrCompletedTutorialProperty.value == value) return;
            _vrCompletedTutorialProperty.value = value;
            InvalidateReliableLength();
            FireVrCompletedTutorialDidChange(value);
        }
    }
    
    public string guesses {
        get {
            return _guessesProperty.value;
        }
        set {
            if (_guessesProperty.value == value) return;
            _guessesProperty.value = value;
            InvalidateReliableLength();
            FireGuessesDidChange(value);
        }
    }
    
    public delegate void PropertyChangedHandler<in T>(VRtistrySyncModel model, T value);
    public event PropertyChangedHandler<string> stateDidChange;
    public event PropertyChangedHandler<int> timerDidChange;
    public event PropertyChangedHandler<string> currentPromptDidChange;
    public event PropertyChangedHandler<string> answersDidChange;
    public event PropertyChangedHandler<int> playersGuessedDidChange;
    public event PropertyChangedHandler<string> chosenAnswerOwnerDidChange;
    public event PropertyChangedHandler<bool> isPaintingDidChange;
    public event PropertyChangedHandler<bool> isDrawingDidChange;
    public event PropertyChangedHandler<UnityEngine.Vector3> drawingIncrementDidChange;
    public event PropertyChangedHandler<bool> isPenEnabledDidChange;
    public event PropertyChangedHandler<UnityEngine.Color> penColorDidChange;
    public event PropertyChangedHandler<UnityEngine.Color> sprayGunColorDidChange;
    public event PropertyChangedHandler<bool> isPaletteMirroredDidChange;
    public event PropertyChangedHandler<string> vrPlayerGuessDidChange;
    public event PropertyChangedHandler<int> vrPlayerPointsDidChange;
    public event PropertyChangedHandler<bool> vrCompletedTutorialDidChange;
    public event PropertyChangedHandler<string> guessesDidChange;
    
    public enum PropertyID : uint {
        State = 1,
        Timer = 2,
        CurrentPrompt = 3,
        Answers = 4,
        PlayersGuessed = 5,
        ChosenAnswerOwner = 6,
        IsPainting = 7,
        IsDrawing = 8,
        DrawingIncrement = 9,
        IsPenEnabled = 10,
        PenColor = 11,
        SprayGunColor = 12,
        IsPaletteMirrored = 13,
        VrPlayerGuess = 14,
        VrPlayerPoints = 15,
        VrCompletedTutorial = 16,
        Guesses = 17,
    }
    
    #region Properties
    
    private ReliableProperty<string> _stateProperty;
    
    private ReliableProperty<int> _timerProperty;
    
    private ReliableProperty<string> _currentPromptProperty;
    
    private ReliableProperty<string> _answersProperty;
    
    private ReliableProperty<int> _playersGuessedProperty;
    
    private ReliableProperty<string> _chosenAnswerOwnerProperty;
    
    private ReliableProperty<bool> _isPaintingProperty;
    
    private ReliableProperty<bool> _isDrawingProperty;
    
    private ReliableProperty<UnityEngine.Vector3> _drawingIncrementProperty;
    
    private ReliableProperty<bool> _isPenEnabledProperty;
    
    private ReliableProperty<UnityEngine.Color> _penColorProperty;
    
    private ReliableProperty<UnityEngine.Color> _sprayGunColorProperty;
    
    private ReliableProperty<bool> _isPaletteMirroredProperty;
    
    private ReliableProperty<string> _vrPlayerGuessProperty;
    
    private ReliableProperty<int> _vrPlayerPointsProperty;
    
    private ReliableProperty<bool> _vrCompletedTutorialProperty;
    
    private ReliableProperty<string> _guessesProperty;
    
    #endregion
    
    public VRtistrySyncModel() : base(null) {
        _stateProperty = new ReliableProperty<string>(1, _state);
        _timerProperty = new ReliableProperty<int>(2, _timer);
        _currentPromptProperty = new ReliableProperty<string>(3, _currentPrompt);
        _answersProperty = new ReliableProperty<string>(4, _answers);
        _playersGuessedProperty = new ReliableProperty<int>(5, _playersGuessed);
        _chosenAnswerOwnerProperty = new ReliableProperty<string>(6, _chosenAnswerOwner);
        _isPaintingProperty = new ReliableProperty<bool>(7, _isPainting);
        _isDrawingProperty = new ReliableProperty<bool>(8, _isDrawing);
        _drawingIncrementProperty = new ReliableProperty<UnityEngine.Vector3>(9, _drawingIncrement);
        _isPenEnabledProperty = new ReliableProperty<bool>(10, _isPenEnabled);
        _penColorProperty = new ReliableProperty<UnityEngine.Color>(11, _penColor);
        _sprayGunColorProperty = new ReliableProperty<UnityEngine.Color>(12, _sprayGunColor);
        _isPaletteMirroredProperty = new ReliableProperty<bool>(13, _isPaletteMirrored);
        _vrPlayerGuessProperty = new ReliableProperty<string>(14, _vrPlayerGuess);
        _vrPlayerPointsProperty = new ReliableProperty<int>(15, _vrPlayerPoints);
        _vrCompletedTutorialProperty = new ReliableProperty<bool>(16, _vrCompletedTutorial);
        _guessesProperty = new ReliableProperty<string>(17, _guesses);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        _stateProperty.UnsubscribeCallback();
        _timerProperty.UnsubscribeCallback();
        _currentPromptProperty.UnsubscribeCallback();
        _answersProperty.UnsubscribeCallback();
        _playersGuessedProperty.UnsubscribeCallback();
        _chosenAnswerOwnerProperty.UnsubscribeCallback();
        _isPaintingProperty.UnsubscribeCallback();
        _isDrawingProperty.UnsubscribeCallback();
        _drawingIncrementProperty.UnsubscribeCallback();
        _isPenEnabledProperty.UnsubscribeCallback();
        _penColorProperty.UnsubscribeCallback();
        _sprayGunColorProperty.UnsubscribeCallback();
        _isPaletteMirroredProperty.UnsubscribeCallback();
        _vrPlayerGuessProperty.UnsubscribeCallback();
        _vrPlayerPointsProperty.UnsubscribeCallback();
        _vrCompletedTutorialProperty.UnsubscribeCallback();
        _guessesProperty.UnsubscribeCallback();
    }
    
    private void FireStateDidChange(string value) {
        try {
            stateDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireTimerDidChange(int value) {
        try {
            timerDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireCurrentPromptDidChange(string value) {
        try {
            currentPromptDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireAnswersDidChange(string value) {
        try {
            answersDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FirePlayersGuessedDidChange(int value) {
        try {
            playersGuessedDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireChosenAnswerOwnerDidChange(string value) {
        try {
            chosenAnswerOwnerDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireIsPaintingDidChange(bool value) {
        try {
            isPaintingDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireIsDrawingDidChange(bool value) {
        try {
            isDrawingDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireDrawingIncrementDidChange(UnityEngine.Vector3 value) {
        try {
            drawingIncrementDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireIsPenEnabledDidChange(bool value) {
        try {
            isPenEnabledDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FirePenColorDidChange(UnityEngine.Color value) {
        try {
            penColorDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireSprayGunColorDidChange(UnityEngine.Color value) {
        try {
            sprayGunColorDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireIsPaletteMirroredDidChange(bool value) {
        try {
            isPaletteMirroredDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireVrPlayerGuessDidChange(string value) {
        try {
            vrPlayerGuessDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireVrPlayerPointsDidChange(int value) {
        try {
            vrPlayerPointsDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireVrCompletedTutorialDidChange(bool value) {
        try {
            vrCompletedTutorialDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireGuessesDidChange(string value) {
        try {
            guessesDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _stateProperty.WriteLength(context);
        length += _timerProperty.WriteLength(context);
        length += _currentPromptProperty.WriteLength(context);
        length += _answersProperty.WriteLength(context);
        length += _playersGuessedProperty.WriteLength(context);
        length += _chosenAnswerOwnerProperty.WriteLength(context);
        length += _isPaintingProperty.WriteLength(context);
        length += _isDrawingProperty.WriteLength(context);
        length += _drawingIncrementProperty.WriteLength(context);
        length += _isPenEnabledProperty.WriteLength(context);
        length += _penColorProperty.WriteLength(context);
        length += _sprayGunColorProperty.WriteLength(context);
        length += _isPaletteMirroredProperty.WriteLength(context);
        length += _vrPlayerGuessProperty.WriteLength(context);
        length += _vrPlayerPointsProperty.WriteLength(context);
        length += _vrCompletedTutorialProperty.WriteLength(context);
        length += _guessesProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _stateProperty.Write(stream, context);
        writes |= _timerProperty.Write(stream, context);
        writes |= _currentPromptProperty.Write(stream, context);
        writes |= _answersProperty.Write(stream, context);
        writes |= _playersGuessedProperty.Write(stream, context);
        writes |= _chosenAnswerOwnerProperty.Write(stream, context);
        writes |= _isPaintingProperty.Write(stream, context);
        writes |= _isDrawingProperty.Write(stream, context);
        writes |= _drawingIncrementProperty.Write(stream, context);
        writes |= _isPenEnabledProperty.Write(stream, context);
        writes |= _penColorProperty.Write(stream, context);
        writes |= _sprayGunColorProperty.Write(stream, context);
        writes |= _isPaletteMirroredProperty.Write(stream, context);
        writes |= _vrPlayerGuessProperty.Write(stream, context);
        writes |= _vrPlayerPointsProperty.Write(stream, context);
        writes |= _vrCompletedTutorialProperty.Write(stream, context);
        writes |= _guessesProperty.Write(stream, context);
        if (writes) InvalidateContextLength(context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        var anyPropertiesChanged = false;
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            var changed = false;
            switch (propertyID) {
                case (uint) PropertyID.State: {
                    changed = _stateProperty.Read(stream, context);
                    if (changed) FireStateDidChange(state);
                    break;
                }
                case (uint) PropertyID.Timer: {
                    changed = _timerProperty.Read(stream, context);
                    if (changed) FireTimerDidChange(timer);
                    break;
                }
                case (uint) PropertyID.CurrentPrompt: {
                    changed = _currentPromptProperty.Read(stream, context);
                    if (changed) FireCurrentPromptDidChange(currentPrompt);
                    break;
                }
                case (uint) PropertyID.Answers: {
                    changed = _answersProperty.Read(stream, context);
                    if (changed) FireAnswersDidChange(answers);
                    break;
                }
                case (uint) PropertyID.PlayersGuessed: {
                    changed = _playersGuessedProperty.Read(stream, context);
                    if (changed) FirePlayersGuessedDidChange(playersGuessed);
                    break;
                }
                case (uint) PropertyID.ChosenAnswerOwner: {
                    changed = _chosenAnswerOwnerProperty.Read(stream, context);
                    if (changed) FireChosenAnswerOwnerDidChange(chosenAnswerOwner);
                    break;
                }
                case (uint) PropertyID.IsPainting: {
                    changed = _isPaintingProperty.Read(stream, context);
                    if (changed) FireIsPaintingDidChange(isPainting);
                    break;
                }
                case (uint) PropertyID.IsDrawing: {
                    changed = _isDrawingProperty.Read(stream, context);
                    if (changed) FireIsDrawingDidChange(isDrawing);
                    break;
                }
                case (uint) PropertyID.DrawingIncrement: {
                    changed = _drawingIncrementProperty.Read(stream, context);
                    if (changed) FireDrawingIncrementDidChange(drawingIncrement);
                    break;
                }
                case (uint) PropertyID.IsPenEnabled: {
                    changed = _isPenEnabledProperty.Read(stream, context);
                    if (changed) FireIsPenEnabledDidChange(isPenEnabled);
                    break;
                }
                case (uint) PropertyID.PenColor: {
                    changed = _penColorProperty.Read(stream, context);
                    if (changed) FirePenColorDidChange(penColor);
                    break;
                }
                case (uint) PropertyID.SprayGunColor: {
                    changed = _sprayGunColorProperty.Read(stream, context);
                    if (changed) FireSprayGunColorDidChange(sprayGunColor);
                    break;
                }
                case (uint) PropertyID.IsPaletteMirrored: {
                    changed = _isPaletteMirroredProperty.Read(stream, context);
                    if (changed) FireIsPaletteMirroredDidChange(isPaletteMirrored);
                    break;
                }
                case (uint) PropertyID.VrPlayerGuess: {
                    changed = _vrPlayerGuessProperty.Read(stream, context);
                    if (changed) FireVrPlayerGuessDidChange(vrPlayerGuess);
                    break;
                }
                case (uint) PropertyID.VrPlayerPoints: {
                    changed = _vrPlayerPointsProperty.Read(stream, context);
                    if (changed) FireVrPlayerPointsDidChange(vrPlayerPoints);
                    break;
                }
                case (uint) PropertyID.VrCompletedTutorial: {
                    changed = _vrCompletedTutorialProperty.Read(stream, context);
                    if (changed) FireVrCompletedTutorialDidChange(vrCompletedTutorial);
                    break;
                }
                case (uint) PropertyID.Guesses: {
                    changed = _guessesProperty.Read(stream, context);
                    if (changed) FireGuessesDidChange(guesses);
                    break;
                }
                default: {
                    stream.SkipProperty();
                    break;
                }
            }
            anyPropertiesChanged |= changed;
        }
        if (anyPropertiesChanged) {
            UpdateBackingFields();
        }
    }
    
    private void UpdateBackingFields() {
        _state = state;
        _timer = timer;
        _currentPrompt = currentPrompt;
        _answers = answers;
        _playersGuessed = playersGuessed;
        _chosenAnswerOwner = chosenAnswerOwner;
        _isPainting = isPainting;
        _isDrawing = isDrawing;
        _drawingIncrement = drawingIncrement;
        _isPenEnabled = isPenEnabled;
        _penColor = penColor;
        _sprayGunColor = sprayGunColor;
        _isPaletteMirrored = isPaletteMirrored;
        _vrPlayerGuess = vrPlayerGuess;
        _vrPlayerPoints = vrPlayerPoints;
        _vrCompletedTutorial = vrCompletedTutorial;
        _guesses = guesses;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */
