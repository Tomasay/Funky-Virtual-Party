using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using CW.Common;

public class P3DPaintSyncer : MonoBehaviour, IHitPoint, IHitLine
{
	public void HandleHitPoint(bool preview, int priority, float pressure, int seed, Vector3 position, Quaternion rotation)
	{
	}

	public void HandleHitLine(bool preview, int priority, float pressure, int seed, Vector3 position, Vector3 endPosition, Quaternion rotation, bool clip)
	{
#if UNITY_ANDROID
		HapticsManager.instance.TriggerHaptic(false, 0.1f, 0.1f);
#endif
	}
}