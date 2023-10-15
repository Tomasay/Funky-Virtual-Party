using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientPlayerCustomizer : MonoBehaviour
{
    [SerializeField] Button toggleHatLeftButton, toggleHatRightButton, toggleColorLeftButton, toggleColorRightButton;
    [SerializeField] Button enableCustomizationButton, closeCustomizationButton;
    [SerializeField] Canvas controllerCanvas;
    [SerializeField] GameObject backgroundDots;
    [SerializeField] Camera cam;

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
        cam.transform.parent = RealtimeSingletonWeb.instance.LocalPlayer.Anim.transform;
        cam.transform.localPosition = new Vector3(0, 5, 10);
        cam.transform.localRotation = Quaternion.Euler(new Vector3(15, 180, 0));

        //Enable UI components
        controllerCanvas.enabled = false;
        toggleHatLeftButton.gameObject.SetActive(true);
        toggleHatRightButton.gameObject.SetActive(true);
        toggleColorLeftButton.gameObject.SetActive(true);
        toggleColorRightButton.gameObject.SetActive(true);
        closeCustomizationButton.gameObject.SetActive(true);
        enableCustomizationButton.gameObject.SetActive(false);
        backgroundDots.SetActive(false);

        RealtimeSingletonWeb.instance.LocalPlayer.SetPlayerNameVisibility(false);
    }

    private void DisableCustomization()
    {
        //Camera
        cam.transform.parent = null;
        cam.transform.position = new Vector3(0, 24, -20);
        cam.transform.rotation = Quaternion.Euler(new Vector3(45, 0, 0));

        //Disable UI components
        controllerCanvas.enabled = true;
        toggleHatLeftButton.gameObject.SetActive(false);
        toggleHatRightButton.gameObject.SetActive(false);
        toggleColorLeftButton.gameObject.SetActive(false);
        toggleColorRightButton.gameObject.SetActive(false);
        closeCustomizationButton.gameObject.SetActive(false);
        enableCustomizationButton.gameObject.SetActive(true);
        backgroundDots.SetActive(true);

        RealtimeSingletonWeb.instance.LocalPlayer.SetPlayerNameVisibility(true);
    }

    private void NextHatCustomization()
    {
        if (RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex < RealtimeSingletonWeb.instance.LocalPlayer.hats.Length)
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex++;
        }
        else
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex = 0;
        }
    }

    private void PreviousHatCustomization()
    {
        if (RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex > 0)
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex--;
        }
        else
        {
            RealtimeSingletonWeb.instance.LocalPlayer.syncer.HatIndex = RealtimeSingletonWeb.instance.LocalPlayer.hats.Length;
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
            colIndex = pallete.width;
        }

        RealtimeSingletonWeb.instance.LocalPlayer.syncer.Color = pallete.GetPixel(colIndex, 0);
    }
}
