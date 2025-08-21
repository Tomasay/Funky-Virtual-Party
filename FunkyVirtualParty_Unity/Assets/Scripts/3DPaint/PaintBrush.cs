using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Autohand;
using UnityEngine.Events;
using UnityEngine.Animations;
using Normal.Realtime;
using PaintIn3D;
using DG.Tweening;

#if !UNITY_WEBGL
using FMODUnity;
#endif

public class PaintBrush : ImmediateModeShapeDrawer
{
    Color currentColor = Color.black;

    [SerializeField]
    P3dPaintSphere paintSphere;

    [SerializeField]
    Transform tip;

    [SerializeField]
    MeshRenderer tipMesh, baseMesh;

    [SerializeField]
    MeshSyncer tipMeshSyncer, baseMeshSyncer;

    [SerializeField]
    public ParentConstraint constraint;

    [SerializeField]
    public Collider col, tipCol;

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    RealtimeTransform realtimeTransform;

    [SerializeField]
    public P3DPaintSyncer paintSyncer;

#if !UNITY_WEBGL
    [HideInInspector]
    public ThreeDPaintGameManager gm;
#endif

    Transform linesParent;

    bool isPaintingAir;

    bool isInHand;

    public bool active;

    const int maxPointCount = 100000;
    int currentPointCount;

    private bool canPaintAir = false;

    public bool IsInHand { get => isInHand; set => isInHand = value; }
    public bool CanPaintAir { get => canPaintAir; set { canPaintAir = value; if (!value) { isPaintingAir = false; if (HapticsManager.instance) { HapticsManager.instance.StopHaptics(true); HapticsManager.instance.StopHaptics(false); } } } }

    public Rigidbody Rb { get => rb; }
    public RealtimeTransform RealtimeTransform { get => realtimeTransform; }
    public Transform LinesParent { get => linesParent; set { linesParent = value; Draw.Position = linesParent.position; } }

    public UnityEvent OnDraw;

    private const float LINE_THICKNESS = 0.01f;
    private const float NEW_POINT_DISTANCE_THRESHOLD = 0.001f;

    private void Awake()
    {
        tipMesh.material.color = currentColor;

        //Set Draw defaults for Polylines
        Draw.BlendMode = ShapesBlendMode.Opaque;
        Draw.Thickness = LINE_THICKNESS;
        Draw.PolylineGeometry = PolylineGeometry.Billboard;
        Draw.DetailLevel = DetailLevel.Minimal;
        Draw.PolylineJoins = PolylineJoins.Round;

        ShapesMaterialUtils.Prewarm();

#if !UNITY_WEBGL
        VRtistrySyncer.instance.StartedDrawing.AddListener(delegate { CreateNewLine(); });
        VRtistrySyncer.instance.StoppedDrawing.AddListener(delegate { isPaintingAir = false; });
        VRtistrySyncer.instance.brushEnabledChanged.AddListener(SetActive);
        VRtistrySyncer.instance.brushColorChanged.AddListener(ChangeColor);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
#endif
    }

    private void OnDestroy()
    {
#if !UNITY_WEBGL
        VRtistrySyncer.instance.StartedDrawing.RemoveListener(delegate { CreateNewLine(); });
        VRtistrySyncer.instance.StoppedDrawing.RemoveListener(delegate { isPaintingAir = false; });
        VRtistrySyncer.instance.brushEnabledChanged.RemoveListener(SetActive);
        VRtistrySyncer.instance.brushColorChanged.RemoveListener(ChangeColor);

        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
#endif

        //Dispose of all polylines
        //Dispose current lines
        foreach (List<PolylinePath> ppl in DrawingsSyncer.instance.drawingLines)
        {
            foreach (PolylinePath pp in ppl)
            {
                pp.ClearAllPoints();
                pp.Dispose();
            }
        }
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
        if (isPaintingAir && (rb.velocity.magnitude > 0.025f || RealtimeSingleton.instance.VRAvatar.GetComponentInChildren<AutoHandPlayer>().GetComponent<Rigidbody>().velocity.magnitude > 1) && currentPointCount < maxPointCount)
        {
            AddNewLinePoint();
            OnDraw.Invoke();
        }
#endif

        if (linesParent && Draw.Position != linesParent.transform.position)
        {
            Draw.Position = linesParent.transform.position;
        }
    }

#if !UNITY_WEBGL
    public void OnTriggerPressed(Hand h, Grabbable g)
    {
        if (canPaintAir)
        {
            VRtistrySyncer.instance.realtimeView.RequestOwnership();
            VRtistrySyncer.instance.IsDrawing = true;

            HapticsManager.instance.TriggerHaptic(h.left, 999, 0.1f);
        }
    }

    public void OnTriggerReleased(Hand h, Grabbable g)
    {
        if (canPaintAir)
        {
            HapticsManager.instance.StopHaptics(h.left);

            VRtistrySyncer.instance.IsDrawing = false;
        }
    }
#endif

