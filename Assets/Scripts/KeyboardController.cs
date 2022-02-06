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

    public TMP_InputField field;

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
        field.text = input;
    }

    public void SetField(TMP_InputField f)
    {
        field = f;
        UpdateInputFieldText(f.text);
    }
}