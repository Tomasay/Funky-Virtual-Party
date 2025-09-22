using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSyncer : RealtimeComponent<SpriteSyncModel>
{
    SpriteRenderer sr;

    public bool Enabled { get => model.enabled; set => model.enabled = value; }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    protected override void OnRealtimeModelReplaced(SpriteSyncModel previousModel, SpriteSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.enabledDidChange -= OnEnabledChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                currentModel.enabled = sr.enabled;
            }

            // Register for events
            currentModel.enabledDidChange += OnEnabledChange;
        }

        //Update to match new data
        sr.enabled = model.enabled;
    }

    #region Variable Callbacks
    void OnEnabledChange(SpriteSyncModel previousModel, bool val)
    {
        sr.enabled = val;
    }
    #endregion
}