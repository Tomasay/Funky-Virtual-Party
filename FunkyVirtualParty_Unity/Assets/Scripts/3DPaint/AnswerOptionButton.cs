using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Normal.Realtime;

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
    public Vector3 initialScale;
    private float correctAnswerBannerInitialScale;

    //The player who wrote this answer
    public string playerID;

    public TMP_Text AnswerText { get => answerText;}

    private void Awake()
    {
        initialScale = (transform as RectTransform).localScale;
        if (correctAnswerBanner)
        {
            correctAnswerBannerInitialScale = (correctAnswerBanner.transform as RectTransform).localScale.x;
        }
    }

    public void SetText(string txt)
    {
        answerText.text = txt;
    }

    public void SetTextWithPlayerName(string txt, ClientPlayer client)
    {
        answerText.text = "<size=80%><color=\"grey\">" + client.syncer.Name + ":</color></size>\n" + txt;
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

        //Animate in bubble
        canvasGroup.alpha = 1;
        (transform as RectTransform).localScale = Vector3.zero;
        (transform as RectTransform).DOScale(initialScale, 1).SetEase(Ease.OutElastic, 1.25f);

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

    public void AnimateScores()
    {
        StartCoroutine("AnimateScoresCoroutine");
    }

    IEnumerator AnimateScoresCoroutine()
    {
        for (int i = 0; i < playerIcons.Length; i++)
        {
            if (playerIcons[i].gameObject.activeSelf)
            {
                //Scale player icon down
                (playerIcons[i].transform as RectTransform).DOScale(0, 0.25f);
                yield return new WaitForSeconds(0.25f);

                //Convert to score and scale back up
                SetImageAlpha(playerIcons[i].GetComponentsInChildren<Image>()[0], 0);
                SetImageAlpha(playerIcons[i].GetComponentsInChildren<Image>()[1], 0);
                playerIcons[i].GetComponentInChildren<TMP_Text>().text = "" + ThreeDPaintGlobalVariables.POINTS_CLIENT_CORRECT_GUESS;
                playerIcons[i].GetComponentInChildren<TMP_Text>().color = Color.green;
                playerIcons[i].GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;

                (playerIcons[i].transform as RectTransform).DOScale(1.5f, 0.25f);

                yield return new WaitForSeconds(0.25f);
            }
        }

        yield return new WaitForSeconds(1);

        //Ignore layout so they can be translated. Has to be done all at once before any translations to avoid layout rearrangement
        foreach (Image i in playerIcons)
        {
            if (i.gameObject.activeSelf)
            {
                i.GetComponent<LayoutElement>().ignoreLayout = true;
            }
        }

        //Translate to corresponding player
        foreach (Image i in playerIcons)
        {
            if (i.gameObject.activeSelf)
            {
                ClientPlayer cp = ClientPlayer.GetClientByCurrentOwnerID(int.Parse(i.gameObject.name));
                (i.transform as RectTransform).DOMove(cp.transform.position, 1);
                i.GetComponentInChildren<TMP_Text>().DOColor(Color.clear, 1);

                yield return new WaitForSeconds(0.5f);

                float nameHeight = cp.playerNameText.transform.localPosition.y;
                cp.playerNameText.DOColor(Color.green, 0.25f);
                cp.playerNameText.transform.DOLocalMoveY(nameHeight + 5, 0.25f);

                yield return new WaitForSeconds(0.25f);

                cp.playerNameText.DOColor(Color.black, 0.25f);
                cp.playerNameText.transform.DOLocalMoveY(nameHeight, 0.25f);
            }
        }
    }

    void SetImageAlpha(Image i, float a)
    {
        Color col = i.color;
        col.a = a;
        i.color = col;
    }
}
