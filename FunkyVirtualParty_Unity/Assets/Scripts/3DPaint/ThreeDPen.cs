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
    //Skips this many points beforing sending a new one. Higher = better performance. Lower = more accurate results for clients
    const int networkedPointsOffset = 10;
    int pointSkipCounter = 0;

    float lastPointTime;

    const int maxPointCount = 100000;
    int currentPointCount;

    private bool canPaint = true;

    public bool IsInHand { get => isInHand; set => isInHand = value; }
    public bool CanPaint { get => canPaint; set { canPaint = value; if (!value) { isPainting = false; HapticsManager.instance.StopHaptics(true); HapticsManager.instance.StopHaptics(false); } } }

    public UnityEvent OnDraw;

    private void Awake()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif

        tipMesh.material.color = currentColor;
    }

#if UNITY_ANDROID
    void Update()
    {
        if(isPainting && (rb.velocity.magnitude > 0.1f || ahp.GetComponent<Rigidbody>().velocity.magnitude > 1) && (Time.time - lastPointTime) > pointSecondDelay && currentPointCount < maxPointCount)
        {
            AddNewLinePoint();
            if(pointSkipCounter == 0 && ClientManager.instance && gm.State == ThreeDPaintGameState.VRPainting) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "PenAddLinePoint", "");

            pointSkipCounter++;
            if(pointSkipCounter >= networkedPointsOffset)
            {
                pointSkipCounter = 0;
            }

            OnDraw.Invoke();
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
        else if(methodName.Equals("ChangeColorPen"))
        {
            if (ColorUtility.TryParseHtmlString(data, out Color col))
            {
                ChangeColor(col);
            }
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

    public void ChangeColor(Color c)
    {
#if UNITY_ANDROID
        if (IsInHand)
        {
            currentColor = c;
            tipMesh.material.color = c;

            if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "ChangeColorPen", "#" + ColorUtility.ToHtmlStringRGB(c));
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
        if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", active ? "PenEnable" : "PenDisable", "");
#endif
#if UNITY_WEBGL
        tipMesh.enabled = active;
        baseMesh.enabled = active;
        this.active = active;
#endif
    }
}