using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class KeyboardController : MonoBehaviour
{
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void CreateDummyInput();

        [DllImport("__Internal")]
        private static extern void CloseInputKeyboard();

        [DllImport("__Internal")]
        private static extern void OnKeyboardInput();

        [DllImport("__Internal")]
        private static extern void UpdateInputFieldText(string txt);

        [DllImport("__Internal")]
        private static extern void SetPointerDownOnButton(bool isDown);

        [DllImport("__Internal")]
        private static extern string GetNameData();

        [DllImport("__Internal")]
        private static extern string CheckURLPartyCode();
#endif
    

    [SerializeField] TMP_InputField nameField, codeField;
    [SerializeField] ButtonEvents nameFieldButton, codeFieldButton;

    private TMP_InputField currentField;

    [SerializeField] private TMP_Text debugText;

    private void Start()
    {
        //CreateDummyInput();

#if UNITY_EDITOR
        nameFieldButton.gameObject.SetActive(false);
        codeFieldButton.gameObject.SetActive(false);
#elif UNITY_WEBGL
        nameFieldButton.onPointerDown.AddListener(ButtonPointerDown);
        nameFieldButton.onPointerUp.AddListener(ButtonPointerUp);
        nameFieldButton.onPointerUp.AddListener(delegate { SetField(nameField); });
        codeFieldButton.onPointerDown.AddListener(ButtonPointerDown);
        codeFieldButton.onPointerUp.AddListener(ButtonPointerUp);
        codeFieldButton.onPointerUp.AddListener(delegate { SetField(codeField); });

        codeField.onValueChanged.AddListener(delegate { RealtimeSingletonWeb.instance.CheckValidPartyCode(codeField.text); });

        string storedName = GetNameData();
        if (storedName != null)
        {
            nameField.text = GetNameData();
        }

        string URLCode = CheckURLPartyCode();
        if(URLCode != null)
        {
            codeField.text = URLCode;
        }
#endif
    }


    private void Update()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        //If input field is active, update it accordingly
        if (currentField)
        {
            currentField.ActivateInputField();
            currentField.caretPosition = currentField.text.Length;
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame && currentField == nameField)
        {
            SetField(codeField);
        }

        if (currentField == nameField && currentField.text.Length > 12)
        {
            currentField.text = currentField.text.Substring(0, 12);
        }
        else if (currentField == codeField && currentField.text.Length > 4)
        {
            currentField.text = currentField.text.Substring(0, 4);
        }
        //Debug.Log("Updating dummy field with " + currentField.gameObject.name + ": " + currentField.text);
        UpdateInputFieldText(currentField.text);

#elif UNITY_EDITOR && UNITY_WEBGL
        if (Keyboard.current.tabKey.wasPressedThisFrame && nameField.isFocused)
        {
            codeField.Select();
        }
#endif
    }


    public void ButtonPointerDown()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        SetPointerDownOnButton(true);
#endif
    }

    public void ButtonPointerUp()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        StartCoroutine("ButtonDown");
#endif
    }

    IEnumerator ButtonDown()
    {
        yield return new WaitForSeconds(1);
#if !UNITY_EDITOR && UNITY_WEBGL
        SetPointerDownOnButton(false);
#endif
    }

    public void CloseKeyboard()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        CloseInputKeyboard();
        if (currentField)
        {
            currentField.DeactivateInputField();
            currentField = null;
        }
#endif
    }

    public void UpdateText(string input)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        currentField.text = input;

        if (currentField == codeField)
        {
            codeField.text = currentField.text.ToUpper();
        }
#endif
    }

    public void SetField(TMP_InputField f)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        currentField = f;
        UpdateInputFieldText(f.text);
#endif
    }
}