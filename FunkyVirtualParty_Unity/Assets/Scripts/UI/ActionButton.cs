using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.Layouts;

public class ActionButton : MonoBehaviour
{
    [InputControl(layout = "Button")]
    [SerializeField]
    private string m_ControlPath;

    void Start()
    {
        if(TryGetComponent<Button>(out Button b))
        {
            b.onClick.AddListener(ClientManagerWeb.instance.ActionButtonPressed);
        }
    }
}