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

    [SerializeField]
    Rigidbody rb;

    bool isPainting;

    //The amount of time that has to pass before another point can be created
    const float pointSecondDelay = 0.01f;

    float lastPointTime;

    const int maxPointCount = 10000;
    int currentPointCount;

    void Update()
    {
        if(isPainting && rb.velocity.magnitude > 0.1f && (Time.time - lastPointTime) > pointSecondDelay && currentPointCount < maxPointCount)
        {
            Vector3 pos = currentLine.transform.InverseTransformPoint(tip.position);
            currentLine.AddPoint(pos);
            currentPointCount++;
            lastPointTime = Time.time;
        }
    }

    private void CreateNewLine()
    {
        GameObject newLine = new GameObject("line");
        newLine.transform.parent = linesParent;

        Polyline pl = newLine.AddComponent<Polyline>();
        pl.Thickness = 0.01f;
        pl.Color = Color.black;
        pl.Geometry = PolylineGeometry.Billboard;
        pl.DetailLevel = DetailLevel.Minimal;
        pl.Joins = PolylineJoins.Round;
        pl.Closed = false;
        pl.SetPoints(new List<PolylinePoint>());

        currentLine = pl;
    }

    public void OnTriggerPressed(Hand h, Grabbable g)
    {
        isPainting = true;

        CreateNewLine();

        HapticsManager.instance.TriggerHaptic(h.left, 999, 0.1f);
    }

    public void OnTriggerReleased(Hand h, Grabbable g)
    {
        isPainting = false;
        HapticsManager.instance.StopHaptics(h.left);
    }
}