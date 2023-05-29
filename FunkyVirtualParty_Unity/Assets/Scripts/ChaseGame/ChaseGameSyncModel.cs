using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class ChaseGameSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _state;

    [RealtimeProperty(2, true, true)]
    private int _timer;

    [RealtimeProperty(3, true, true)]
    private bool _vrPlayerReady;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class ChaseGameSyncModel : RealtimeModel {
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
    
    public delegate void PropertyChangedHandler<in T>(ChaseGameSyncModel model, T value);
    public event PropertyChangedHandler<string> stateDidChange;
    public event PropertyChangedHandler<int> timerDidChange;
    
    public enum PropertyID : uint {
        State = 1,
        Timer = 2,
    }
    
    #region Properties
    
    private ReliableProperty<string> _stateProperty;
    
    private ReliableProperty<int> _timerProperty;
    
    #endregion
    
    public ChaseGameSyncModel() : base(null) {
        _stateProperty = new ReliableProperty<string>(1, _state);
        _timerProperty = new ReliableProperty<int>(2, _timer);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        _stateProperty.UnsubscribeCallback();
        _timerProperty.UnsubscribeCallback();
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
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _stateProperty.WriteLength(context);
        length += _timerProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _stateProperty.Write(stream, context);
        writes |= _timerProperty.Write(stream, context);
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
    }
    
}
/* ----- End Normal Autogenerated Code ----- */
