using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;
using DG.Tweening;

public class VRtistryClientPlayer : ClientPlayer
{
    [SerializeField] public Vector3[] spawnRotations;

    [SerializeField] public Button playerButton;

    [SerializeField] public AnswerOptionButton playerAnswer;

    [SerializeField] public Image playerAnswerArrow;

    [SerializeField] public MeshSyncer[] phones; //Phone meshes corresponding to each phone anim

    [SerializeField] RectTransform playerNameIndicatorArrow;

    [SerializeField] public Camera faceDrawCam;

    [SerializeField] FaceBillboarder faceBillboarder;

    [SerializeField] MeshRenderer faceMesh;

    [SerializeField] Shader faceShader;

    protected override void Awake()
    {
#if UNITY_WEBGL
        billboardNameText = false;
        Destroy((playerAnswer.transform as RectTransform).GetComponent<FaceCamera>());
#endif

        base.Awake();
    }

    protected override void LocalStart()
    {
        base.LocalStart();

        foreach (MeshSyncer ms in phones)
        {
            ms.GetComponent<RealtimeView>().RequestOwnership();
        }

        SetSpawnRotation();
    }

    protected override void Start()
    {
        base.Start();

#if UNITY_ANDROID
        //Was having an occasional bug where player canvases would billboard to the wrong camera (mostly on IOS?)
        //Not entirely sure why that was happening, but this should prevent that
        Invoke("SetBubbleBillboardCamera", 1);
#endif

        Invoke("SetupTextBubbleTransforms", 3);

        SetPlayerNameTextPosition();

        faceBillboarder.forwardDirection = (realtimeView.ownerIDSelf % 2 == 0) ? ForwardDirection.Left : ForwardDirection.Right;
        syncer.OnFaceDrawingChangedEvent.AddListener(OnFaceDrawingChanged);

        // Give each player a unique stencil ID so their face only renders on top of their own body
        Material[] bodyMats = smr.materials;
        bodyMats[1].SetInt("_StencilRef", realtimeView.ownerIDSelf + 1);
        smr.materials = bodyMats;

        // If this player already has face drawing data (e.g. we joined after they drew their face),
        // ApplyFaceMat won't be triggered by the event. Apply it after ClientSync's 1s ApplyFace delay.
        if (syncer.FaceDrawing != null && syncer.FaceDrawing.Length > 0)
        {
            Invoke("ApplyFaceMat", 1.2f);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        syncer.OnFaceDrawingChangedEvent.RemoveListener(OnFaceDrawingChanged);
    }

    void OnFaceDrawingChanged()
    {
        faceMesh.transform.localScale = Vector3.zero;
        Invoke("ApplyFaceMat", 0.1f);
    }

    void ApplyFaceMat()
    {
        Material[] mats = faceMesh.materials;
        Texture prevTex = mats[0].mainTexture;
        Material faceMat = new Material(faceShader != null ? faceShader : Shader.Find("Custom/UnlitAlphaOnTop")) { mainTexture = prevTex };
        int stencilRef = realtimeView.ownerIDSelf + 1;
        faceMat.SetInt("_StencilRef", stencilRef);
        mats[0] = faceMat;
        mats[1].SetInt("_StencilRef", stencilRef);
        faceMesh.materials = mats;
        faceMesh.transform.DOScale(1, 0.25f);
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
        int id = realtimeView.ownerIDSelf - 1;

        RectTransform rt = (playerAnswer.transform as RectTransform);
        RectTransform arrowRt = (playerAnswerArrow.transform as RectTransform);

#if UNITY_WEBGL
        //Scale/rot
        if (id % 2 == 0)
        {
            playerAnswer.initialScale = rt.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            rt.Rotate(45, 0, 0);
        }
        else
        {
            playerAnswer.initialScale = rt.localScale = new Vector3(0.12f, 0.12f, 0.12f);
            rt.Rotate(45, 180, 0);
        }

        //Pos
        if (id == 0 || id == 6) //Back right and back middle left
        {
            rt.localPosition = new Vector3((id == 0) ? 10 : -10, -115, -175);
        }
        else if (id == 1 || id == 7) //Front right and front middle left
        {
            rt.localPosition = new Vector3((id == 1) ? 10 : 0, -80, 90);
        }
        else if (id == 2 || id == 4) //Back left and back middle right
        {
            rt.localPosition = new Vector3((id == 2) ? -10 : 10, -20, 0);
        }

        //Arrow
        if (id == 1 || id == 7 || id == 0 || id == 6)
        {
            arrowRt.localPosition = new Vector3(0, 225, 0);
            arrowRt.Rotate(0, 0, 180);
        }
#elif UNITY_ANDROID
        //Pos/arrow scale
        if (id == 0 || id == 6 || id == 1 || id == 7) //Back right and back middle left, Front right and front middle left
        {
            rt.localPosition = new Vector3(0, -110, -110);
            arrowRt.localScale = new Vector3(3.5f, 2.5f, 2.5f);
        }

        //Arrow pos/rot
        if (id == 0 || id == 6)
        {
            arrowRt.localPosition = new Vector3(-360, 170, 0);
            arrowRt.localRotation = Quaternion.Euler(0, 0, 135);
        }
        else if (id == 1 || id == 7)
        {
            arrowRt.localPosition = new Vector3(360, 170, 0);
            arrowRt.localRotation = Quaternion.Euler(0, 0, 45);
        }
#endif
    }

    void SetPlayerNameTextPosition()
    {
        int id = realtimeView.ownerIDSelf - 1;

#if UNITY_WEBGL
        //Position
        if (id == 0 || id == 6)
        {
            playerNameText.rectTransform.localPosition = new Vector3(0, -150, -85);
            playerNameIndicatorArrow.Rotate(0, 0, 180);
        }
        else if (id == 1 || id == 7)
        {
            playerNameText.rectTransform.localPosition = new Vector3((id == 1) ? 10 : 0, -60, 85);
            playerNameIndicatorArrow.Rotate(0, 0, 180);
        }

        //Font size / Rotation
        if (id%2 != 0)
        {
            playerNameText.fontSize = 28;
            playerNameText.rectTransform.localRotation = Quaternion.Euler(45, 180, 0);
        }
        else
        {
            playerNameText.rectTransform.localRotation = Quaternion.Euler(35, 0, 0);
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

    public void SetPlayerNameIndicatorArrowVisibility(bool active)
    {
        playerNameIndicatorArrow.gameObject.SetActive(active);
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