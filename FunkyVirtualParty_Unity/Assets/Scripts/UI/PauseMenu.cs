using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Autohand.Demo;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] float positionOffset = 3.0f;

    private VRPlayerController vrPlayer;
    private PositionConstraint posContraint;
    private XRControllerEvent leftMenuEvent;
    private Canvas can;

    void Start()
    {
        //Canvas
        can = GetComponent<Canvas>();
        can.enabled = false;

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;


        //TODO: Menu should always spawn in front of the player's face
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        vrPlayer = avatar.GetComponent<VRPlayerController>();

        //Setup position constraint to follow player camera
        posContraint = GetComponent<PositionConstraint>();
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = vrPlayer.Ahp.headCamera.transform;
        src.weight = 1;
        posContraint.AddSource(src);
        posContraint.constraintActive = true;
        posContraint.translationOffset = new Vector3(0, 0, positionOffset);

        //Setup event for left menu button to toggle pause menu
        leftMenuEvent = gameObject.AddComponent<XRControllerEvent>();
        leftMenuEvent.link = vrPlayer.Ahp.handLeft.GetComponent<XRHandControllerLink>();
        leftMenuEvent.button = CommonButton.menuButton;
        leftMenuEvent.Pressed = new UnityEngine.Events.UnityEvent();
        leftMenuEvent.Pressed.AddListener(this.ToggleMenu);
    }

    public void ToggleMenu()
    {
        can.enabled = !can.enabled;
    }

    public void ResumePressed()
    {
        can.enabled = false;
    }

    public void RestartPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenuPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}