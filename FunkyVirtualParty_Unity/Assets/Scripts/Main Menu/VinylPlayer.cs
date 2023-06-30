using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Animator))]
public class VinylPlayer : MonoBehaviour
{
    [SerializeField] SceneChangerSyncer sceneChanger;

    [SerializeField] Transform vinylParent;

    [SerializeField] TMP_Text titleText, descriptionText;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] RawImage videoTexture;

    [SerializeField] GameObject trianglesParent;

    private Animator anim;
    private GameObject currentVinyl = null;

    private bool isSceneLoading = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        titleText.enabled = false;
        descriptionText.enabled = false;
        videoTexture.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!currentVinyl && other.gameObject.name.Contains("Vinyl")) //If vinyl is in trigger, and there is no vinyl on the player
        {
            Grabbable g = other.gameObject.GetComponent<Grabbable>();

            if (!g.IsHeld()) //If vinyl is released from hands
            {
                currentVinyl = other.gameObject;
                VinylInfo info = other.gameObject.GetComponent<VinylInfo>();

                //Set vinyl pos/rot
                info.SetDiscOnPlayer();
                anim.SetTrigger("Play");

                //Set UI info
                titleText.enabled = true;
                descriptionText.enabled = true;
                titleText.text = info.Title;
                descriptionText.text = info.Description;
                videoPlayer.clip = info.videoPreview;
                videoTexture.enabled = true;

                //Load game scene in 3 seconds
                StartCoroutine(LoadSceneDelayed(info.SceneToLoad, 3));
            }
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(currentVinyl)) //If current vinyl leaves the trigger
        {
            //Reset physics
            Rigidbody rb = currentVinyl.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            currentVinyl = null;

            anim.SetTrigger("Stop");
        }
    }
    */

    private void Update()
    {
#if UNITY_EDITOR
        if (!isSceneLoading)
        {
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                sceneChanger.CurrentScene = "ChaseGame";
            }
            else if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                sceneChanger.CurrentScene = "Shootout";
            }
            else if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                sceneChanger.CurrentScene = "MazeGame";
            }
            else if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                sceneChanger.CurrentScene = "Kaiju";
            }
            else if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                sceneChanger.CurrentScene = "3DPaintGame";
            }
        }
#endif
    }

    IEnumerator LoadSceneDelayed(string sceneName, int delay)
    {
        yield return new WaitForSeconds(delay);

        isSceneLoading = true;

        sceneChanger.CurrentScene = sceneName;
    }
}