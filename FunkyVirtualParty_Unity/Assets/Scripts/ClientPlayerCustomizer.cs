using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientPlayerCustomizer : MonoBehaviour
{
    [SerializeField] NormcoreRoomConnector norm;

    [SerializeField] Button toggleHatLeftButton, toggleHatRightButton, toggleColorLeftButton, toggleColorRightButton;
    [SerializeField] Button enableCustomizationButton, closeCustomizationButton;
    [SerializeField] Canvas controllerCanvas;
    [SerializeField] GameObject backgroundDots;
    [SerializeField] Camera cam;

    private Vector3 initialCameraPos;
    private Quaternion initialCameraRot;

    void Start()
    {
        initialCameraPos = cam.transform.position;
        initialCameraRot = cam.transform.rotation;

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
        cam.transform.parent = norm.LocalPlayer.Anim.transform;
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

        norm.LocalPlayer.SetPlayerNameVisibility(false);
    }

    private void DisableCustomization()
    {
        //Camera
        cam.transform.parent = null;
        cam.transform.position = initialCameraPos;
        cam.transform.rotation = initialCameraRot;

        //Disable UI components
        controllerCanvas.enabled = true;
        toggleHatLeftButton.gameObject.SetActive(false);
        toggleHatRightButton.gameObject.SetActive(false);
        toggleColorLeftButton.gameObject.SetActive(false);
        toggleColorRightButton.gameObject.SetActive(false);
        closeCustomizationButton.gameObject.SetActive(false);
        enableCustomizationButton.gameObject.SetActive(true);
        backgroundDots.SetActive(true);

        norm.LocalPlayer.SetPlayerNameVisibility(true);
    }

    private void NextHatCustomization()
    {
        if (norm.LocalPlayer.syncer.HatIndex < norm.LocalPlayer.hats.Length)
        {
            norm.LocalPlayer.syncer.HatIndex++;
        }
        else
        {
            norm.LocalPlayer.syncer.HatIndex = 0;
        }
    }

    private void PreviousHatCustomization()
    {
        if (norm.LocalPlayer.syncer.HatIndex > 0)
        {
            norm.LocalPlayer.syncer.HatIndex--;
        }
        else
        {
            norm.LocalPlayer.syncer.HatIndex = norm.LocalPlayer.hats.Length;
        }
    }

    private void NextColorCustomization()
    {
        int colIndex = norm.LocalPlayer.GetColorIndex();
        Texture2D pallete = norm.LocalPlayer.colorPalette;

        if(colIndex < pallete.width)
        {
            colIndex++;
        }
        else
        {
            colIndex = 0;
        }

        norm.LocalPlayer.syncer.Color = pallete.GetPixel(colIndex, 0);
    }

    private void PreviousColorCustomization()
    {
        int colIndex = norm.LocalPlayer.GetColorIndex();
        Texture2D pallete = norm.LocalPlayer.colorPalette;

        if (colIndex > 0)
        {
            colIndex--;
        }
        else
        {
            colIndex = pallete.width;
        }

        norm.LocalPlayer.syncer.Color = pallete.GetPixel(colIndex, 0);
    }
}
