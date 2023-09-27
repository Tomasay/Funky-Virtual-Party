using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Autohand;
using UnityEngine.Events;
using UnityEngine.Animations;

#if UNITY_ANDROID
using FMODUnity;
#endif

public class ThreeDPen : MonoBehaviour
{
    Polyline currentLine;

    Color currentColor = Color.black;

    [SerializeField]
    Transform linesParent;

    [SerializeField]
    Transform tip;

    [SerializeField]
    MeshRenderer tipMesh, baseMesh;

    [SerializeField]
    ParentConstraint constraint;

    [SerializeField]
    PaintPalette palette;

    [SerializeField]
    Collider col, tipCol;

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    ThreeDPaintGameManager gm;

    bool isPainting;

    bool isInHand;

    public bool active;

    //The amount of time that has to pass before another point can be created
    const float pointSecondDelay = 0.01f;

    float lastPointTime;

    const int maxPointCount = 100000;
    int currentPointCount;

    Vector3 lastPenPos;

    private bool canPaint = true;

    public bool IsInHand { get => isInHand; set => isInHand = value; }
    public bool CanPaint { get => canPaint; set { canPaint = value; if (!value) { isPainting = false; if (HapticsManager.instance) { HapticsManager.instance.StopHaptics(true); HapticsManager.instance.StopHaptics(false); } } } }

    public UnityEvent OnDraw;

    private void Awake()
    {
        tipMesh.material.color = currentColor;
    }

    private void Start()
    {
        VRtistrySyncer.instance.StartedDrawing.AddListener(delegate { CreateNewLine();  isPainting = true; });
        VRtistrySyncer.instance.StoppedDrawing.AddListener(delegate { isPainting = false; });
        VRtistrySyncer.instance.penEnabledChanged.AddListener(SetActive);
        VRtistrySyncer.instance.penColorChanged.AddListener(ChangeColor);

#if UNITY_ANDROID
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
#endif
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.StartedDrawing.RemoveListener(delegate { CreateNewLine(); isPainting = true; });
        VRtistrySyncer.instance.StoppedDrawing.RemoveListener(delegate { isPainting = false; });
        VRtistrySyncer.instance.penEnabledChanged.RemoveListener(SetActive);
        VRtistrySyncer.instance.penColorChanged.RemoveListener(ChangeColor);

#if UNITY_ANDROID
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
#endif
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        //Setup default constraint
        ConstraintSource newSource = new ConstraintSource();
        newSource.sourceTransform = avatar.GetComponent<VRtistryVRPlayerController>().rightHandGrabPoint;
        newSource.weight = 1;
        constraint.AddSource(newSource);
        constraint.constraintActive = true;
    }

    void Update()
    {
#if UNITY_ANDROID
        if (isPainting && (rb.velocity.magnitude > 0.025f || RealtimeSingleton.instance.VRAvatar.GetComponentInChildren<AutoHandPlayer>().GetComponent<Rigidbody>().velocity.magnitude > 1) && currentPointCount < maxPointCount)
        {
            AddNewLinePoint();
            OnDraw.Invoke();
        }
#endif
#if UNITY_WEBGL
        if (isPainting && (transform.position - lastPenPos).magnitude > 0.01f && (Time.time - lastPointTime) > pointSecondDelay && currentPointCount < maxPointCount)
        {
            AddNewLinePoint();
            OnDraw.Invoke();
            lastPenPos = transform.position;
        }
#endif
    }

#if UNITY_ANDROID
    public void OnTriggerPressed(Hand h, Grabbable g)
    {
        if (canPaint)
        {
            VRtistrySyncer.instance.realtimeView.RequestOwnership();
            VRtistrySyncer.instance.IsDrawing = true;

            HapticsManager.instance.TriggerHaptic(h.left, 999, 0.1f);
        }
    }

    public void OnTriggerReleased(Hand h, Grabbable g)
    {
        if (canPaint)
        {
            HapticsManager.instance.StopHaptics(h.left);

            VRtistrySyncer.instance.IsDrawing = false;
        }
    }
#endif

    private void CreateNewLine()
    {
        GameObject newLine = new GameObject("line");
        newLine.transform.parent = linesParent;

        Polyline pl = newLine.AddComponent<Polyline>();
        pl.BlendMode = ShapesBlendMode.Opaque;
        pl.Thickness = 0.01f;
        pl.Color = currentColor;
        pl.Geometry = PolylineGeometry.Billboard;
        pl.DetailLevel = DetailLevel.Minimal;
        pl.Joins = PolylineJoins.Round;
        pl.Closed = false;
        pl.SetPoints(new List<PolylinePoint>());

        currentLine = pl;

        lastPenPos = transform.position;

        newLine.SetActive(false);
    }

    private void AddNewLinePoint()
    {
        Vector3 pos = currentLine.transform.InverseTransformPoint(tip.position);
        if (currentLine.Count == 0 || Vector3.Distance(currentLine.points[currentLine.points.Count-1].point, pos) > 0.01f)
        {
            currentLine.AddPoint(pos);
            currentPointCount++;
            lastPointTime = Time.time;
        }

        //Seems to be a bug with Shapes where polylines with less than 2 points render as incorrectly as a triangle...
        if(currentLine.Count > 1)
        {
            currentLine.gameObject.SetActive(true);
        }
    }

    public void EraseAllLines()
    {
        foreach (Polyline pl in linesParent.GetComponentsInChildren<Polyline>())
        {
            Destroy(pl.gameObject);
        }
    }

    public void ChangeColor(Color c)
    {
#if UNITY_ANDROID
        if (IsInHand)
        {
            if(!(currentColor == c))
            {
                RuntimeManager.PlayOneShot("event:/SFX/Drop", transform.position);
            }

            currentColor = c;
            tipMesh.material.color =c;
        }
#endif
#if UNITY_WEBGL
            currentColor = c;
            tipMesh.material.color = c;
#endif
    }

    public void SetActive(bool active)
    {
#if UNITY_ANDROID
        tipMesh.enabled = active;
        baseMesh.enabled = active;
        col.enabled = active;
        tipCol.enabled = active;
        this.active = active;
#endif
#if UNITY_WEBGL
        tipMesh.enabled = active;
        baseMesh.enabled = active;
        this.active = active;
#endif
    }
}