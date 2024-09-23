using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Normal.Realtime;
using DG.Tweening;

public class MazeGameManager : MonoBehaviour
{
    [SerializeField] GameObject maze;

    [SerializeField] Collider[] mazeIgnoreColliders;

    [SerializeField] Collider marbleCollider;

    [SerializeField] MeshRenderer leftHandle, rightHandle;

    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 30;
    private TMP_Text vrInfoText, vrGameTimeText;
    private float timeRemaining;

    private const int MAX_COINS = 4; //Max amount of coins on screen at one time
    private const int COINS_SPAWN_DELAY = 3; //Amount of time it takes another coin to spawn once one has been collected

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

    Tweener leftHandleTweener, rightHandleTweener;
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

        //Maze handle pulsing
        if (!leftHandleGrabbed)
        {
            Debug.Log("outline width: " + leftHandle.material.GetFloat("_OutlineWidth"));
            if (leftHandle.material.GetFloat("_OutlineWidth") == 0)
            {
                leftHandleTweener = leftHandle.material.DOFloat(0.75f, "_OutlineWidth", 1);
            }
            else if (leftHandle.material.GetFloat("_OutlineWidth") == 0.75f)
            {
                leftHandleTweener = leftHandle.material.DOFloat(0, "_OutlineWidth", 1);
            }
        }

        if (!rightHandleGrabbed)
        {
            if (rightHandle.material.GetFloat("_OutlineWidth") == 0)
            {
                rightHandleTweener = rightHandle.material.DOFloat(0.75f, "_OutlineWidth", 1);
            }
            else if (rightHandle.material.GetFloat("_OutlineWidth") == 0.75f)
            {
                rightHandleTweener = rightHandle.material.DOFloat(0, "_OutlineWidth", 1);
            }
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

    /*
    void SpawnCoins()
    {
        for (int i = 0; i < MAX_COINS; i++)
        {
            Transform t = GetValidCoinSpawnLocation();

            if (t)
            {
                Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
                options.ownedByClient = true;
                Realtime.Instantiate("MazeGame/Coin", t.position, t.rotation, options);
            }
        }
    }

    Transform GetValidCoinSpawnLocation()
    {
        //Check if max amount of coins have been spawned
        int coinsSpawned = 0;
        List<Transform> validSpawnLocations = new List<Transform>();
        foreach (KeyValuePair<Transform, MazeCoin> entry in spawnedCoins)
        {
            if(entry.Value)
            {
                coinsSpawned++;
            }
            else
            {
                validSpawnLocations.Add(entry.Key);
            }
        }

        if (coinsSpawned == MAX_COINS) //If yes, return null
        {
            return null;
        }
        else //If not, get a random valid transform for a new spawn
        {
            return validSpawnLocations[Random.Range(0, validSpawnLocations.Count)];
        }
    }
    */

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

    bool leftHandleGrabbed, rightHandleGrabbed;
    public void OnMazeGrabbed(Autohand.Hand h, Grabbable g)
    {
        if(h.left) leftHandleGrabbed = true;
        if(!h.left) rightHandleGrabbed = true;

        if (leftHandleGrabbed && rightHandleGrabbed)
        {
            MazeGameSyncer.instance.VRPlayerReady = true;
            SetVRPlayerMovement(false);

            leftHandleTweener.Kill();
            leftHandle.material.SetFloat("_OutlineWidth", 0);
            rightHandleTweener.Kill();
            rightHandle.material.SetFloat("_OutlineWidth", 0);
        }
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