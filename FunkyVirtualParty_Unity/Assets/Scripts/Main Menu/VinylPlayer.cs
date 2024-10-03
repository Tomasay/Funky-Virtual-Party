using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Animator))]
public class VinylPlayer : MonoBehaviour
{
    [SerializeField] Transform vinylParent;

    [SerializeField] TMP_Text titleText, descriptionText;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] RawImage videoTexture;
    [SerializeField] Image playButtonIcon;

    [SerializeField] GameObject trianglesParent;

    [SerializeField] Image playButtonImage;
    [SerializeField] Color playButtonHighlightedColor, playButtonUnhighlightedColor;
    Tweener playButtonTweener;

    private Animator anim;
    private VinylInfo currentVinyl = null;

    private bool isSceneLoading = false;

    private bool clientsPresent; //Are there clients present? If not, cannot start game

    private void Start()
    {
        anim = GetComponent<Animator>();

        titleText.enabled = false;
        descriptionText.enabled = false;
        videoTexture.enabled = false;

        ClientPlayer.OnClientConnected.AddListener(OnClientConnected);
        ClientPlayer.OnClientDisconnected.AddListener(OnClientSDisconnected);
    }

    public void PlayButtonPressed()
    {
        if(currentVinyl && clientsPresent)
        {
            isSceneLoading = true;
            SceneChangerSyncer.instance.CurrentScene = currentVinyl.SceneToLoad;
        }
    }

    void OnClientConnected(ClientPlayer cp)
    {
        CheckIfClientsPresent();
    }

    void OnClientSDisconnected(ClientPlayer cp)
    {
        CheckIfClientsPresent();
    }

    void CheckIfClientsPresent()
    {
        clientsPresent = ClientPlayer.clients != null && ClientPlayer.clients.Count > 0;

        SetPlayButtonAlpha((clientsPresent && currentVinyl) ? 1 : 0.25f);

        if(clientsPresent && currentVinyl) playButtonTweener = playButtonImage.DOColor(playButtonHighlightedColor, 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    void SetPlayButtonAlpha(float a)
    {
        Color c = playButtonIcon.color;
        c.a = a;
        playButtonIcon.color = c;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!currentVinyl && other.gameObject.name.Contains("Vinyl")) //If vinyl is in trigger, and there is no vinyl on the player
        {
            Grabbable g = other.gameObject.GetComponent<Grabbable>();

            if (!g.IsHeld()) //If vinyl is released from hands
            {
                currentVinyl = other.gameObject.GetComponent<VinylInfo>();

                //Set vinyl pos/rot
                currentVinyl.SetDiscOnPlayer();
                anim.SetTrigger("Play");

                //Set UI info
                titleText.enabled = true;
                descriptionText.enabled = true;
                titleText.text = currentVinyl.Title;
                descriptionText.text = currentVinyl.Description;
                videoPlayer.clip = currentVinyl.videoPreview;
                videoTexture.enabled = true;

                CheckIfClientsPresent();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<VinylInfo>(out VinylInfo vi))
        {
            if (vi.Equals(currentVinyl) && !vi.isOnPlayer) //If current vinyl leaves the trigger
            {
                //Clear UI
                titleText.enabled = false;
                descriptionText.enabled = false;
                videoTexture.enabled = false;
                SetPlayButtonAlpha(0.25f);

                currentVinyl = null;

                anim.SetTrigger("Stop");

                playButtonTweener.Kill();
                playButtonImage.color = playButtonUnhighlightedColor;
            }
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!isSceneLoading)
        {
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                SceneChangerSyncer.instance.CurrentScene = "ChaseGame";
            }
            else if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                SceneChangerSyncer.instance.CurrentScene = "Shootout";
            }
            else if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                SceneChangerSyncer.instance.CurrentScene = "MazeGame";
            }
            else if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                SceneChangerSyncer.instance.CurrentScene = "Kaiju";
            }
            else if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                isSceneLoading = true;
                SceneChangerSyncer.instance.CurrentScene = "3DPaintGame";
            }
        }
#endif
    }
}