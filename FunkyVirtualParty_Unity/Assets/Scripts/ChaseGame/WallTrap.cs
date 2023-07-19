using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using DG.Tweening;

public class WallTrap : MonoBehaviour
{
    [SerializeField]
    GameObject leftWall, rightWall;

    private bool isOpen = true;

    public void ButtonPressed()
    {
        if(isOpen)
        {
            CloseTrap();
        }
        else
        {
            OpenTrap();
        }
    }

    void OpenTrap()
    {
        isOpen = true;

        RequestOwnership();

        leftWall.transform.DOLocalRotate(new Vector3(0, 0, -75), 1.5f);
        rightWall.transform.DOLocalRotate(new Vector3(0, 0, 75), 1.5f);
    }

    void CloseTrap()
    {
        isOpen = false;

        RequestOwnership();

        leftWall.transform.DOLocalRotate(new Vector3(0, 0, 15), 1.5f);
        rightWall.transform.DOLocalRotate(new Vector3(0, 0, 165), 1.5f);
    }

    void RequestOwnership()
    {
        leftWall.GetComponent<RealtimeView>().RequestOwnership();
        leftWall.GetComponent<RealtimeTransform>().RequestOwnership();
        rightWall.GetComponent<RealtimeView>().RequestOwnership();
        rightWall.GetComponent<RealtimeTransform>().RequestOwnership();
    }
}