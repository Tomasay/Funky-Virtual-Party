using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedLogoManager : MonoBehaviour
{
    public static AnimatedLogoManager instance;

    [SerializeField]
    GameObject blueDrawIntro;

    [SerializeField]
    GameObject[] inOutloop; //Start with blueOut, end with blueIn

    [SerializeField]
    GameObject[] eraseAnimations; //Start with blue, end with pink

    [SerializeField]
    int stateIdleTime = 3;

    int index = -1; //-1 = blueDrawIntro

    bool erased = false;

    private void Start()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void Next()
    {
        if (erased) return;

        if (index % 2 == 0)
        {
            NextDelayed();
        }
        else
        {
            Invoke("NextDelayed", stateIdleTime);
        }
    }

    void NextDelayed()
    {
        if (erased) return;

        //Disable previous state
        if (index == -1)
        {
            blueDrawIntro.SetActive(false);
        }
        else
        {
            inOutloop[index].SetActive(false);
        }

        //Enable new one
        index = (index >= inOutloop.Length - 1) ? 0 : index + 1;
        inOutloop[index].SetActive(true);
    }

    public void EraseOut()
    {
        if (index == 0 || index == 9) //Blue
        {
            inOutloop[index].SetActive(false);
            eraseAnimations[0].SetActive(true);
        }
        else if (index == 1 || index == 2) //Red
        {
            inOutloop[index].SetActive(false);
            eraseAnimations[1].SetActive(true);
        }
        else if (index == 3 || index == 4) //Green
        {
            inOutloop[index].SetActive(false);
            eraseAnimations[2].SetActive(true);
        }
        else if (index == 5 || index == 6) //Orange
        {
            inOutloop[index].SetActive(false);
            eraseAnimations[3].SetActive(true);
        }
        else if (index == 7 || index == 8) //Pink
        {
            inOutloop[index].SetActive(false);
            eraseAnimations[4].SetActive(true);
        }

        erased = true;
    }
}