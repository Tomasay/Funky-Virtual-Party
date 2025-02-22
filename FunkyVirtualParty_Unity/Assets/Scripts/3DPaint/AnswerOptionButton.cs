using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AnswerOptionButton : MonoBehaviour
{
    [SerializeField]
    TMP_Text answerText;

    [SerializeField]
    Image answerBG;

    [SerializeField]
    Image[] borders;

    [SerializeField]
    Image[] playerIcons;

    [SerializeField]
    public CanvasGroup canvasGroup;

    public GameObject correctAnswerBanner;
    private float correctAnswerBannerInitialScale;

    //The player who wrote this answer
    public string playerID;

    public TMP_Text AnswerText { get => answerText;}

    private void Awake()
    {
        correctAnswerBannerInitialScale = (correctAnswerBanner.transform as RectTransform).localScale.x;
    }

    public void SetText(string txt)
    {
        answerText.text = txt;
    }

    public void SetColor(Color col)
    {
        answerBG.color = col;
    }

    public void SetBorderColor(Color col)
    {
        foreach (Image i in borders)
        {
            i.color = col;
        }
    }

    /// <summary>
    /// Returns number of active player icons
    /// </summary>
    public int GetNumberOfPlayers()
    {
        int numOfPlayerIconsActive = 0;
        foreach (Image i in playerIcons)
        {
            if (i.gameObject.activeSelf)
            {
                numOfPlayerIconsActive++;
            }
        }
        return numOfPlayerIconsActive;
    }

    public void ResetPlayerIcons()
    {
        foreach (Image i in playerIcons)
        {
            i.gameObject.SetActive(false);
            (i.transform as RectTransform).localScale = Vector3.zero;
        }
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

    public void AnimateAnswers(int delay)
    {
        StartCoroutine("AnimateAnswersCoroutine", delay);
    }

    IEnumerator AnimateAnswersCoroutine(int delay)
    {
        yield return new WaitForSeconds(delay);

        canvasGroup.alpha = 1;
        (transform as RectTransform).parent.localScale = Vector3.zero;
        (transform as RectTransform).parent.DOScale(1, 1).SetEase(Ease.OutElastic, 1.25f);

        yield return new WaitForSeconds(1);

        foreach (Image i in playerIcons)
        {
            if (i.gameObject.activeSelf)
            {
                (i.transform as RectTransform).DOScale(1, 1).SetEase(Ease.OutElastic, 1.25f);
            }
            yield return new WaitForSeconds(0.25f);
        }

        if(correctAnswerBanner.activeSelf)
        {
            (correctAnswerBanner.transform as RectTransform).DOScale(correctAnswerBannerInitialScale, 1).SetEase(Ease.OutElastic, 1.25f);
        }
    }

    public void AnimateScores()
    {

    }
}
