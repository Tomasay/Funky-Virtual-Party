using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Autohand;

public class ThreeDPen : MonoBehaviour
{
    Polyline currentLine;

    [SerializeField]
    Transform linesParent;

    [SerializeField]
    Transform tip;

#if UNITY_ANDROID
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    ThreeDPaintGameManager gm;
#endif

    bool isPainting;

    //The amount of time that has to pass before another point can be created
    const float pointSecondDelay = 0.1f;

    float lastPointTime;

    const int maxPointCount = 10000;
    int currentPointCount;

    public bool canPaint = true;

    private void Awake()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif
    }

#if UNITY_ANDROID
    void Update()
    {
        if(isPainting && rb.velocity.magnitude > 0.1f && (Time.time - lastPointTime) > pointSecondDelay && currentPointCount < maxPointCount)
        {
            AddNewLinePoint();
            if(ClientManager.instance && gm.State == ThreeDPaintGameState.VRPainting) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PenAddLinePoint", "");
        }
    }

    public void OnTriggerPressed(Hand h, Grabbable g)
    {
        if (canPaint)
        {
            isPainting = true;

            CreateNewLine();
            if (ClientManager.instance && gm.State == ThreeDPaintGameState.VRPainting) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PenCreateNewLine", "");

            HapticsManager.instance.TriggerHaptic(h.left, 999, 0.1f);
        }
    }

    public void OnTriggerReleased(Hand h, Grabbable g)
    {
        if (canPaint)
        {
            isPainting = false;
            HapticsManager.instance.StopHaptics(h.left);
        }
    }
#endif

#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("PenCreateNewLine"))
        {
            CreateNewLine();
        }
        else if(methodName.Equals("PenAddLinePoint"))
        {
            AddNewLinePoint();
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
        pl.Color = Color.black;
        pl.Geometry = PolylineGeometry.Billboard;
        pl.DetailLevel = DetailLevel.Minimal;
        pl.Joins = PolylineJoins.Round;
        pl.Closed = false;
        pl.SetPoints(new List<PolylinePoint>());

        currentLine = pl;
    }

    private void AddNewLinePoint()
    {
        Vector3 pos = currentLine.transform.InverseTransformPoint(tip.position);
        currentLine.AddPoint(pos);
        currentPointCount++;
        lastPointTime = Time.time;
    }

    public void EraseAllLines()
    {
        foreach (Polyline pl in linesParent.GetComponentsInChildren<Polyline>())
        {
            Destroy(pl.gameObject);
        }
    }
}