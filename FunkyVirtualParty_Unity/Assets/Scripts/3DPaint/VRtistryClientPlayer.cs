using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;

public class VRtistryClientPlayer : ClientPlayer
{
    [SerializeField] public Vector3[] spawnRotations;

    [SerializeField] public Button playerButton;

    [SerializeField] public AnswerOptionButton playerAnswer;

    [SerializeField] public MeshSyncer[] phones; //Phone meshes corresponding to each phone anim

    [SerializeField] RectTransform playerNameIndicatorArrow;

    protected override void LocalStart()
    {
        base.LocalStart();

        foreach (MeshSyncer ms in phones)
        {
            ms.GetComponent<RealtimeView>().RequestOwnership();
        }

        SetSpawnRotation();
        Invoke("SetupTextBubbleTransforms", 3);
    }

    protected override void Start()
    {
        base.Start();

        //Was having an occasional bug where player canvases would billboard to the wrong camera (mostly on IOS?)
        //Not entirely sure why that was happening, but this should prevent that
        Invoke("SetBubbleBillboardCamera", 1);

        SetPlayerNameTextPosition();
    }

    void SetBubbleBillboardCamera()
    {
        GameObject tryCamera = GameObject.Find("DrawingPhaseCamera");
        if (tryCamera && tryCamera.TryGetComponent(out Camera cam))
        {
            foreach (FaceCamera fc in GetComponentsInChildren<FaceCamera>())
            {
                fc.cameraToLookAt = cam;
            }        
        }
    }

    void SetupTextBubbleTransforms()
    {
        foreach (ClientPlayer cp in clients)
        {
            VRtistryClientPlayer vcp = (cp as VRtistryClientPlayer);
            RectTransform rt = (vcp.playerAnswer.transform as RectTransform);
            Vector3 pos = rt.localPosition;
            float scale = rt.localScale.x;

            if (clients.Count > 4)
            {
                switch ((vcp.realtimeView.ownerIDSelf - 1))
                {
                    case 0: //Back Right
                        scale = 0.14f;
                        break;
                    case 1: //Front Right
                        break;
                    case 2: //Back Left
                        pos.y = 75;
                        scale = 0.14f;
                        break;
                    case 3: //Front Left
                        pos.y = 80;
                        break;
                    case 4: //Back Middle Right
                        pos.y = 75;
                        scale = 0.14f;
                        break;
                    case 5: //Front Middle Right
                        pos.y = 80;
                        break;
                    case 6: //Back Left
                        scale = 0.14f;
                        break;
                    case 7: //Front Left
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch ((vcp.realtimeView.ownerIDSelf - 1))
                {
                    case 0: //Back Right
                        scale = 0.2f;
                        break;
                    case 1: //Front Right
                        pos.x = -15;
                        scale = 0.15f;
                        break;
                    case 2: //Back Left
                        scale = 0.2f;
                        break;
                    case 3: //Front Left
                        pos.x = 15;
                        scale = 0.15f;
                        break;
                    default:
                        break;
                }
            }

            rt.localPosition = pos;
            rt.localScale = new Vector3(scale, scale, scale);
        }
    }

    void SetPlayerNameTextPosition()
    {
        int id = realtimeView.ownerIDSelf - 1;

#if UNITY_WEBGL
        //Position
        if (id == 0 || id == 6)
        {
            playerNameText.rectTransform.localPosition = new Vector3(0, -175, 0);
            playerNameIndicatorArrow.Rotate(0, 0, 180);
        }
        else if (id == 1 || id == 7)
        {
            playerNameText.rectTransform.localPosition = new Vector3(0, -100, -25);
            playerNameIndicatorArrow.Rotate(0, 0, 180);
        }

        //Font size
        if (id%2 != 0)
        {
            playerNameText.fontSize = 28;
        }
#elif UNITY_ANDROID
        //Position
        if (id == 0 || id == 6 || id == 1 || id == 7)
        {
            playerNameText.rectTransform.localPosition = new Vector3(0, 35, 0);
        }
        playerNameText.fontSize = 28;
#endif

    }

    public void SetSitAnim()
    {
        animSyncer.Trigger = "Sit1";
    }

    [HideInInspector]
    public int usingPhone;
    public void TogglePhone()
    {
        if(usingPhone == 0)
        {
            usingPhone = Random.Range(1, 4);
            animSyncer.SetBool = "UsingPhone" + usingPhone + " True";
            animSyncer.AnimOffset = Random.Range(0.0f, 1.0f);
            phones[usingPhone - 1].Enabled = true;
        }
        else
        {
            animSyncer.SetBool = "UsingPhone" + usingPhone + " False";
            phones[usingPhone - 1].Enabled = false;
            usingPhone = 0;
        }
    }

    protected void SetSpawnRotation()
    {
        if (syncer.IsDebugPlayer)
        {
            transform.rotation = Quaternion.Euler(spawnRotations[debugPlayerIndex - 9]);
            return;
        }

        transform.rotation = Quaternion.Euler(spawnRotations[realtimeView.ownerIDSelf - 1]);
    }

    public void SetButtonInteractable(bool interactable)
    {
        playerButton.gameObject.SetActive(interactable);
    }
}