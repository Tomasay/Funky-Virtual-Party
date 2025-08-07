using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using Shapes;
using PaintIn3D;
using TMPro;

public class DrawingsSyncer : RealtimeComponent<DrawingsModel>
{
    public static DrawingsSyncer instance;

    public List<List<PolylinePath>> drawingLines;

    public List<List<PolylinePath>> practiceDrawingLines;

    public RealtimeDictionary<DrawingModel> Drawings { get => model.drawings; }

    [SerializeField]
    P3dPaintableTexture paintTexture;

    [SerializeField]
    GameObject[] artPieceLineParents;

    [SerializeField]
    P3dPaintableTexture[] galleryPaintTextures;

    [SerializeField]
    GameObject[] galleryArmatures;

    [SerializeField]
    TMP_Text[] galleryTitles;

    [SerializeField]
    Transform linesParent;

    private void Awake()
    {
        //Singleton
        instance = this;

        drawingLines = new List<List<PolylinePath>>();
        practiceDrawingLines = new List<List<PolylinePath>>();
    }

#if UNITY_ANDROID || UNITY_STANDALONE_WIN
    private void Start()
    {
        int count = Drawings.Count;
        for (int i = 0; i < count; i++)
        {
            uint k = (uint)i;
            Drawings.Remove(k);
        }
    }
#endif

    private void OnDestroy()
    {
        Drawings.modelAdded -= Drawings_modelAdded;
        for (int i = 0; i < Drawings.Count; i++)
        {
            uint k = (uint)i;
            Drawings[k].penStrokes.modelAdded -= PenStrokes_modelAdded;
            Drawings[k].practicePenStrokes.modelAdded -= PracticePenStrokes_modelAdded;
            Drawings[k].paintTextureDidChange -= Model_paintTextureDidChange;
            Drawings[k].poseData.modelAdded -= PoseData_modelAdded;
            Drawings[k].titleDidChange -= Model_titleDidChange;
        }
    }

    /// <summary>
    /// Stores pose data for the current drawing being worked on
    /// </summary>
    /// <param name="armature">Parent GameObject of the mannequin. Should be named "Armature" in the scene</param>
    public void StorePoseData(GameObject armature)
    {
        foreach (Transform t in armature.GetComponentsInChildren<Transform>())
        {
            JointModel newJointInfo = new JointModel();
            newJointInfo.pos = t.localPosition;
            newJointInfo.rot = t.localRotation;

            Drawings[(uint)Drawings.Count - 1].poseData.Add(newJointInfo);
        }
    }

    /// <summary>
    /// Applies the pose data of a drawing of a specified index to a specified armature
    /// </summary>
    /// <param name="armature">The armature to apply the pose data to</param>
    /// <param name="drawingIndex">The index of the drawing to use the pose data from</param>
    public void ApplyPoseData(GameObject armature, int drawingIndex)
    {
        Transform[] transforms = armature.GetComponentsInChildren<Transform>();
        for (int i = 1; i < Drawings[(uint)drawingIndex].poseData.Count; i++)
        {
            transforms[i].localPosition = Drawings[(uint)drawingIndex].poseData[i].pos;
            transforms[i].localRotation = Drawings[(uint)drawingIndex].poseData[i].rot;

            //Ignore hip height set by height slider
            if(i == 1)
            {
                Vector3 pos = transforms[i].localPosition;
                pos.y = 0;
                transforms[i].localPosition = pos;
            }
        }
    }

    /// <summary>
    /// Shifts drawing lines over to be placed on appropriate gallery pedestals
    /// </summary>
    public void SetDrawingGalleryPositions()
    {
        if (drawingLines != null && drawingLines.Count > 0)
        {
            for (int i = 0; i < drawingLines.Count; i++)
            {
                Vector3 posOffset = linesParent.position - artPieceLineParents[i].transform.position;
                for (int j = 0; j < drawingLines[i].Count; j++)
                {
                    for (int k = 0; k < drawingLines[i][j].Count; k++)
                    {
                        PolylinePoint point = drawingLines[i][j][k]; //:o
                        point.point -= posOffset;
                        drawingLines[i][j].SetPoint(k, point);
                    }
                }
            }
        }
    }

