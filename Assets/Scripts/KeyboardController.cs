using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class KeyboardController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void CreateDummyInput();

    [DllImport("__Internal")]
    private static extern void OpenInputKeyboard();

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

    public void OpenKeyboard()
    {
        OpenInputKeyboard();
    }

    public void UpdateText(string input)
    {
        currentField.text = input;
    }

    public void SetField(TMP_InputField f)
    {
        currentField = f;
        UpdateInputFieldText(f.text);
    }

    public void SubmitButtonPressed()
    {
        manager.AttemptJoinRoom(codeField.text, nameField.text);
    }
}