using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Autohand;
using UnityEngine.Events;

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
    PaintPalette palette;

#if UNITY_ANDROID
    [SerializeField]
    Collider col, tipCol;

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    ThreeDPaintGameManager gm;

    [SerializeField]
    AutoHandPlayer ahp;
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
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
        ClientManagerWeb.instance.Manager.Socket.On<string, byte>("MethodCallToClientByte", MethodCalledFromServer);
#endif

        tipMesh.material.color = currentColor;
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClient");
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClientByte");
#endif
    }

    void Update()
    {
#if UNITY_ANDROID
        if (isPainting && (rb.velocity.magnitude > 0.025f || ahp.GetComponent<Rigidbody>().velocity.magnitude > 1) && currentPointCount < maxPointCount)
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
            isPainting = true;

            CreateNewLine();
            if (ClientManager.instance && gm.State == ThreeDPaintGameState.VRPainting) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PenTriggerPressed", "");

            HapticsManager.instance.TriggerHaptic(h.left, 999, 0.1f);
        }
    }

    public void OnTriggerReleased(Hand h, Grabbable g)
    {
        if (canPaint)
        {
            isPainting = false;
            HapticsManager.instance.StopHaptics(h.left);

            if (ClientManager.instance && gm.State == ThreeDPaintGameState.VRPainting) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PenTriggerReleased", "");
        }
    }
#endif

#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("PenTriggerPressed"))
        {
            isPainting = true;
            CreateNewLine();
        }
        else if(methodName.Equals("PenTriggerReleased"))
        {
            isPainting = false;
        }
        else if (methodName.Equals("PenDisable"))
        {
            SetActive(false);
        }
        else if (methodName.Equals("PenEnable"))
        {
            SetActive(true);
        }
    }

    void MethodCalledFromServer(string methodName, byte data)
    {
        if(methodName.Equals("ChangeColorPen"))
        {
            ChangeColor(data);
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
    }

    public void EraseAllLines()
    {
        foreach (Polyline pl in linesParent.GetComponentsInChildren<Polyline>())
        {
            Destroy(pl.gameObject);
        }
    }

    public void ChangeColor(int c)
    {
#if UNITY_ANDROID
        if (IsInHand)
        {
            currentColor = palette.colors[c];
            tipMesh.material.color = palette.colors[c];

            if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByte", "ChangeColorPen", (byte)c);
        }
#endif
#if UNITY_WEBGL
            currentColor = palette.colors[c];
            tipMesh.material.color = palette.colors[c];
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
        if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", active ? "PenEnable" : "PenDisable", "");
#endif
#if UNITY_WEBGL
        tipMesh.enabled = active;
        baseMesh.enabled = active;
        this.active = active;
#endif
    }
}