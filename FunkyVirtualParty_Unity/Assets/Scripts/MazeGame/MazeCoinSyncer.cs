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

    public bool IsCollected { get => model.isCollected; set => model.isCollected = value; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        transform.Rotate(0, 40 * Time.deltaTime, 0);

#if UNITY_ANDROID //Only VR host manages coin collection check
        if (IsCollected == false)
        {
            List<ClientPlayer> cps = ClientPlayer.clients;
            foreach (ClientPlayer cp in cps)
            {
                if (col.bounds.Contains(cp.transform.position))
                {
                    IsCollected = true;
                }
            }
        }
#endif
    }

    protected override void OnRealtimeModelReplaced(MazeCoinSyncModel previousModel, MazeCoinSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.isCollectedDidChange -= IsActiveChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {

            }

            //Update to match new data

            // Register for events
            currentModel.isCollectedDidChange += IsActiveChange;
        }
    }

#region Variable Callbacks
    void IsActiveChange(MazeCoinSyncModel previousModel, bool val)
    {
        meshRenderer.enabled = !val;

#if UNITY_ANDROID //Only VR host manages trigger enter
        col.enabled = !val;
#endif
    }
    #endregion
}