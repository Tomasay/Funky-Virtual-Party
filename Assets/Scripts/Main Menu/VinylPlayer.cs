using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VinylPlayer : MonoBehaviour
{
    [SerializeField] ClientManager cm;

    [SerializeField] Transform vinylParent;
    [SerializeField] float vinylSpinSpeed = 0.2f;

    [SerializeField] TMP_Text titleText, descriptionText;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] RawImage videoTexture;

    [SerializeField] GameObject trianglesParent;

    private Animator anim;
    private GameObject currentVinyl = null;

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

                //Set vinyl pos/rot
                Rigidbody rb = currentVinyl.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                currentVinyl.transform.position = vinylParent.position;
                currentVinyl.transform.rotation = Quaternion.Euler(Vector3.zero);

                anim.SetTrigger("Play");

                //Set UI info
                VinylInfo info = other.gameObject.GetComponent<VinylInfo>();
                titleText.enabled = true;
                descriptionText.enabled = true;
                titleText.text = info.Title;
                descriptionText.text = info.Description;
                videoPlayer.clip = info.videoPreview;
                videoTexture.enabled = true;

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
        //Spin current vinyl
        if(currentVinyl)
        {
            currentVinyl.transform.Rotate(0, vinylSpinSpeed, 0);
        }
    }

    IEnumerator LoadSceneDelayed(string sceneName, int delay)
    {
        yield return new WaitForSeconds(delay);

        cm.OnMinigameStart(sceneName);
    }
}