using UnityEngine;
using TMPro;

public class KeyboardActivator : MonoBehaviour
{
    private TMP_InputField field;

    void Start()
    {
        field = GetComponent<TMP_InputField>();
    }

    public void ActivateKeyboard()
    {
        field.ActivateInputField();
        field.caretPosition = field.text.Length;
    }
}