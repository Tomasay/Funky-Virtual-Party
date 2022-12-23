using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerOptionButton : MonoBehaviour
{
    [SerializeField]
    TMP_Text answerText;

    [SerializeField]
    Image answerBG;

    [SerializeField]
    Image[] playerIcons;

    //The player who wrote this answer
    public string playerID;

    public TMP_Text AnswerText { get => answerText;}

    public void SetText(string txt)
    {
        answerText.text = txt;
    }

    public void SetColor(Color col)
    {
        answerBG.color = col;
    }

    public void AddPlayerIcon(string name, Color col)
    {
        //Find next player icon available
        foreach (Image i in playerIcons)
        {
            if(!i.gameObject.activeSelf)
            {
                i.gameObject.SetActive(true);

                //Set color
                i.color = col;

                //Set player name text
                i.GetComponentInChildren<TMP_Text>().text = name;

                return;
            }
        }
    }
}
