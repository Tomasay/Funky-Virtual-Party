using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class FireballSyncModel
{
    [RealtimeProperty(1, true, true)]
    private float _currentScale; //Between 0 and 1, indicates level between min and max size

    [RealtimeProperty(2, true, true)]
    private bool _boosted; //Whether or not fireball has been boosted, if so turns blue and launches 3 more fireballs on explosion

    [RealtimeProperty(3, true, true)]
    private bool _active; //Whether or not fireball is visible and active

    [RealtimeProperty(4, true, true)]
    private bool _explosionTrigger;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class FireballSyncModel : RealtimeModel {
    public float currentScale {
        get {
            return _currentScaleProperty.value;
        }
        set {
            if (_currentScaleProperty.value == value) return;
            _currentScaleProperty.value = value;
            InvalidateReliableLength();
            FireCurrentScaleDidChange(value);
        }
    }
    
    public bool boosted {
        get {
            return _boostedProperty.value;
        }
        set {
            if (_boostedProperty.value == value) return;
            _boostedProperty.value = value;
            InvalidateReliableLength();
            FireBoostedDidChange(value);
        }
    }
    
    public bool active {
        get {
            return _activeProperty.value;
        }
        set {
            if (_activeProperty.value == value) return;
            _activeProperty.value = value;
            InvalidateReliableLength();
            FireActiveDidChange(value);
        }
    }
    
    public bool explosionTrigger {
        get {
            return _explosionTriggerProperty.value;
        }
        set {
            if (_explosionTriggerProperty.value == value) return;
            _explosionTriggerProperty.value = value;
            InvalidateReliableLength();
            FireExplosionTriggerDidChange(value);
        }
    }
    
    public delegate void PropertyChangedHandler<in T>(FireballSyncModel model, T value);
    public event PropertyChangedHandler<float> currentScaleDidChange;
    public event PropertyChangedHandler<bool> boostedDidChange;
    public event PropertyChangedHandler<bool> activeDidChange;
    public event PropertyChangedHandler<bool> explosionTriggerDidChange;
    
    public enum PropertyID : uint {
        CurrentScale = 1,
        Boosted = 2,
        Active = 3,
        ExplosionTrigger = 4,
    }
    
    #region Properties
    
    private ReliableProperty<float> _currentScaleProperty;
    
    private ReliableProperty<bool> _boostedProperty;
    
    private ReliableProperty<bool> _activeProperty;
    
    private ReliableProperty<bool> _explosionTriggerProperty;
    
    #endregion
    
    public FireballSyncModel() : base(null) {
        _currentScaleProperty = new ReliableProperty<float>(1, _currentScale);
        _boostedProperty = new ReliableProperty<bool>(2, _boosted);
        _activeProperty = new ReliableProperty<bool>(3, _active);
        _explosionTriggerProperty = new ReliableProperty<bool>(4, _explosionTrigger);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        _currentScaleProperty.UnsubscribeCallback();
        _boostedProperty.UnsubscribeCallback();
        _activeProperty.UnsubscribeCallback();
        _explosionTriggerProperty.UnsubscribeCallback();
    }
    
    private void FireCurrentScaleDidChange(float value) {
        try {
            currentScaleDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireBoostedDidChange(bool value) {
        try {
            boostedDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireActiveDidChange(bool value) {
        try {
            activeDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireExplosionTriggerDidChange(bool value) {
        try {
            explosionTriggerDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _currentScaleProperty.WriteLength(context);
        length += _boostedProperty.WriteLength(context);
        length += _activeProperty.WriteLength(context);
        length += _explosionTriggerProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _currentScaleProperty.Write(stream, context);
        writes |= _boostedProperty.Write(stream, context);
        writes |= _activeProperty.Write(stream, context);
        writes |= _explosionTriggerProperty.Write(stream, context);
        if (writes) InvalidateContextLength(context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        var anyPropertiesChanged = false;
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            var changed = false;
            switch (propertyID) {
                case (uint) PropertyID.CurrentScale: {
                    changed = _currentScaleProperty.Read(stream, context);
                    if (changed) FireCurrentScaleDidChange(currentScale);
                    break;
                }
                case (uint) PropertyID.Boosted: {
                    changed = _boostedProperty.Read(stream, context);
                    if (changed) FireBoostedDidChange(boosted);
                    break;
                }
                case (uint) PropertyID.Active: {
                    changed = _activeProperty.Read(stream, context);
                    if (changed) FireActiveDidChange(active);
                    break;
                }
                case (uint) PropertyID.ExplosionTrigger: {
                    changed = _explosionTriggerProperty.Read(stream, context);
                    if (changed) FireExplosionTriggerDidChange(explosionTrigger);
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
        _currentScale = currentScale;
        _boosted = boosted;
        _active = active;
        _explosionTrigger = explosionTrigger;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */