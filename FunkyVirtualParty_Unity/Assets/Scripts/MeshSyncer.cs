using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class MeshSyncer : RealtimeComponent<MeshSyncModel>
{
    MeshRenderer mr;
    SkinnedMeshRenderer smr;

    bool isSMR = false;

    public bool Enabled { get => model.enabled; set => model.enabled = value; }

    void Awake()
    {
        if(TryGetComponent(out MeshRenderer m))
        {
            mr = m;
        }
        else if(TryGetComponent(out SkinnedMeshRenderer s))
        {
            smr = s;
            isSMR = true;
        }
        else
        {
            Debug.LogError("No MeshRenderer or SkinnedMeshRenderer was found");
        }
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
                if(isSMR)
                {
                    currentModel.enabled = smr.enabled;
                }
                else
                {
                    currentModel.enabled = mr.enabled;
                }
            }

            // Register for events
            currentModel.enabledDidChange += OnEnabledChange;
        }

        //Update to match new data
        SetMeshEnabled(model.enabled);
    }

    #region Variable Callbacks
    void OnEnabledChange(MeshSyncModel previousModel, bool val)
    {
        SetMeshEnabled(val);
    }

    void SetMeshEnabled(bool enabled)
    {
        if (isSMR)
        {
            smr.enabled = enabled;
        }
        else
        {
            mr.enabled = enabled;
        }
    }
    #endregion
}