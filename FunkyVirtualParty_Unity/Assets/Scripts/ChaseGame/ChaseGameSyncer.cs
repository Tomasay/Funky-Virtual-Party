using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ChaseGameSyncer : RealtimeComponent<ChaseGameSyncModel>
{
    public static ChaseGameSyncer instance;

    public string State { get => model.state; set => model.state = value; }

    private void Awake()
    {
        //Singleton instantiation
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void OnRealtimeModelReplaced(ChaseGameSyncModel previousModel, ChaseGameSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            currentModel.stateDidChange -= OnStateChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
            }

            //Update to match new data


            // Register for events
            currentModel.stateDidChange += OnStateChange;
        }
    }

    #region Variable Callbacks
    void OnStateChange(ChaseGameSyncModel previousModel, string val)
    {
        
    }
    #endregion
}
