using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using NaughtyAttributes;

public class BandwidthCalculator : MonoBehaviour
{
    private void OnDestroy()
    {
        CalculateTotalBandwidth();
    }

    [Button]
    public void CalculateTotalBandwidth()
    {
        // Get the type of the internal RealtimeProfiler class via reflection
        Type realtimeProfilerType = Type.GetType("Normal.Realtime.Profiler.RealtimeProfiler, Normal.Realtime.Profiler");

        if (realtimeProfilerType == null)
        {
            Debug.LogError("RealtimeProfiler class not found.");
            return;
        }

        // Get the static 'sessions' property of RealtimeProfiler
        PropertyInfo sessionsProperty = realtimeProfilerType.GetProperty("sessions", BindingFlags.Static | BindingFlags.Public);

        if (sessionsProperty == null)
        {
            Debug.LogError("RealtimeProfiler.sessions property not found.");
            return;
        }

        // Retrieve the session list
        var sessions = (IReadOnlyCollection<object>)sessionsProperty.GetValue(null);

        uint totalSentBytes = 0;
        uint totalReceivedBytes = 0;

        // Loop through each session and use reflection to access its properties
        foreach (var session in sessions)
        {
            Type sessionType = session.GetType();
            //totalSentBytes += (uint)sessionType.GetProperty("sentBytesRPCTotal").GetValue(session);
            //totalSentBytes += (uint)sessionType.GetProperty("sentBytesAudioTotal").GetValue(session);
            totalSentBytes += (uint)sessionType.GetProperty("sentBytesDatastoreTotal").GetValue(session);

            //totalReceivedBytes += (uint)sessionType.GetProperty("receivedBytesRPCTotal").GetValue(session);
            //totalReceivedBytes += (uint)sessionType.GetProperty("receivedBytesAudioTotal").GetValue(session);
            totalReceivedBytes += (uint)sessionType.GetProperty("receivedBytesDatastoreTotal").GetValue(session);
        }

        Debug.Log("Total Bandwidth Sent: " + totalSentBytes + " bytes, Received " + totalReceivedBytes + " bytes, Sent + Received = " + (totalSentBytes + totalReceivedBytes) + " bytes");
    }
}