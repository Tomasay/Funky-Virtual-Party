using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Normal.Realtime;

public class MazeGameManager : MonoBehaviour
{
    [SerializeField] GameObject maze;

    [SerializeField] Collider[] mazeIgnoreColliders;

    [SerializeField] Collider marbleCollider;

    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 30;
    private TMP_Text vrInfoText, vrGameTimeText;
    private float timeRemaining;

    //Debug player variables
    [SerializeField] private Transform[] debugWaypoints;

    void Start()
    {
        timeRemaining = GAME_TIME_AMOUNT;

        MazeGameSyncer.instance.OnStateChangeEvent.AddListener(OnStateChanged);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
    }

    private void OnDestroy()
    {
        MazeGameSyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChanged);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        MazeGameVRPlayerController player = avatar.GetComponent<MazeGameVRPlayerController>();

        vrInfoText = player.vrInfoText;

        vrGameTimeText = player.vrGameTimeText;
        vrGameTimeText.text = FormatTime(timeRemaining);

        foreach (Collider col in player.vrIgnoreColliders)
        {
            foreach (Collider col2 in mazeIgnoreColliders)
            {
                Physics.IgnoreCollision(col, col2);
            }

            Physics.IgnoreCollision(col, marbleCollider);
        }

        //SetVRPlayerMovement(false);
    }

    void Update()
    {
        switch (MazeGameSyncer.instance.State)
        {
            case "tutorial":
                break;
            case "countdown":
                break;
            case "game loop":
                timeRemaining -= Time.deltaTime;
                vrGameTimeText.text = FormatTime(timeRemaining);

                if (timeRemaining <= 5) //Display final 5 seconds for VR player
                {
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", true);
                }
                if (timeRemaining <= 0) //Game end, VR player wins
                {
                    MazeGameSyncer.instance.State = "vr player lost";
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                }
                break;
            case "vr player won":
                break;
            case "vr player lost":
                break;
            default:
                break;
        }
    }

    void OnStateChanged(string newState)
    {
        switch (newState)
        {
            case "tutorial":
                break;
            case "countdown":
                StartCoroutine("StartCountdownTimer", COUNTDOWN_AMOUNT);

                //Setup client death events. Waiting till countdown to ensure all clients have been spawned in
                foreach (MazeGameClientPlayer cp in MazeGameClientPlayer.clients)
                {
                    cp.syncer.OnDeath.AddListener(CheckPlayersLeft);
                }
                break;
            case "game loop":

                break;
            case "vr player won":
                StartCoroutine(GameOver(2, "YOU WIN!"));
                break;
            case "vr player lost":
                StartCoroutine(GameOver(2, "TIMES UP!\nYOU LOSE"));
                break;
            default:
                break;
        }
    }

    void CheckPlayersLeft()
    {
        int playersAlive = 0;
        foreach (MazeGameClientPlayer cp in MazeGameClientPlayer.clients)
        {
            if (cp.isAlive)
            {
                playersAlive++;
            }
        }

        if (playersAlive == 0)
        {
            MazeGameSyncer.instance.State = "vr player won";
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        yield return new WaitForSeconds(2);

        for (int i = countdown; i > 0; i--)
        {
            vrInfoText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        vrInfoText.text = "GO!";

        yield return new WaitForSeconds(1);
        vrInfoText.text = "";

        MazeGameSyncer.instance.State = "game loop";
    }

    IEnumerator GameOver(int countdown, string txt)
    {
        vrInfoText.text = txt;
        yield return new WaitForSeconds(3);

        SceneChangerSyncer.instance.CurrentScene = "MainMenu";
    }

    protected void SetVRPlayerMovement(bool canPlayerMove)
    {
        RealtimeSingleton.instance.VRAvatar.GetComponentInChildren<AutoHandPlayer>().useMovement = canPlayerMove;
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}