using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Autohand.Demo;
using Autohand;

public class VRTutorial : MonoBehaviour
{
    [SerializeField]
    GraphicRaycaster raycaster;

    [SerializeField]
    TMP_Text headerText;

    [SerializeField]
    GameObject sprayInstructions, swapToolsInstructions, drawInstructions, swapHandsInstructions, movementInstructions;

    [SerializeField]
    GameObject controllerImagesParent;

    [SerializeField]
    GameObject introButtons;

    [SerializeField]
    PaintSprayGun sprayGun;

    [SerializeField]
    ThreeDPen pen;

    [SerializeField]
    PaintPalette palette;

    [SerializeField]
    XRControllerEvent YButtonEvent, BButtonEvent;

    [SerializeField]
    AutoHandPlayer ahp;

    public enum TutorialStage
    {
        Intro,
        Spray,
        SwapTools,
        Draw,
        SwapColors,
        SwitchHands,
        Movement,
        Done
    }

    TutorialStage currentStage;

    public TutorialStage CurrentStage { get => currentStage; set { currentStage = value; OnStateChange(); } }

    bool hasMoved, hasRotated;

    // Start is called before the first frame update
    void Start()
    {
        sprayGun.OnSpray.AddListener(delegate { if (CurrentStage == TutorialStage.Spray) CurrentStage = TutorialStage.SwapTools; });
        pen.OnDraw.AddListener(delegate { if (CurrentStage == TutorialStage.Draw) CurrentStage = TutorialStage.SwapColors; });
        palette.OnColorChanged.AddListener(delegate { hasRotated = true; if (CurrentStage == TutorialStage.SwapColors) CurrentStage = TutorialStage.SwitchHands; });
        BButtonEvent.Pressed.AddListener(delegate { if (CurrentStage == TutorialStage.SwapTools) CurrentStage = TutorialStage.Draw; });
        YButtonEvent.Pressed.AddListener(delegate { if (CurrentStage == TutorialStage.SwitchHands) CurrentStage = TutorialStage.Movement; });
        ahp.OnMove.AddListener(delegate { if (CurrentStage == TutorialStage.Movement) { hasMoved = true; if (hasMoved && hasRotated) { CurrentStage = TutorialStage.Done; } } });
        ahp.OnRotate.AddListener(delegate { if (CurrentStage == TutorialStage.Movement) { hasRotated = true; if (hasMoved && hasRotated) { CurrentStage = TutorialStage.Done; } } });

        StartCoroutine("DisableMovementDelayed");
    }

    public void ContinueButtonPressed()
    {
        CurrentStage = TutorialStage.Spray;
    }

    void OnStateChange()
    {
        switch (currentStage)
        {
            case TutorialStage.Intro:
                break;
            case TutorialStage.Spray:
                raycaster.enabled = false;
                introButtons.SetActive(false);
                sprayInstructions.SetActive(true);
                headerText.text = "Use the trigger button to spray paint";
                break;
            case TutorialStage.SwapTools:
                sprayInstructions.SetActive(false);
                swapToolsInstructions.SetActive(true);
                headerText.text = "Press the primary button to swap between your spray gun and 3D pen";
                break;
            case TutorialStage.Draw:
                swapToolsInstructions.SetActive(false);
                drawInstructions.SetActive(true);
                headerText.text = "Use the trigger button to draw in 3D space";
                break;
            case TutorialStage.SwapColors:
                drawInstructions.SetActive(false);
                headerText.text = "Tap your tool on the color palette to change colors";
                break;
            case TutorialStage.SwitchHands:
                swapHandsInstructions.SetActive(true);
                headerText.text = "To switch handedness, press the primary button in the hand holding your color palette";
                break;
            case TutorialStage.Movement:
                ahp.useMovement = true;
                swapHandsInstructions.SetActive(false);
                movementInstructions.SetActive(true);
                headerText.text = "Use the left joystick to move around, and the right joystick to rotate";
                break;
            case TutorialStage.Done:
                headerText.text = "You're ready to show off your skills! \nTurn around to look at the controls if you need any additional help";

                controllerImagesParent.SetActive(false);

                sprayGun.OnSpray.RemoveListener(delegate { if (CurrentStage == TutorialStage.Spray) CurrentStage = TutorialStage.SwapTools; });
                pen.OnDraw.RemoveListener(delegate { if (CurrentStage == TutorialStage.Draw) CurrentStage = TutorialStage.SwapColors; });
                BButtonEvent.Pressed.RemoveListener(delegate { if (CurrentStage == TutorialStage.SwapTools) CurrentStage = TutorialStage.Draw; });
                YButtonEvent.Pressed.RemoveListener(delegate { if (CurrentStage == TutorialStage.SwitchHands) CurrentStage = TutorialStage.Movement; });
                ahp.OnMove.RemoveListener(delegate { if (CurrentStage == TutorialStage.Movement) { hasMoved = true; if (hasMoved && hasRotated) { CurrentStage = TutorialStage.Done; } } });
                ahp.OnRotate.RemoveListener(delegate { if (CurrentStage == TutorialStage.Movement) { hasRotated = true; if (hasMoved && hasRotated) { CurrentStage = TutorialStage.Done; } } });

                StartCoroutine("CloseTutorial");
                break;
            default:
                break;
        }
    }

    IEnumerator CloseTutorial()
    {
        yield return new WaitForSeconds(3);

        gameObject.SetActive(false);
    }

    IEnumerator DisableMovementDelayed()
    {
        yield return new WaitForSeconds(1);

        ahp.useMovement = false;
    }
}