using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using DG.Tweening;

public class MaterialSyncer : RealtimeComponent<MaterialSyncModel>
{
    [SerializeField]
    Material mat;

    public string SetColor { get => model.setColor; set => model.setColor = value; }
    public string SetColorWithTween { get => model.setColorWithTween; set => model.setColorWithTween = value; }

    protected override void OnRealtimeModelReplaced(MaterialSyncModel previousModel, MaterialSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.setColorDidChange -= OnSetColorChange;
            previousModel.setColorWithTweenDidChange -= OnSetColorWithTweenChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {

            }

            // Register for events
            currentModel.setColorDidChange += OnSetColorChange;
            currentModel.setColorWithTweenDidChange += OnSetColorWithTweenChange;
        }
    }

    #region Variable Callbacks
    void OnSetColorChange(MaterialSyncModel previousModel, string val)
    {
        if (val.Equals("")) return;

        string[] vals = val.Split(',');

        if (ColorUtility.TryParseHtmlString(vals[1], out Color col))
        {
            mat.SetColor(vals[0], col);
        }
    }

    void OnSetColorWithTweenChange(MaterialSyncModel previousModel, string val)
    {
        if (val.Equals("")) return;

        Debug.Log("OnSetColorWithTweenChange: " + val);

        string[] vals = val.Split(',');

        if (ColorUtility.TryParseHtmlString(vals[1], out Color col) && float.TryParse(vals[2], out float f))
        {
            Debug.Log("OnSetColorWithTweenChange Parsed: " + vals[0] + " " + vals[1] + " " + vals[2]);
            mat.DOColor(col, vals[0], f);
        }
    }
    #endregion
}