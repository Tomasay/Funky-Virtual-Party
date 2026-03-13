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
using NaughtyAttributes;

#if !UNITY_WEBGL
using FMODUnity;
#endif

public class PaintBrush : ImmediateModeShapeDrawer
{
    public bool canAirAndCollisionPaintAtSameTime = false;

    Color currentColor = Color.black;

    [SerializeField]
    P3dPaintSphere paintSphere;

    [SerializeField]
    P3dHitBetween paintHitBetween;

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

    public P3dPaintableTexture paintTexture;

#if !UNITY_WEBGL
    [HideInInspector]
    public VRtistryGameManager gm;
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

    public UnityEvent OnRevealAnimationComplete;

    bool revealAirPaintComplete, revealCollisionPaintComplete, revealAnimationComplete;

    public bool RevealAnimationComplete { get => revealAnimationComplete; }

    private const float LINE_THICKNESS = 0.01f;
    private const float NEW_POINT_DISTANCE_THRESHOLD = 0.001f;
    private const float REVEAL_ANIMATION_SPEED = 0.01f;

    private void Awake()
    {
        OnRevealAnimationComplete = new UnityEvent();

        tipMesh.material.color = currentColor;

        //Set Draw defaults for Polylines
        Draw.BlendMode = ShapesBlendMode.Opaque;
        Draw.Thickness = LINE_THICKNESS;
        Draw.PolylineGeometry = PolylineGeometry.Billboard;
        Draw.DetailLevel = DetailLevel.Minimal;
        Draw.PolylineJoins = PolylineJoins.Round;

        ShapesMaterialUtils.Prewarm();

        VRtistrySyncer.instance.brushColorChanged.AddListener(ChangeColor);

#if !UNITY_WEBGL
        VRtistrySyncer.instance.StartedDrawing.AddListener(delegate {CreateNewLine(); if (!canAirAndCollisionPaintAtSameTime) { paintHitBetween.enabled = false; } });
        VRtistrySyncer.instance.StoppedDrawing.AddListener(delegate { isPaintingAir = false; if (!canAirAndCollisionPaintAtSameTime) { paintHitBetween.enabled = true; } });
        VRtistrySyncer.instance.brushEnabledChanged.AddListener(SetActive);
        
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
#endif
    }

