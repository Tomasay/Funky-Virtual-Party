using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.SceneManagement;
using Autohand;
using Normal.Realtime;

public class ShootoutGameManager : MonoBehaviour
{
    private const int COUNTDOWN_AMOUNT = 3, GAME_TIME_AMOUNT = 30;
    private const int FIREBALLS_AMOUNT = 8;
    private TMP_Text vrInfoText, vrGameTimeText;
    private float timeRemaining;

    //Debug player variables
    [SerializeField] private Transform[] debugWaypoints;
    private Vector3[] currentWaypoints;
    private float[] currentWaypointDistances;

    void Start()
    {
        timeRemaining = GAME_TIME_AMOUNT;

        ShootoutGameSyncer.instance.OnStateChangeEvent.AddListener(OnStateChanged);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;

        //Spawn fireballs
        for (int i = 0; i < FIREBALLS_AMOUNT; i++)
        {
            Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
            options.ownedByClient = true;
            Realtime.Instantiate("IcyIgnition/Fireball", Vector3.zero, Quaternion.identity, options);
        }

        Invoke("InitializeWaypoints", 3); //Give debug client players a second to spawn
    }

    void InitializeWaypoints()
    {
        currentWaypoints = new Vector3[ClientPlayer.debugPlayerCount];
        currentWaypointDistances = new float[ClientPlayer.debugPlayerCount];
    }

    private void OnDestroy()
    {
        ShootoutGameSyncer.instance.OnStateChangeEvent.RemoveListener(OnStateChanged);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        vrInfoText = avatar.GetComponent<ShootoutGameVRPlayerController>().vrInfoText;

        vrGameTimeText = avatar.GetComponent<ShootoutGameVRPlayerController>().vrGameTimeText;
        vrGameTimeText.text = FormatTime(timeRemaining);

        SetVRPlayerMovement(false);
        SetVRPlayerCanThrowFireballs(false);
    }

    void Update()
    {
        switch (ShootoutGameSyncer.instance.State)
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
                    ShootoutGameSyncer.instance.State = "time ended";
                    vrGameTimeText.GetComponent<Animator>().SetBool("Pulsate", false);
                }

                UpdateDebugPlayers();

                break;
            case "vr player won":
                break;
            case "time ended":
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
                foreach (ShootoutGameClientPlayer cp in ShootoutGameClientPlayer.clients)
                {
                    cp.syncer.OnDeath.AddListener(CheckPlayersLeft);
                }
                break;
            case "game loop":
                SetVRPlayerCanThrowFireballs(true);
                break;
            case "vr player won":
                StartCoroutine(GameOver(4, "YOU WIN!"));
                break;
            case "time ended":
                StartCoroutine(GameOver(4, "TIMES UP!\nYOU LOSE"));
                break;
            default:
                break;
        }
    }

    void CheckPlayersLeft()
    {
        int playersAlive = 0;
        foreach (ShootoutGameClientPlayer cp in ShootoutGameClientPlayer.clients)
        {
            if (cp.isAlive)
            {
                playersAlive++;
            }
        }

        if (playersAlive == 0)
        {
            ShootoutGameSyncer.instance.State = "vr player won";
        }
    }

    IEnumerator StartCountdownTimer(int countdown)
    {
        yield return new WaitForSeconds(4);

        for (int i = countdown; i > 0; i--)
        {
            vrInfoText.text = "" + i;
            yield return new WaitForSeconds(1);
        }

        vrInfoText.text = "GO!";

        SetVRPlayerCanThrowFireballs(true);

        yield return new WaitForSeconds(1);
        vrInfoText.text = "";

        ShootoutGameSyncer.instance.State = "game loop";
    }

    IEnumerator GameOver(int delay, string txt)
    {
        vrInfoText.text = txt;
        yield return new WaitForSeconds(delay);

        //Destroy fireballs
        if (Fireball.pool != null)
        {
            foreach (Fireball f in Fireball.pool)
            {
                Realtime.Destroy(f.gameObject);
            }
            Fireball.pool.Clear();
            Fireball.pool = null;
        }

        //Destroy holes
        if (Fireball.holes != null)
        {
            foreach (GameObject h in Fireball.holes)
            {
                Realtime.Destroy(h.gameObject);
            }
            Fireball.holes.Clear();
        }

        SceneChangerSyncer.instance.CurrentScene = "MainMenu";
    }

    private void SetVRPlayerCanThrowFireballs(bool canThrow)
    {
        RealtimeSingleton.instance.VRAvatar.GetComponent<ShootoutGameVRPlayerController>().canThrowFireballs = canThrow;
    }

    protected void SetVRPlayerMovement(bool canPlayerMove)
    {
        RealtimeSingleton.instance.VRAvatar.GetComponentInChildren<AutoHandPlayer>().useMovement = canPlayerMove;
    }

    private void UpdateDebugPlayers()
    {
        for (int i = 0; i < ClientPlayer.clients.Count; i++)
        {
            if (ClientPlayer.clients[i].syncer.IsDebugPlayer)
            {
                UpdateDebugPlayerWaypoint(ClientPlayer.clients[i] as ShootoutGameClientPlayer, i);
            }
        }
    }

    private void UpdateDebugPlayerWaypoint(ShootoutGameClientPlayer cp, int index)
    {
        //If player does not have a waypoint, or has made it to their waypoint
        if (currentWaypoints != null && (currentWaypoints[index] == Vector3.zero || Vector3.Distance(cp.transform.position, currentWaypoints[index]) < 0.1f))
        {
            //Get new waypoint
            currentWaypoints[index] = debugWaypoints[Random.Range(0, debugWaypoints.Length)].position;
            currentWaypointDistances[index] = Vector3.Distance(currentWaypoints[index], cp.transform.position);
            
        }
        //Else move towards next waypoint
        else if(currentWaypoints != null)
        {
            Vector3 dir3 = (currentWaypoints[index] - cp.transform.position).normalized;
            Vector2 dir2 = new Vector2(dir3.x, dir3.z) * 0.25f;

            //Ease in & out speed
            /*
            float dist = Vector3.Distance(currentWaypoints[index], cp.transform.position);
            dir2 *= Mathf.Clamp((dist / currentWaypointDistances[index]) * 2, 0.25f, 0.5f);
            */

            cp.Move(dir2);
        }
    }

    void OnDrawGizmos()
    {
        if(currentWaypoints != null)
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(currentWaypoints[0], 0.1f);
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}