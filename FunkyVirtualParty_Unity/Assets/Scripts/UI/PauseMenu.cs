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
    private OVRControllerEvent leftMenuEvent;
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

    private void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Start))
        {
            ToggleMenu();
        }
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
        /*
        leftMenuEvent = gameObject.AddComponent<OVRControllerEvent>();
        OVRControllerEventData newEventData = new OVRControllerEventData();
        newEventData.controller = OVRInput.Controller.LHand;
        newEventData.button = OVRInput.Button.Start;
        leftMenuEvent.eventList.Initialize();
        leftMenuEvent.eventList.SetValue(newEventData, 0);
        leftMenuEvent.eventList[0].OnPress.AddListener(this.ToggleMenu);
        */
    }

    public void ToggleMenu()
    {
        Debug.Log("ToggleMenu");
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