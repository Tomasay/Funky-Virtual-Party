using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Autohand.Demo;
using Autohand;

public class VRTutorial : MonoBehaviour
{
    //TODO: Changed for standalone
    [SerializeField]
    //ThreeDPaintGameManager gm;
    VRtistryGameManager gm;

    [SerializeField]
    GraphicRaycaster raycaster;

    [SerializeField]
    TMP_Text headerText;

    [SerializeField]
    GameObject paintInstructions, swapHandsInstructions, movementInstructions;

    [SerializeField]
    GameObject controllerLeft, controllerRight;

    [SerializeField]
    Animator controllerInstructionsAnim;

    [SerializeField]
    GameObject introButtons;

    [SerializeField]
    Transform mannequinCloseUpSpawnPos;

    PaintBrush paintBrush;

    PaintPalette palette;

    private VRtistryVRPlayerController vrPlayer;

    public bool bButtonEnabled, yButtonEnabled;

    public enum TutorialStage
    {
        Intro,
        PaintCollision,
        PaintAir,
        SwapColors,
        SwitchHands,
        Movement,
        Done
    }

    TutorialStage currentStage;

    public TutorialStage CurrentStage { get => currentStage; set { currentStage = value; OnStateChange(); } }

    bool hasMoved, hasRotated;

    void Start()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
    }

    public void SetTools(PaintBrush pb, PaintPalette pp)
    {
        paintBrush = pb;
        palette = pp;

        paintBrush.paintSyncer.OnHandleHitline.AddListener(delegate { if (CurrentStage == TutorialStage.PaintCollision) CurrentStage = TutorialStage.PaintAir; });
        paintBrush.OnDraw.AddListener(delegate { if (CurrentStage == TutorialStage.PaintAir) CurrentStage = TutorialStage.SwapColors; });
        palette.OnColorChanged.AddListener(delegate { hasRotated = true; if (CurrentStage == TutorialStage.SwapColors) CurrentStage = TutorialStage.Movement; });
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        vrPlayer = avatar.GetComponent<VRtistryVRPlayerController>();

        vrPlayer.Ahp.maxMoveSpeed = 0;

        vrPlayer.Ahp.OnMove.AddListener(delegate { if (CurrentStage == TutorialStage.Movement) { hasMoved = true; if (hasMoved && hasRotated) { CurrentStage = TutorialStage.Done; } } });
        vrPlayer.Ahp.OnRotate.AddListener(delegate { if (CurrentStage == TutorialStage.Movement) { hasRotated = true; if (hasMoved && hasRotated) { CurrentStage = TutorialStage.Done; } } });
    }

    public void ContinueButtonPressed()
    {
        CurrentStage = TutorialStage.PaintCollision;
    }

    public void SkipButtonPressed()
    {
        vrPlayer.Ahp.maxMoveSpeed = 3;

        RemoveListeners();

        gameObject.SetActive(false);
    }

    void OnStateChange()
    {
        switch (currentStage)
        {
            case TutorialStage.Intro:
                break;
            case TutorialStage.PaintCollision:
                StartCoroutine(OnPaintCollision());
                break;
            case TutorialStage.PaintAir:
                StartCoroutine(OnPaintAir());
                break;
            case TutorialStage.SwapColors:
                bButtonEnabled = true;

                paintInstructions.SetActive(false);
                controllerLeft.SetActive(false); controllerRight.SetActive(false);
                headerText.text = "Tap your brush on the color palette to change colors";
                headerText.verticalAlignment = VerticalAlignmentOptions.Middle;
                break;
            case TutorialStage.SwitchHands:
                swapHandsInstructions.SetActive(true);
                controllerInstructionsAnim.SetTrigger("Next");
                headerText.text = "To switch handedness, press the primary button in the hand holding your color palette";
                break;
            case TutorialStage.Movement:
                vrPlayer.Ahp.maxMoveSpeed = 3;

                swapHandsInstructions.SetActive(false);
                movementInstructions.SetActive(true);
                controllerLeft.SetActive(true); controllerRight.SetActive(true);
                controllerInstructionsAnim.SetTrigger("Next");
                headerText.text = "Use the left joystick to move around, and the right joystick to rotate";
                headerText.verticalAlignment = VerticalAlignmentOptions.Top;
                break;
            case TutorialStage.Done:
                headerText.text = "You're ready to show off your skills!";

                movementInstructions.SetActive(false);

                RemoveListeners();

                StartCoroutine("CloseTutorial");
                break;
            default:
                break;
        }
    }

    IEnumerator OnPaintCollision()
    {
        paintBrush.CanPaintAir = false;

        yield return new WaitForSeconds(0.25f);

        SceneChangerSyncer.instance.FadeOutManual();
        StartCoroutine(gm.SetVRPlayerPos(mannequinCloseUpSpawnPos.position, 1));

        yield return new WaitForSeconds(1.0f);

        raycaster.enabled = false;
        introButtons.SetActive(false);

        bButtonEnabled = false;
        headerText.text = "Touch the paint brush to the mannequin to paint on it";
    }

    IEnumerator OnPaintAir()
    {
        yield return new WaitForSeconds(1);

        paintBrush.CanPaintAir = true;

        SceneChangerSyncer.instance.FadeOutManual();

        yield return new WaitForSeconds(1);

        StartCoroutine(gm.SetVRPlayerPos(vrPlayer.spawnPos, 1));

        yield return new WaitForSeconds(0.5f);

        bButtonEnabled = false;

        paintInstructions.SetActive(true);
        controllerLeft.SetActive(true); controllerRight.SetActive(true);
        controllerInstructionsAnim.SetTrigger("Next");
        headerText.text = "Use the trigger button to paint in 3D space";
        headerText.verticalAlignment = VerticalAlignmentOptions.Top;

        yield return new WaitForSeconds(0.5f);
    }

    void RemoveListeners()
    {
        paintBrush.paintSyncer.OnHandleHitline.RemoveListener(delegate { if (CurrentStage == TutorialStage.PaintCollision) CurrentStage = TutorialStage.PaintAir; });
        paintBrush.OnDraw.RemoveListener(delegate { if (CurrentStage == TutorialStage.PaintAir) CurrentStage = TutorialStage.SwapColors; });
        palette.OnColorChanged.RemoveListener(delegate { hasRotated = true; if (CurrentStage == TutorialStage.SwapColors) CurrentStage = TutorialStage.Movement; });
        vrPlayer.Ahp.OnMove.RemoveListener(delegate { if (CurrentStage == TutorialStage.Movement) { hasMoved = true; if (hasMoved && hasRotated) { CurrentStage = TutorialStage.Done; } } });
        vrPlayer.Ahp.OnRotate.RemoveListener(delegate { if (CurrentStage == TutorialStage.Movement) { hasRotated = true; if (hasMoved && hasRotated) { CurrentStage = TutorialStage.Done; } } });

        yButtonEnabled = true;
        bButtonEnabled = true;
    }

    IEnumerator CloseTutorial()
    {
        yield return new WaitForSeconds(3);

        gameObject.SetActive(false);

        gm.OnTutorialCompleted();
    }
}