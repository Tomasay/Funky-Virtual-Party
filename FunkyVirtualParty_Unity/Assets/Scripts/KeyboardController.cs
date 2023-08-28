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
        private static extern void StoreNameData(string name);

        [DllImport("__Internal")]
        private static extern string GetNameData();

        [DllImport("__Internal")]
        private static extern string CheckURLPartyCode();
#endif
    

    [SerializeField] TMP_InputField nameField, codeField;
    [SerializeField] ButtonEvents nameFieldButton, codeFieldButton;

    private TMP_InputField currentField;

    public ClientManagerWeb manager;

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

#elif UNITY_EDITOR && UNITY_WEBGL
        if (Keyboard.current.tabKey.wasPressedThisFrame && nameField.isFocused)
        {
            codeField.Select();
        }
#endif
    }

#if !UNITY_EDITOR && UNITY_WEBGL
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
        manager.AttemptJoinRoom(codeField.text.ToUpper(), nameField.text);
        StoreNameData(nameField.text);
    }
#endif

}