    public override void DrawShapes(Camera cam)
    {
#if UNITY_ANDROID
        if (Camera.main != cam) //Only draw to main camera (avoid warning overlay cam)
        {
            return;
        }
#endif

        using (Draw.Command(cam, UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingOpaques))
        {
            bool isVRPlayerPracticing = (VRtistrySyncer.instance.State == "" || VRtistrySyncer.instance.State == "clients answering");
            if (isVRPlayerPracticing)
            {
                if (DrawingsSyncer.instance.practiceDrawingLines != null && DrawingsSyncer.instance.practiceDrawingLines.Count > 0)
                {
                    foreach (PolylinePath plp in DrawingsSyncer.instance.practiceDrawingLines[DrawingsSyncer.instance.practiceDrawingLines.Count - 1])
                    {
                        if (plp.Count > 1)
                        {
                            Draw.Polyline(plp, closed: false, thickness: LINE_THICKNESS);
                        }
                    }
                }
            }
            else
            {
                if (DrawingsSyncer.instance.drawingLines != null && DrawingsSyncer.instance.drawingLines.Count > 0)
                {
                    if (VRtistrySyncer.instance.State.Equals("gallery") || VRtistrySyncer.instance.State.Equals("game over")) //If in gallery state, render all drawings
                    {
                        foreach (List<PolylinePath> plpList in DrawingsSyncer.instance.drawingLines)
                        {
                            foreach (PolylinePath plp in plpList)
                            {
                                if (plp.Count > 1)
                                {
                                    Draw.Polyline(plp, closed: false, thickness: LINE_THICKNESS);
                                }
                            }
                        }
                    }
                    else //Else, only render current drawing
                    {
                        foreach (PolylinePath plp in DrawingsSyncer.instance.drawingLines[DrawingsSyncer.instance.drawingLines.Count - 1])
                        {
                            if (plp.Count > 1)
                            {
                                Draw.Polyline(plp, closed: false, thickness: LINE_THICKNESS);
                            }
                        }
                    }
                }
            }
        }
    }

    private void CreateNewLine()
    {
        bool isVRPlayerPracticing = (VRtistrySyncer.instance.State == "" || VRtistrySyncer.instance.State == "clients answering");

        int currentDrawing = DrawingsSyncer.instance.Drawings.Count - 1;
        if (currentDrawing >= 0)
        {
            PenStrokeModel newPenStroke = new PenStrokeModel();
            newPenStroke.lineColor = currentColor;

            if (isVRPlayerPracticing)
            {
                DrawingsSyncer.instance.Drawings[(uint)currentDrawing].practicePenStrokes.Add(newPenStroke);
            }
            else
            {
                DrawingsSyncer.instance.Drawings[(uint)currentDrawing].penStrokes.Add(newPenStroke);
            }

            isPaintingAir = true;
        }
    }

    private void AddNewLinePoint()
    {
        Vector3 pos = tip.position - linesParent.position;

        bool isVRPlayerPracticing = (VRtistrySyncer.instance.State == "" || VRtistrySyncer.instance.State == "clients answering");

        PolylinePath currentLine;
        if (isVRPlayerPracticing)
        {
            int i = DrawingsSyncer.instance.practiceDrawingLines.Count - 1;
            int j = DrawingsSyncer.instance.practiceDrawingLines[i].Count - 1;
            currentLine = DrawingsSyncer.instance.practiceDrawingLines[i][j];
        }
        else
        {
            int i = DrawingsSyncer.instance.drawingLines.Count - 1;
            int j = DrawingsSyncer.instance.drawingLines[i].Count - 1;
            currentLine = DrawingsSyncer.instance.drawingLines[i][j];
        }

        if (currentLine.Count == 0 || Vector3.Distance(currentLine.LastPoint.point, pos) > NEW_POINT_DISTANCE_THRESHOLD)
        {
            LinePointModel newLinePoint = new LinePointModel();
            newLinePoint.position = pos;
            int k = DrawingsSyncer.instance.Drawings.Count - 1;

            if (isVRPlayerPracticing)
            {
                int lastPenStrokeIndex = DrawingsSyncer.instance.Drawings[(uint)k].practicePenStrokes.Count - 1;
                DrawingsSyncer.instance.Drawings[(uint)k].practicePenStrokes[lastPenStrokeIndex].linePoints.Add(newLinePoint);
            }
            else
            {
                int lastPenStrokeIndex = DrawingsSyncer.instance.Drawings[(uint)k].penStrokes.Count - 1;
                DrawingsSyncer.instance.Drawings[(uint)k].penStrokes[lastPenStrokeIndex].linePoints.Add(newLinePoint);
            }
        }
    }

    public void ChangeColor(Color c)
    {
#if !UNITY_WEBGL
        if (IsInHand)
        {
            if (!(currentColor == c))
            {
                RuntimeManager.PlayOneShot("event:/SFX/Drop", transform.position);
            }

            paintSphere.Color = c;

            currentColor = c;
            tipMesh.material.DOColor(c, 0.25f);
        }
#endif
#if UNITY_WEBGL
        currentColor = c;
        tipMesh.material.DOColor(c, 0.25f);
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