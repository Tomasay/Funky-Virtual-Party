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

    private Vector3 initialCameraPos;
    private Quaternion initialCameraRot;

    void Start()
    {
        initialCameraPos = Camera.main.transform.position;
        initialCameraRot = Camera.main.transform.rotation;

        enableCustomizationButton.onClick.AddListener(EnableCustomization);
        closeCustomizationButton.onClick.AddListener(DisableCustomization);
        toggleHatLeftButton.onClick.AddListener(PreviousHatCustomization);
        toggleHatRightButton.onClick.AddListener(NextHatCustomization);
        toggleColorLeftButton.onClick.AddListener(PreviousColorCustomization);
        toggleColorRightButton.onClick.AddListener(NextColorCustomization);
    }

    private void EnableCustomization()
    {
        //Camera
        Camera.main.transform.parent = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
        Camera.main.transform.localPosition = new Vector3(0, 5, 10);
        Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(15, 180, 0));

        //Enable UI components
        controllerCanvas.enabled = false;
        toggleHatLeftButton.gameObject.SetActive(true);
        toggleHatRightButton.gameObject.SetActive(true);
        toggleColorLeftButton.gameObject.SetActive(true);
        toggleColorRightButton.gameObject.SetActive(true);
        closeCustomizationButton.gameObject.SetActive(true);
        enableCustomizationButton.gameObject.SetActive(false);
        backgroundDots.SetActive(false);
    }

    private void DisableCustomization()
    {
        //Camera
        Camera.main.transform.parent = null;
        Camera.main.transform.position = initialCameraPos;
        Camera.main.transform.rotation = initialCameraRot;

        //Disable UI components
        controllerCanvas.enabled = true;
        toggleHatLeftButton.gameObject.SetActive(false);
        toggleHatRightButton.gameObject.SetActive(false);
        toggleColorLeftButton.gameObject.SetActive(false);
        toggleColorRightButton.gameObject.SetActive(false);
        closeCustomizationButton.gameObject.SetActive(false);
        enableCustomizationButton.gameObject.SetActive(true);
        backgroundDots.SetActive(true);
    }

    private void NextHatCustomization()
    {
        if (ClientManagerWeb.instance.LocalPlayer.PlayerHatIndex < ClientManagerWeb.instance.LocalPlayer.hats.Length)
        {
            ClientManagerWeb.instance.LocalPlayer.PlayerHatIndex++;
        }
        else
        {
            ClientManagerWeb.instance.LocalPlayer.PlayerHatIndex = 0;
        }
    }

    private void PreviousHatCustomization()
    {
        if (ClientManagerWeb.instance.LocalPlayer.PlayerHatIndex > 0)
        {
            ClientManagerWeb.instance.LocalPlayer.PlayerHatIndex--;
        }
        else
        {
            ClientManagerWeb.instance.LocalPlayer.PlayerHatIndex = ClientManagerWeb.instance.LocalPlayer.hats.Length;
        }
    }

    private void NextColorCustomization()
    {
        int colIndex = ClientManagerWeb.instance.LocalPlayer.GetColorIndex();
        Texture2D pallete = ClientManagerWeb.instance.LocalPlayer.colorPalette;

        if(colIndex < pallete.width)
        {
            colIndex++;
        }
        else
        {
            colIndex = 0;
        }

        ClientManagerWeb.instance.LocalPlayer.UpdateColor(pallete.GetPixel(colIndex, 0));
    }

    private void PreviousColorCustomization()
    {
        int colIndex = ClientManagerWeb.instance.LocalPlayer.GetColorIndex();
        Texture2D pallete = ClientManagerWeb.instance.LocalPlayer.colorPalette;

        if (colIndex > 0)
        {
            colIndex--;
        }
        else
        {
            colIndex = pallete.width;
        }

        ClientManagerWeb.instance.LocalPlayer.UpdateColor(pallete.GetPixel(colIndex, 0));
    }
}
