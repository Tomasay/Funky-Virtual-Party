using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PaintIn3D;
using CW.Common;

public class P3DPaintSyncer : MonoBehaviour, IHitPoint, IHitLine
{
	public UnityEvent OnHandleHitline;

    private void Awake()
    {
		OnHandleHitline = new UnityEvent();

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

		DrawingsSyncer.instance.CurrentDrawing.paintHitLines.Add(newHitLine);

		HapticsManager.instance.TriggerHaptic(false, 0.1f, 0.1f);
#endif
	}

	public void ReceiveHitLine(PaintHitLineModel lhm)
	{
		// Loop through all components that implement IHitLine
		foreach (var hitLine in GetComponentsInChildren<IHitLine>())
		{
			// Ignore this one so we don't recursively paint
			if ((Object)hitLine != this)
			{
				// Submit the hit line
				hitLine.HandleHitLine(lhm.preview, lhm.priority, lhm.pressure, lhm.seed, lhm.position, lhm.endPosition, lhm.rotation, lhm.clip);
			}
		}
	}
}