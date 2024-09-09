using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;
using TMPro;
using Autohand;
using NaughtyAttributes;

public class MazeCoinSyncer : RealtimeComponent<MazeCoinSyncModel>
{
    MeshRenderer meshRenderer;
    BoxCollider col;

    public bool IsActive { get => model.isActive; set => model.isActive = value; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        transform.Rotate(0, 20 * Time.deltaTime, 0);

#if UNITY_ANDROID //Only VR host manages coin collection check
        List<ClientPlayer> cps = MazeGameClientPlayer.clients;
        foreach (ClientPlayer cp in cps)
        {
            if (col.bounds.Contains(cp.transform.position))
            {
                IsActive = false;
            }
        }
#endif
    }

    protected override void OnRealtimeModelReplaced(MazeCoinSyncModel previousModel, MazeCoinSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.isActiveDidChange -= IsActiveChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
            }

            //Update to match new data

            // Register for events
            currentModel.isActiveDidChange += IsActiveChange;
        }
    }

#region Variable Callbacks
    void IsActiveChange(MazeCoinSyncModel previousModel, bool val)
    {
        meshRenderer.enabled = val;

#if UNITY_ANDROID //Only VR host manages trigger enter
        col.enabled = val;
#endif
    }
    #endregion
}