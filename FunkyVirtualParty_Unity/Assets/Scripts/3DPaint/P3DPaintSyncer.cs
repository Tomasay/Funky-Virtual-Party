using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PaintIn3D;
using CW.Common;

public class P3DPaintSyncer : MonoBehaviour, IHitPoint, IHitLine
{
	public UnityEvent OnHandleHitline;

	public List<PaintHitLineModel> currentHitLineModels;
	int currentHitLineIndex = 0;

	P3dPaintSphere paintSphere;
	IHitLine hitLine;

	private void Awake()
    {
		paintSphere = GetComponent<P3dPaintSphere>();
		hitLine = GetComponent<IHitLine>();

		OnHandleHitline = new UnityEvent();
		currentHitLineModels = new List<PaintHitLineModel>();

		DrawingsSyncer.instance.paintSyncer = this;
	}

    public void HandleHitPoint(bool preview, int priority, float pressure, int seed, Vector3 position, Quaternion rotation)
	{
	}

	public void HandleHitLine(bool preview, int priority, float pressure, int seed, Vector3 position, Vector3 endPosition, Quaternion rotation, bool clip)
	{
		OnHandleHitline.Invoke();

#if UNITY_ANDROID
		PaintHitLineModel newHitLine = new PaintHitLineModel();
		newHitLine.preview = preview;
		newHitLine.priority = priority;
		newHitLine.pressure = pressure;
		newHitLine.seed = seed;
		newHitLine.position = position;
		newHitLine.endPosition = endPosition;
		newHitLine.rotation = rotation;
		newHitLine.clip = clip;
		newHitLine.color = paintSphere.Color;

		DrawingsSyncer.instance.CurrentDrawing.paintHitLines.Add(newHitLine);

		HapticsManager.instance.TriggerHaptic(false, 0.1f, 0.1f);
#endif
	}

	public void ReceiveHitLine(PaintHitLineModel lhm)
	{
		paintSphere.Color = lhm.color;
		hitLine.HandleHitLine(lhm.preview, lhm.priority, lhm.pressure, lhm.seed, lhm.position, lhm.endPosition, lhm.rotation, lhm.clip);

		currentHitLineModels.Add(lhm);
	}

	public void RevealHitLine()
    {
		PaintHitLineModel lhm = currentHitLineModels[currentHitLineIndex];
		paintSphere.Color = lhm.color;
		hitLine.HandleHitLine(lhm.preview, lhm.priority, lhm.pressure, lhm.seed, lhm.position, lhm.endPosition, lhm.rotation, lhm.clip);
		currentHitLineIndex++;
	}

	public void ResetStoredHitLines()
    {
		currentHitLineModels.Clear();
		currentHitLineModels = new List<PaintHitLineModel>();
		currentHitLineIndex = 0;
	}

	public bool CanRevealAnotherHitLine()
    {
		return currentHitLineIndex < currentHitLineModels.Count;
    }
}