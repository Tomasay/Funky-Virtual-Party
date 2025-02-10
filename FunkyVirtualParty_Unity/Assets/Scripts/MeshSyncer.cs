using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(MeshRenderer))]
public class MeshSyncer : RealtimeComponent<MeshSyncModel>
{
    MeshRenderer mr;

    public bool Enabled { get => model.enabled; set => model.enabled = value; }

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    protected override void OnRealtimeModelReplaced(MeshSyncModel previousModel, MeshSyncModel currentModel)
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
                currentModel.enabled = mr.enabled;
            }

            // Register for events
            currentModel.enabledDidChange += OnEnabledChange;
        }
    }

    #region Variable Callbacks
    void OnEnabledChange(MeshSyncModel previousModel, bool val)
    {
        mr.enabled = val;
    }
    #endregion
}