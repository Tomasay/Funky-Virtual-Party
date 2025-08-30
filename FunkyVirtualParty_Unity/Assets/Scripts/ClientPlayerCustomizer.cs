using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClientPlayerCustomizer : MonoBehaviour
{
    [SerializeField] Button toggleHatLeftButton, toggleHatRightButton, toggleColorLeftButton, toggleColorRightButton;
    [SerializeField] Button enableCustomizationButton, closeCustomizationButton;
    [SerializeField] Canvas controllerCanvas;
    [SerializeField] Camera cam;
    [SerializeField] Transform playerCustomizationCameraTransform, defaultCameraTransform;
    [SerializeField] float playerCustomizationNearClipPlane;

    [HideInInspector] public Canvas canvas;

    public UnityEvent OnCustomizationEnabled, OnCustomizationDisabled;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    void Start()
    {
        enableCustomizationButton.onClick.AddListener(EnableCustomization);
        closeCustomizationButton.onClick.AddListener(DisableCustomization);
        toggleHatLeftButton.onClick.AddListener(PreviousHatCustomization);
        toggleHatRightButton.onClick.AddListener(NextHatCustomization);
        toggleColorLeftButton.onClick.AddListener(PreviousColorCustomization);
        toggleColorRightButton.onClick.AddListener(NextColorCustomization);
    }

    private void OnDestroy()
    {
        enableCustomizationButton.onClick.RemoveListener(EnableCustomization);
        closeCustomizationButton.onClick.RemoveListener(DisableCustomization);
        toggleHatLeftButton.onClick.RemoveListener(PreviousHatCustomization);
        toggleHatRightButton.onClick.RemoveListener(NextHatCustomization);
        toggleColorLeftButton.onClick.RemoveListener(PreviousColorCustomization);
        toggleColorRightButton.onClick.RemoveListener(NextColorCustomization);
    }

    private void EnableCustomization()
    {
        //Camera
        if (cam.TryGetComponent<Animator>(out Animator anim)) anim.applyRootMotion = true;
        cam.transform.parent = RealtimeSingletonWeb.instance.LocalPlayer.Anim.transform;
        cam.transform.localPosition = playerCustomizationCameraTransform.position;
        cam.transform.localRotation = playerCustomizationCameraTransform.rotation;
        cam.nearClipPlane = playerCustomizationNearClipPlane;

        SetNonLocalClientPlayerVisibility(false);

        //Enable UI components
        if(controllerCanvas) controllerCanvas.enabled = false;
        toggleHatLeftButton.gameObject.SetActive(true);
        toggleHatRightButton.gameObject.SetActive(true);
        toggleColorLeftButton.gameObject.SetActive(true);
        toggleColorRightButton.gameObject.SetActive(true);
        closeCustomizationButton.gameObject.SetActive(true);
        enableCustomizationButton.gameObject.SetActive(false);

        RealtimeSingletonWeb.instance.LocalPlayer.SetPlayerNameVisibility(false);

        OnCustomizationEnabled.Invoke();
    }

    public void DisableCustomization()
    {
        //Camera
        cam.transform.parent = null;
        cam.transform.position = defaultCameraTransform.position;
        cam.transform.rotation = defaultCameraTransform.rotation;
        cam.nearClipPlane = 0.3f;
        if (cam.TryGetComponent<Animator>(out Animator anim)) anim.applyRootMotion = false;

        SetNonLocalClientPlayerVisibility(true);

        //Disable UI components
        if(controllerCanvas) controllerCanvas.enabled = true;
        toggleHatLeftButton.gameObject.SetActive(false);
        toggleHatRightButton.gameObject.SetActive(false);
        toggleColorLeftButton.gameObject.SetActive(false);
        toggleColorRightButton.gameObject.SetActive(false);
        closeCustomizationButton.gameObject.SetActive(false);
        enableCustomizationButton.gameObject.SetActive(true);

        RealtimeSingletonWeb.instance.LocalPlayer.SetPlayerNameVisibility(true);

        OnCustomizationDisabled.Invoke();
    }

    void SetNonLocalClientPlayerVisibility(bool visible)
    {
        foreach (ClientPlayer cp in ClientPlayer.clients)
        {
            if(!cp.IsLocal)
            {
                foreach (Transform t in cp.gameObject.GetComponentsInChildren<Transform>())
                {
                    t.gameObject.layer = visible ? LayerMask.NameToLayer("ClientPlayer") : LayerMask.NameToLayer("VROnly");
                }
            }
        }
    }

    private void NextHatCustomization()
    {
        if (RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex < RealtimeSingletonWeb.instance.LocalPlayer.hats.Length-1)
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex++;
        }
        else
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex = -1;
        }
    }

    private void PreviousHatCustomization()
    {
        if (RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex >= 0)
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex--;
        }
        else
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex = RealtimeSingletonWeb.instance.LocalPlayer.hats.Length-1;
        }
    }

    private void NextColorCustomization()
    {
        int colIndex = RealtimeSingletonWeb.instance.LocalPlayer.GetColorIndex();
        Texture2D pallete = RealtimeSingletonWeb.instance.LocalPlayer.colorPalette;

        if(colIndex < pallete.width)
        {
            colIndex++;
        }
        else
        {
            colIndex = 0;
        }

        RealtimeSingletonWeb.instance.LocalPlayer.syncer.Color = pallete.GetPixel(colIndex, 0);
    }

    private void PreviousColorCustomization()
    {
        int colIndex = RealtimeSingletonWeb.instance.LocalPlayer.GetColorIndex();
        Texture2D pallete = RealtimeSingletonWeb.instance.LocalPlayer.colorPalette;

        if (colIndex > 0)
        {
            colIndex--;
        }
        else
        {
            colIndex = pallete.width-1;
        }

        RealtimeSingletonWeb.instance.LocalPlayer.syncer.Color = pallete.GetPixel(colIndex, 0);
    }
}
