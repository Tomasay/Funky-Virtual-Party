using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class ThreeDPaintGameManagerWeb : GameManagerWeb
{
    [DllImport("__Internal")]
    private static extern void OpenInputKeyboard();

    [DllImport("__Internal")]
    private static extern void CloseInputKeyboard();

    [DllImport("__Internal")]
    private static extern void OnKeyboardInput();

    [DllImport("__Internal")]
    private static extern void UpdateInputFieldText(string txt);

    [DllImport("__Internal")]
    private static extern void SetPointerDownOnButton(bool isDown);

    [SerializeField]
    TMP_Text headerText;

    [SerializeField]
    Canvas inputCanvas;

    [SerializeField]
    GameObject playerInputParent;

    [SerializeField]
    TMP_InputField answerInputField;

    [SerializeField]
    ButtonEvents answerInputButton;

    [SerializeField]
    KeyboardActivator answerInputActivator;

    [SerializeField]
    Button submitButton;

    bool typingAnswer = false;

    private void Awake()
    {
        answerInputButton.onPointerDown.AddListener(ButtonPointerDown);
        answerInputButton.onPointerUp.AddListener(ButtonPointerUp);

        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
    }

    void Update()
    {
        if (typingAnswer)
        {
            answerInputField.ActivateInputField();
            answerInputField.caretPosition = answerInputField.text.Length;
        }
    }

    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("SetNewPrompt"))
        {
            headerText.text = data; //Set prompt text
            playerInputParent.SetActive(true); //Enable input
        }
        if (methodName.Equals("ShowBlurredView"))
        {
            inputCanvas.enabled = false;
        }
    }

    public void OpenKeyboard()
    {
        typingAnswer = true;
        OpenInputKeyboard();
    }

    public void UpdateText(string input)
    {
        answerInputField.text = input;
    }

    public void ButtonPointerDown()
    {
        SetPointerDownOnButton(true);
    }

    public void ButtonPointerUp()
    {
        StartCoroutine("ButtonDown");
    }

    IEnumerator ButtonDown()
    {
        yield return new WaitForSeconds(1);
        SetPointerDownOnButton(false);
    }

    public void CheckAnswerInput(string input)
    {
        submitButton.interactable = (input.Length > 0);
    }

    public void SetField(TMP_InputField f)
    {
        UpdateInputFieldText(f.text);
    }

    public void SubmitAnswer()
    {
        if (ClientManagerWeb.instance) ClientManagerWeb.instance.Manager.Socket.Emit("InfoFromPlayer", answerInputField.text);

        typingAnswer = false;
        CloseInputKeyboard();
        answerInputField.DeactivateInputField();

        headerText.text = "Waiting for all players to submit their answer...";
        playerInputParent.SetActive(false);
    }
}