    protected override void OnRealtimeModelReplaced(DrawingsModel previousModel, DrawingsModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.drawings.modelAdded -= Drawings_modelAdded;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {

            }

            // Register for events
            currentModel.drawings.modelAdded += Drawings_modelAdded;
        }
    }


#region Variable Callbacks
    //New drawing created
    private void Drawings_modelAdded(RealtimeDictionary<DrawingModel> dictionary, uint key, DrawingModel model, bool remote)
    {
        drawingLines.Add(new List<PolylinePath>());
        practiceDrawingLines.Add(new List<PolylinePath>());

        model.penStrokes.modelAdded += PenStrokes_modelAdded;
        model.practicePenStrokes.modelAdded += PracticePenStrokes_modelAdded;
        model.paintTextureDidChange += Model_paintTextureDidChange;
        model.poseData.modelAdded += PoseData_modelAdded;
        model.titleDidChange += Model_titleDidChange;
    }

    private void Model_titleDidChange(DrawingModel model, string value)
    {
        if (Drawings.Count >= 1)
        {
            galleryTitles[Drawings.Count - 1].text = value;
        }
    }

    //New pose data added for current drawing
    private void PoseData_modelAdded(RealtimeArray<JointModel> array, JointModel model, bool remote)
    {
        ApplyPoseData(galleryArmatures[Drawings.Count - 1], Drawings.Count - 1);
    }

    //Drawing's paint texture was changed
    private void Model_paintTextureDidChange(DrawingModel model, byte[] value)
    {
#if UNITY_WEBGL //Override final texture on main model on mobile to ensure it is 100% correct
        paintTexture.LoadFromData(value);
#endif

        if (Drawings.Count >= 1)
        {
            galleryPaintTextures[Drawings.Count - 1].LoadFromData(value);
        }
    }

    //New line within a drawing created
    private void PenStrokes_modelAdded(RealtimeArray<PenStrokeModel> array, PenStrokeModel model, bool remote)
    {
        drawingLines[drawingLines.Count-1].Add(new PolylinePath());
        model.linePoints.modelAdded += LinePoints_modelAdded;
    }

    //New line while practicing created
    private void PracticePenStrokes_modelAdded(RealtimeArray<PenStrokeModel> array, PenStrokeModel model, bool remote)
    {
        practiceDrawingLines[practiceDrawingLines.Count - 1].Add(new PolylinePath());
        model.linePoints.modelAdded += PracticeLinePoints_modelAdded;
    }

    //New point within a line created
    private void LinePoints_modelAdded(RealtimeArray<LinePointModel> array, LinePointModel model, bool remote)
    {
        int currentLineIndex = drawingLines[drawingLines.Count-1].Count-1;
        int currentPenStrokeIndex = Drawings[(uint)Drawings.Count - 1].penStrokes.Count - 1;
        drawingLines[drawingLines.Count-1][currentLineIndex].AddPoint(model.position, Drawings[(uint)Drawings.Count-1].penStrokes[currentPenStrokeIndex].lineColor);
    }

    //New point within a practice line created
    private void PracticeLinePoints_modelAdded(RealtimeArray<LinePointModel> array, LinePointModel model, bool remote)
    {
        int currentLineIndex = practiceDrawingLines[practiceDrawingLines.Count - 1].Count - 1;
        int currentPenStrokeIndex = Drawings[(uint)Drawings.Count - 1].practicePenStrokes.Count - 1;
        practiceDrawingLines[practiceDrawingLines.Count - 1][currentLineIndex].AddPoint(model.position, Drawings[(uint)Drawings.Count - 1].practicePenStrokes[currentPenStrokeIndex].lineColor);
    }
#endregion
}