    private void OnDestroy()
    {
        VRtistrySyncer.instance.brushColorChanged.RemoveListener(ChangeColor);

#if !UNITY_WEBGL
        VRtistrySyncer.instance.StartedDrawing.RemoveListener(delegate { CreateNewLine(); if (!canAirAndCollisionPaintAtSameTime) { paintHitBetween.enabled = false; } });
        VRtistrySyncer.instance.StoppedDrawing.RemoveListener(delegate { isPaintingAir = false; if (!canAirAndCollisionPaintAtSameTime) { paintHitBetween.enabled = true; } });
        VRtistrySyncer.instance.brushEnabledChanged.RemoveListener(SetActive);
        
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

        if (cam.CompareTag("UI Camera")) return;

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

    public void AnimatePaintingReveal()
    {
        StartCoroutine("AnimatePaintLines");
        StartCoroutine("AnimatePaintTexture");
    }


    IEnumerator AnimatePaintLines()
    {
        // Cache refs to avoid deep indexing in hot loops
        var drawings = DrawingsSyncer.instance.drawingLines;
        if (drawings == null || drawings.Count == 0)
        {
            revealAirPaintComplete = true;
            CheckRevealAnimationComplete();
            yield break;
        }

        var lines = drawings[drawings.Count - 1];
        if (lines == null || lines.Count == 0)
        {
            revealAirPaintComplete = true;
            CheckRevealAnimationComplete();
            yield break;
        }

        int pointCount = DrawingsSyncer.instance.GetCurrentDrawingLinesPointCount();
        if (pointCount <= 0)
        {
            revealAirPaintComplete = true;
            CheckRevealAnimationComplete();
            yield break;
        }


        float animSpeed = REVEAL_ANIMATION_SPEED;
        float timeItWillTake = pointCount * REVEAL_ANIMATION_SPEED;

        if (timeItWillTake > 3)
        {
            animSpeed = 3.0f / ((float)pointCount);
        }

        //Make all points transparent
        for (int i = 0; i < DrawingsSyncer.instance.drawingLines[DrawingsSyncer.instance.drawingLines.Count - 1].Count; i++)
        {
            for (int j = 0; j < DrawingsSyncer.instance.drawingLines[DrawingsSyncer.instance.drawingLines.Count - 1][i].Count; j++)
            {
                PolylinePoint p = DrawingsSyncer.instance.drawingLines[DrawingsSyncer.instance.drawingLines.Count - 1][i][j];
                Color col = p.color;
                col.a = 0;
                p.color = col;
                DrawingsSyncer.instance.drawingLines[DrawingsSyncer.instance.drawingLines.Count - 1][i][j] = p;
            }
        }

        yield return new WaitForSeconds(0.5f);

        //Set them back to opaque with a delay in between
        float pointsPerSecond = 1f / animSpeed;
        int li = 0;      // line index
        int pj = 0;      // point index within current line
        float accumulator = 0f;

        while (li < lines.Count)
        {
            accumulator += pointsPerSecond * Time.deltaTime;

            int toReveal = Mathf.FloorToInt(accumulator);
            if (toReveal > 0)
            {
                accumulator -= toReveal;

                while (toReveal > 0 && li < lines.Count)
                {
                    var line = lines[li];

                    // Reveal current point
                    var p = line[pj];
                    var col = p.color;
                    col.a = 1f;
                    p.color = col;
                    line[pj] = p;
                    lines[li] = line;

                    // Advance indices
                    pj++;
                    if (pj >= line.Count)
                    {
                        pj = 0;
                        li++;
                    }

                    toReveal--;
                }
            }

            // One frame; effective speed governed by pointsPerSecond
            yield return null;
        }

        revealAirPaintComplete = true;
        CheckRevealAnimationComplete();
    }

    IEnumerator AnimatePaintTexture()
    {
        paintTexture.Clear();

        yield return new WaitForSeconds(0.5f);

        // 1) Compute duration cap (seconds per step)
        int steps = paintSyncer.currentHitLineModels.Count;
        if (steps <= 0)
        {
            revealCollisionPaintComplete = true;
            CheckRevealAnimationComplete();

            yield break;
        }

        float animSpeed = REVEAL_ANIMATION_SPEED; // seconds per step
        float planned = steps * REVEAL_ANIMATION_SPEED;
        if (planned > 3f) animSpeed = 3f / steps;

        //Reveal multiple steps per frame if needed
        float stepsPerSecond = 1f / animSpeed;
        float acc = 0f;
        while (paintSyncer.CanRevealAnotherHitLine())
        {
            acc += stepsPerSecond * Time.deltaTime;

            int toApply = Mathf.FloorToInt(acc);
            if (toApply > 0) acc -= toApply;

            // Apply as many redo steps as our time budget allows this frame
            while (toApply-- > 0 && paintSyncer.CanRevealAnotherHitLine())
            {
                paintSyncer.RevealHitLine();
            }

            // If nothing applied (e.g., very slow rate), still progress next frame
            yield return null;
        }

        //Once animation has played using low res texture, update to full res
        //paintTexture.LoadFromData(DrawingsSyncer.instance.CurrentDrawing.paintTexture);

        revealCollisionPaintComplete = true;
        CheckRevealAnimationComplete();
    }

    void CheckRevealAnimationComplete()
    {
        if (revealAirPaintComplete && revealCollisionPaintComplete)
        {
            revealAnimationComplete = true;
            OnRevealAnimationComplete.Invoke();
        }
    }

    public void ResetRevealAnimation()
    {
        revealAnimationComplete = false;
        revealAirPaintComplete = false;
        revealCollisionPaintComplete = false;
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
        paintSphere.Color = c;

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