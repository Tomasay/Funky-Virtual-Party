using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class DataSyncTester : RealtimeComponent<DataSyncTestModel>
{
    [SerializeField]
    MeshRenderer mr;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void UpdateColor()
    {
        mr.material.color = model.color;
    }

    protected override void OnRealtimeModelReplaced(DataSyncTestModel previousModel, DataSyncTestModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.colorDidChange -= ColorDidChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it, populate it with the current mesh renderer color.
            if (currentModel.isFreshModel)
                currentModel.color = mr.material.color;

            // Update the mesh render to match the new model
            UpdateColor();

            // Register for events so we'll know if the color changes later
            currentModel.colorDidChange += ColorDidChange;
        }
    }

    void ColorDidChange(DataSyncTestModel previousModel, Color val)
    {
        UpdateColor();
    }

    public void SetColor(Color color)
    {
        model.color = color;
    }
}