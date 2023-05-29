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
        if(!TryGetComponent<Animator>(out anim))
        {
            Debug.LogWarning("No animator found on vinyl player!");
        }

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
                VinylDiscSyncer currentVinylSyncer = other.gameObject.GetComponent<VinylDiscSyncer>();

                //Set vinyl pos/rot
                currentVinylSyncer.SetDiscOnPlayer();

                anim.SetTrigger("Play");

                //Set UI info
                VinylInfo info = other.gameObject.GetComponent<VinylInfo>();
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
                Debug.Log("Bruh");
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