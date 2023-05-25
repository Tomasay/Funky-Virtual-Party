using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using TMPro;

public class ClientSync : RealtimeComponent<ClientSyncModel>
{
    [SerializeField]
    ClientPlayer cp;

    [SerializeField]
    Animator anim;

    [SerializeField]
    TMP_Text nameText;

    #region Properties
    public Color Color { get => model.color; set => model.color = value; }
    public string Name { get => model.name; set => model.name = value; }
    public Color NameColor { get => model.nameColor; set => model.nameColor = value; }
    public float AnimSpeed { get => model.animSpeed; set => model.animSpeed = value; }
    public float Height { get => model.height; set => model.height = value; } //Between -0.2 and 2.0
    public int HatIndex { get => model.hatIndex; set => model.hatIndex = value; }
    public int HeadType { get => model.headType; set => model.headType = value; }
    public int DanceIndex { get => model.danceIndex; set => model.danceIndex = value; }

    #endregion

    protected override void OnRealtimeModelReplaced(ClientSyncModel previousModel, ClientSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.nameDidChange -= OnNameChange;
            currentModel.colorDidChange -= OnColorChange;
            currentModel.animSpeedDidChange -= OnAnimSpeedChange;
            currentModel.nameColorDidChange -= OnNameColorChange;
            currentModel.hatIndexDidChange -= OnHatChanged;
            currentModel.headTypeDidChange -= OnHeadTypeChanged;
            currentModel.heightDidChange -= OnHeightChanged;
            currentModel.danceIndexDidChange -= OnDanceIndexChanged;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
            }

            //Update to match new data
            cp.ChangeColor(model.color);
            nameText.text = model.name;
            cp.UpdateHat(model.hatIndex, false);
            cp.UpdateHeadType(model.headType);
            cp.UpdateHeight(model.height);
            if (model.danceIndex > 0) anim.SetTrigger("Dance" + model.danceIndex);

            // Register for events
            currentModel.nameDidChange += OnNameChange;
            currentModel.colorDidChange += OnColorChange;
            currentModel.animSpeedDidChange += OnAnimSpeedChange;
            currentModel.nameColorDidChange += OnNameColorChange;
            currentModel.hatIndexDidChange += OnHatChanged;
            currentModel.headTypeDidChange += OnHeadTypeChanged;
            currentModel.heightDidChange += OnHeightChanged;
            currentModel.danceIndexDidChange += OnDanceIndexChanged;
        }
    }

    #region Variable Callbacks
    private void OnColorChange(ClientSyncModel model, Color val)
    {
        cp.ChangeColor(val);
    }

    void OnNameChange(ClientSyncModel previousModel, string val)
    {
        nameText.text = val;
    }

    void OnNameColorChange(ClientSyncModel previousModel, Color val)
    {
        nameText.color = val;
    }

    void OnAnimSpeedChange(ClientSyncModel previousModel, float val)
    {
        anim.SetFloat("Speed", val);
    }

    void OnHatChanged(ClientSyncModel previousModel, int val)
    {
        cp.UpdateHat(val);
    }

    void OnHeadTypeChanged(ClientSyncModel previousModel, int val)
    {
        cp.UpdateHeadType(val);
    }

    void OnHeightChanged(ClientSyncModel previousModel, float val)
    {
        cp.UpdateHeight(val);
    }

    void OnDanceIndexChanged(ClientSyncModel previousModel, int val)
    {
        if (val > 0)
        {
            anim.SetTrigger("Dance" + val);
        }
    }
    #endregion
}