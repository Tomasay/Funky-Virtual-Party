using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public class KeyboardController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void CreateDummyInput();

    [DllImport("__Internal")]
    private static extern void OpenInputKeyboard();

    [DllImport("__Internal")]
    private static extern void CloseInputKeyboard();

    [DllImport("__Internal")]
    private static extern void OnKeyboardInput();

    [DllImport("__Internal")]
    private static extern void UpdateInputFieldText(string txt);

    [SerializeField] TMP_InputField nameField, codeField;

    private TMP_InputField currentField;

    public ClientManagerWeb manager;

    [SerializeField] private TMP_Text debugText;

    private void Start()
    {
        CreateDummyInput();
    }

    private void Update()
    {
        //If input field is active, update it accordingly
        if (currentField)
        {
            currentField.ActivateInputField();
            currentField.caretPosition = currentField.text.Length;
        }
    }

    public void OpenKeyboard()
    {
        OpenInputKeyboard();
    }

    public void CloseKeyboard()
    {
        CloseInputKeyboard();
        if (currentField)
        {
            currentField.DeactivateInputField();
            currentField = null;
        }
    }

    public void UpdateText(string input)
    {
        currentField.text = input;

        if (currentField == codeField)
        {
            codeField.text = currentField.text.ToUpper();
        }
    }

    public void SetField(TMP_InputField f)
    {
        currentField = f;
        UpdateInputFieldText(f.text);
    }

    public void SubmitButtonPressed()
    {
        CloseKeyboard();
        manager.AttemptJoinRoom(codeField.text, nameField.text);
    }
}