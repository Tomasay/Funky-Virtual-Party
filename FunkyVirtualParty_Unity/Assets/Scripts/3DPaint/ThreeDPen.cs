using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Autohand;
using UnityEngine.Events;
using UnityEngine.Animations;

#if !UNITY_WEBGL
using FMODUnity;
#endif

public class ThreeDPen : ImmediateModeShapeDrawer
{
    List<List<PolylinePath>> drawingLines;

    List<PolylinePath> currentDrawingLines;

    PolylinePath currentLine;

    Color currentColor = Color.black;

    [SerializeField]
    Transform linesParent;

    [SerializeField]
    Transform tip;

    [SerializeField]
    MeshRenderer tipMesh, baseMesh;

    [SerializeField]
    MeshSyncer tipMeshSyncer, baseMeshSyncer;

    [SerializeField]
    ParentConstraint constraint;

    [SerializeField]
    PaintPalette palette;

    [SerializeField]
    Collider col, tipCol;

    [SerializeField]
    Rigidbody rb;

#if !UNITY_WEBGL
    [SerializeField]
    ThreeDPaintGameManager gm;
#endif

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

        //Set Draw defaults for Polylines
        Draw.BlendMode = ShapesBlendMode.Opaque;
        Draw.Thickness = 0.01f;
        Draw.PolylineGeometry = PolylineGeometry.Billboard;
        Draw.DetailLevel = DetailLevel.Minimal;
        Draw.PolylineJoins = PolylineJoins.Round;

        Draw.Position = linesParent.transform.position;

        drawingLines = new List<List<PolylinePath>>();
        currentDrawingLines = new List<PolylinePath>();

        ShapesMaterialUtils.Prewarm();
    }

    private void Start()
    {
        VRtistrySyncer.instance.StartedDrawing.AddListener(delegate { CreateNewLine();  isPainting = true; });
        VRtistrySyncer.instance.StoppedDrawing.AddListener(delegate { isPainting = false; });
        VRtistrySyncer.instance.penEnabledChanged.AddListener(SetActive);
        VRtistrySyncer.instance.penColorChanged.AddListener(ChangeColor);

#if !UNITY_WEBGL
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
#endif
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.StartedDrawing.RemoveListener(delegate { CreateNewLine(); isPainting = true; });
        VRtistrySyncer.instance.StoppedDrawing.RemoveListener(delegate { isPainting = false; });
        VRtistrySyncer.instance.penEnabledChanged.RemoveListener(SetActive);
        VRtistrySyncer.instance.penColorChanged.RemoveListener(ChangeColor);

#if !UNITY_WEBGL
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
#endif

        //Dispose of all polylines
        //Dispose current lines
        foreach (List<PolylinePath> ppl in drawingLines)
        {
            foreach (PolylinePath pp in ppl)
            {
                pp.ClearAllPoints();
                pp.Dispose();
            }
        }
        currentLine.ClearAllPoints();
        currentLine.Dispose();
    }

#if !UNITY_WEBGL
    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        //Setup default constraint
        ConstraintSource newSource = new ConstraintSource();
        newSource.sourceTransform = avatar.GetComponent<VRtistryVRPlayerController>().rightHandGrabPoint;
        newSource.weight = 1;
        constraint.AddSource(newSource);
        constraint.constraintActive = true;
    }
#endif

    void Update()
    {
#if !UNITY_WEBGL
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

        if(Draw.Position != linesParent.transform.position)
        {
            Draw.Position = linesParent.transform.position;
        }
    }

#if !UNITY_WEBGL
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

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam, UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingOpaques))
        {
            //Draw any previous lines in the drawing
            if (currentDrawingLines != null && currentDrawingLines.Count > 0)
            {
                foreach (PolylinePath pp in currentDrawingLines)
                {
                    if (pp.Count > 1)
                    {
                        Draw.Polyline(pp, closed: false, thickness: 0.01f); // Drawing happens here
                    }
                }
            }
            //Draw current line being drawn
            if (currentLine != null && currentLine.Count > 1)
            {
                Draw.Polyline(currentLine, closed: false, thickness: 0.01f); // Drawing happens here
            }
        }
    }

    private void CreateNewLine()
    {
        //If previous line had points in it, it needs to be added to the list for current drawing
        if(currentLine != null && currentLine.Count > 0)
        {
            currentDrawingLines.Add(currentLine);
        }

        //Instantiate new line
        currentLine = new PolylinePath();
    }

    private void AddNewLinePoint()
    {
        Vector3 pos = tip.position - linesParent.position;
        if (currentLine.Count == 0 || Vector3.Distance(currentLine.LastPoint.point, pos) > 0.001f)
        {
            currentLine.AddPoint(pos, currentColor);
            lastPointTime = Time.time;
        }
    }

    public void SaveCurrentDrawingLines()
    {
        drawingLines.Add(currentDrawingLines);
    }

    public void EraseAllCurrentLines()
    {
        //Dispose current lines
        foreach (PolylinePath pp in currentDrawingLines)
        {
            pp.ClearAllPoints();
            pp.Dispose();
        }
        currentLine.ClearAllPoints();
        currentLine.Dispose();
    }

    public void ChangeColor(Color c)
    {
#if !UNITY_WEBGL
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
#if !UNITY_WEBGL
        tipMeshSyncer.Enabled = active;
        baseMeshSyncer.Enabled = active;
        col.enabled = active;
        tipCol.enabled = active;
        this.active = active;
#endif
#if UNITY_WEBGL
        this.active = active;
#endif
    }

    public void SetMeshVisibility(bool visible)
    {
        tipMeshSyncer.Enabled = visible;
        baseMeshSyncer.Enabled = visible;
    }
}