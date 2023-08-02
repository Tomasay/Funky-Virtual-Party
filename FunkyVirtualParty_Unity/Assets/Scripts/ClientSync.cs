using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using TMPro;
using UnityEngine.Events;

public class ClientSync : RealtimeComponent<ClientSyncModel>
{
    [SerializeField]
    ClientPlayer cp;

    [SerializeField]
    Animator anim;

    [SerializeField]
    TMP_Text nameText;

    public UnityEvent OnDeath;

    #region Properties
    public Color Color { get => model.color; set => model.color = value; }
    public string Name { get => model.name; set => model.name = value; }
    public Color NameColor { get => model.nameColor; set => model.nameColor = value; }
    public float AnimSpeed { get => model.animSpeed; set => model.animSpeed = value; }
    public float Height { get => model.height; set => model.height = value; } //Between -0.2 and 2.0
    public int HatIndex { get => model.hatIndex; set => model.hatIndex = value; }
    public int HeadType { get => model.headType; set => model.headType = value; }
    public int DanceIndex { get => model.danceIndex; set => model.danceIndex = value; }
    public bool IsReady { get => model.isReady; set => model.isReady = value; }
    public bool OnDeathTrigger { get => model.onDeathTrigger; set => model.onDeathTrigger = value; }

    #endregion

    private void Awake()
    {
        OnDeath = new UnityEvent();
    }

    protected override void OnRealtimeModelReplaced(ClientSyncModel previousModel, ClientSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.nameDidChange -= OnNameChange;
            previousModel.colorDidChange -= OnColorChange;
            previousModel.animSpeedDidChange -= OnAnimSpeedChange;
            previousModel.nameColorDidChange -= OnNameColorChange;
            previousModel.hatIndexDidChange -= OnHatChanged;
            previousModel.headTypeDidChange -= OnHeadTypeChanged;
            previousModel.heightDidChange -= OnHeightChanged;
            previousModel.danceIndexDidChange -= OnDanceIndexChanged;
            previousModel.isReadyDidChange -= OnReadyUpChanged;
            previousModel.onDeathTriggerDidChange -= OnDeathTriggerChanged;
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
            if (model.isReady) ClientPlayer.OnReadyUp.Invoke(cp);

            // Register for events
            currentModel.nameDidChange += OnNameChange;
            currentModel.colorDidChange += OnColorChange;
            currentModel.animSpeedDidChange += OnAnimSpeedChange;
            currentModel.nameColorDidChange += OnNameColorChange;
            currentModel.hatIndexDidChange += OnHatChanged;
            currentModel.headTypeDidChange += OnHeadTypeChanged;
            currentModel.heightDidChange += OnHeightChanged;
            currentModel.danceIndexDidChange += OnDanceIndexChanged;
            currentModel.isReadyDidChange += OnReadyUpChanged;
            currentModel.onDeathTriggerDidChange += OnDeathTriggerChanged;
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

    void OnReadyUpChanged(ClientSyncModel previousModel, bool val)
    {
        if (val)
        {
            ClientPlayer.OnReadyUp.Invoke(cp);
        }
    }

    void OnDeathTriggerChanged(ClientSyncModel previousModel, bool val)
    {
        if (val)
        {
            OnDeath.Invoke();
            model.onDeathTrigger = false;
        }
    }
    #endregion
}