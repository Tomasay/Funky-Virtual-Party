using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    void Start()
    {
        if(TryGetComponent<Button>(out Button b))
        {
            b.onClick.AddListener(ClientManagerWeb.instance.ActionButtonPressed);
        }
    }
}