using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

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
        if (correctAnswerBanner)
        {
            correctAnswerBannerInitialScale = (correctAnswerBanner.transform as RectTransform).localScale.x;
        }
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
            (i.transform as RectTransform).localScale = Vector3.zero;
            (i.transform as RectTransform).localPosition = Vector3.zero;
            i.GetComponentsInChildren<Image>()[1].color = Color.white;
            i.GetComponentInChildren<TMP_Text>().color = Color.black;
            i.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            i.GetComponent<LayoutElement>().ignoreLayout = false;
            i.gameObject.SetActive(false);
        }
    }

    public void AddPlayerIcon(int id)
    {
        ClientPlayer cp = ClientPlayer.GetClientByCurrentOwnerID(id);

        //Find next player icon available
        foreach (Image i in playerIcons)
        {
            if(!i.gameObject.activeSelf)
            {
                i.gameObject.SetActive(true);

                i.gameObject.name = "" + id;

                //Set color
                i.color = cp.syncer.Color;

                //Set player name text
                i.GetComponentInChildren<TMP_Text>().text = cp.syncer.Name;

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
            yield return new WaitForSeconds(2);
            AnimateScores();
        }
    }

    public void TestPositioning()
    {
        foreach (Image i in playerIcons)
        {
            if (i.gameObject.activeSelf)
            {
                Vector3 localPos = (i.transform as RectTransform).localPosition;
                i.GetComponent<LayoutElement>().ignoreLayout = true;
                (i.transform as RectTransform).localPosition = localPos;
            }
        }
    }

    public void AnimateScores()
    {
        StartCoroutine("AnimateScoresCoroutine");
    }

    IEnumerator AnimateScoresCoroutine()
    {
        //Scale player icon down
        foreach (Image i in playerIcons)
        {
            if (i.gameObject.activeSelf)
            {
                (playerIcons[0].transform as RectTransform).DOScale(0, 0.25f);
                yield return new WaitForSeconds(0.25f);
            }
        }

        //Get intial positions for names
        List<Vector3> initialPositions = new List<Vector3>();
        foreach (Image i in playerIcons)
        {
            initialPositions.Add((i.transform as RectTransform).localPosition);
        }

        //Convert to score and scale back up
        for (int i = 0; i < playerIcons.Length; i++)
        {
            if (playerIcons[i].gameObject.activeSelf)
            {
                SetImageAlpha(playerIcons[i].GetComponentsInChildren<Image>()[0], 0);
                SetImageAlpha(playerIcons[i].GetComponentsInChildren<Image>()[1], 0);
                playerIcons[i].GetComponentInChildren<TMP_Text>().text = "" + ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS;
                playerIcons[i].GetComponentInChildren<TMP_Text>().color = Color.green;
                playerIcons[i].GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;

                playerIcons[i].GetComponent<LayoutElement>().ignoreLayout = true;
                (playerIcons[i].transform as RectTransform).localPosition = initialPositions[i];

                (playerIcons[0].transform as RectTransform).DOScale(1, 0.25f);

                yield return new WaitForSeconds(0.25f);
            }
        }

        yield return new WaitForSeconds(1);

        //Translate to corresponding player
        foreach (Image i in playerIcons)
        {
            if (i.gameObject.activeSelf)
            {
                ClientPlayer cp = ClientPlayer.GetClientByCurrentOwnerID(int.Parse(i.gameObject.name));
                (playerIcons[0].transform as RectTransform).DOMove(cp.transform.position, 1);
                i.GetComponentInChildren<TMP_Text>().DOColor(Color.clear, 1);
            }
        }
    }

    /*
     IEnumerator AnimateScoresCoroutine()
    {
        Image i = playerIcons[0];
        (playerIcons[0].transform as RectTransform).DOScale(0, 0.25f);

        yield return new WaitForSeconds(0.25f);

        SetImageAlpha(i.GetComponentsInChildren<Image>()[0], 0);
        SetImageAlpha(i.GetComponentsInChildren<Image>()[1], 0);
        i.GetComponentInChildren<TMP_Text>().text = "500";
        i.GetComponentInChildren<TMP_Text>().color = Color.green;
        i.GetComponent<LayoutElement>().ignoreLayout = true;

        (playerIcons[0].transform as RectTransform).DOScale(1, 0.25f);

        yield return new WaitForSeconds(2);

        (playerIcons[0].transform as RectTransform).DOMove(testPos.position, 1);
        i.GetComponentInChildren<TMP_Text>().DOColor(Color.clear, 1);
    }
     * */

    void SetImageAlpha(Image i, float a)
    {
        Color col = i.color;
        col.a = a;
        i.color = col;
    }
}
