using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using TMPro;
using UnityEngine.Events;
using PaintIn3D;
using NaughtyAttributes;

public class ClientSync : RealtimeComponent<ClientSyncModel>
{
    [SerializeField]
    ClientPlayer cp;

    [SerializeField]
    Animator anim;

    [SerializeField]
    TMP_Text nameText;

    [SerializeField]
    P3dPaintableTexture facePaintTexture;

    public UnityEvent OnDeath;

    public UnityEvent OnFaceDrawingChangedEvent;

    #region Properties
    public Color Color { get => model.color; set => model.color = value; }
    public string Name { get => model.name; set => model.name = value; }
    public Color NameColor { get => model.nameColor; set => model.nameColor = value; }
    public float AnimSpeed { get => model.animSpeed; set => model.animSpeed = value; }
    public float Height { get => model.height; set => model.height = value; } //Between -0.2 and 2.0
    public int HatIndex { get => model.hatIndex; set => model.hatIndex = value; }
    public int HeadType { get => model.headType; set => model.headType = value; }
    public bool IsReady { get => model.isReady; set => model.isReady = value; }
    public bool OnDeathTrigger { get => model.onDeathTrigger; set => model.onDeathTrigger = value; }
    public bool IsDebugPlayer { get => model.isDebugPlayer; set => model.isDebugPlayer = value; }
    public int IsDancing { get => model.isDancing; set => model.isDancing = value; }
    public int Score { get => model.score; set => model.score = value; }
    public byte[] FaceDrawing { get => model.faceDrawing; set => model.faceDrawing = value; }

    public string VrtistryAnswer { get => model.vrtistryAnswer; set => model.vrtistryAnswer = value; }
    public string VrtistryTypedGuess { get => model.vrtistryTypedGuess; set => model.vrtistryTypedGuess = value; }
    public int VrtistryArtGuess { get => model.vrtistryArtGuess; set => model.vrtistryArtGuess = value; }
    public int VrtistryPlayerGuess { get => model.vrtistryPlayerGuess; set => model.vrtistryPlayerGuess = value; }

    // Static events fired whenever any client changes their VRtistry guess properties.
    // The VR host subscribes to these to trigger count checks without race conditions.
    public static UnityEvent OnAnyVrtistryAnswerChanged = new UnityEvent();
    public static UnityEvent OnAnyVrtistryTypedGuessChanged = new UnityEvent();
    public static UnityEvent OnAnyVrtistryArtGuessChanged = new UnityEvent();
    public static UnityEvent OnAnyVrtistryPlayerGuessChanged = new UnityEvent();

    #endregion

    private void Awake()
    {
        OnDeath = new UnityEvent();
        OnFaceDrawingChangedEvent = new UnityEvent();
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
            previousModel.isReadyDidChange -= OnReadyUpChanged;
            previousModel.onDeathTriggerDidChange -= OnDeathTriggerChanged;
            previousModel.faceDrawingDidChange -= OnFaceDrawingChanged;
            previousModel.vrtistryAnswerDidChange -= OnVrtistryAnswerChanged;
            previousModel.vrtistryTypedGuessDidChange -= OnVrtistryTypedGuessChanged;
            previousModel.vrtistryArtGuessDidChange -= OnVrtistryArtGuessChanged;
            previousModel.vrtistryPlayerGuessDidChange -= OnVrtistryPlayerGuessChanged;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                currentModel.isDancing = -1;
                currentModel.hatIndex = -1;
                currentModel.vrtistryArtGuess = -1;
                currentModel.vrtistryPlayerGuess = -1;
            }

            //Update to match new data
            cp.ChangeColor(model.color);
            nameText.text = model.name;
            cp.UpdateHat(model.hatIndex, false);
            //cp.UpdateHeadType(model.headType);
            //cp.UpdateHeight(model.height);

            //Delay by 1 sec so paint texture has a chance to initialize
            if (facePaintTexture && model.faceDrawing != null) Invoke("ApplyFace", 1);
            if (model.isReady) ClientPlayer.OnReadyUp.Invoke(cp);

            // Register for events
            currentModel.nameDidChange += OnNameChange;
            currentModel.colorDidChange += OnColorChange;
            currentModel.animSpeedDidChange += OnAnimSpeedChange;
            currentModel.nameColorDidChange += OnNameColorChange;
            currentModel.hatIndexDidChange += OnHatChanged;
            currentModel.headTypeDidChange += OnHeadTypeChanged;
            currentModel.heightDidChange += OnHeightChanged;
            currentModel.isReadyDidChange += OnReadyUpChanged;
            currentModel.onDeathTriggerDidChange += OnDeathTriggerChanged;
            currentModel.faceDrawingDidChange += OnFaceDrawingChanged;
            currentModel.vrtistryAnswerDidChange += OnVrtistryAnswerChanged;
            currentModel.vrtistryTypedGuessDidChange += OnVrtistryTypedGuessChanged;
            currentModel.vrtistryArtGuessDidChange += OnVrtistryArtGuessChanged;
            currentModel.vrtistryPlayerGuessDidChange += OnVrtistryPlayerGuessChanged;
        }
    }

    [Button]
    void ApplyFace()
    {
        facePaintTexture.LoadFromData(model.faceDrawing);
    }

    #region Variable Callbacks
    private void OnColorChange(ClientSyncModel model, Color val)
    {
        cp.ChangeColor(val);
        ClientPlayer.OnColorChanged.Invoke(cp);
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
            Invoke("SetOnDeathFalse", 0.5f);
        }
    }

    void OnFaceDrawingChanged(ClientSyncModel previousModel, byte[] val)
    {
        if (facePaintTexture && val.Length > 0)
        {
            facePaintTexture.LoadFromData(val);
            OnFaceDrawingChangedEvent.Invoke();
        }
    }

    void OnVrtistryAnswerChanged(ClientSyncModel previousModel, string val)
    {
        if (!string.IsNullOrEmpty(val)) OnAnyVrtistryAnswerChanged.Invoke();
    }

    void OnVrtistryTypedGuessChanged(ClientSyncModel previousModel, string val)
    {
        if (!string.IsNullOrEmpty(val)) OnAnyVrtistryTypedGuessChanged.Invoke();
    }

    void OnVrtistryArtGuessChanged(ClientSyncModel previousModel, int val)
    {
        if (val != -1) OnAnyVrtistryArtGuessChanged.Invoke();
    }

    void OnVrtistryPlayerGuessChanged(ClientSyncModel previousModel, int val)
    {
        if (val != -1) OnAnyVrtistryPlayerGuessChanged.Invoke();
    }
    #endregion

    //Methods to reset play and stop triggers back to false
    //NOTE these are set back to false with a small delay, as setting them back immediately would cause the initial trigger to not invoke
    void SetOnDeathFalse()
    {
        model.onDeathTrigger = false;
    }
}