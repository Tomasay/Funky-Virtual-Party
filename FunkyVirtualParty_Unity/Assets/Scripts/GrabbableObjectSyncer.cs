using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Animations;

#if UNITY_ANDROID
using Autohand;
#endif

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class GrabbableObjectSyncer : ObjectSyncer
{
#if UNITY_ANDROID
    [SerializeField] protected Grabbable grabbable;
#endif

    [SerializeField] protected ParentConstraint constraint;
    [SerializeField] protected Rigidbody rb;

    protected bool isDropped;

    protected override void Awake()
    {
        base.Awake();

#if UNITY_ANDROID
        grabbable.onRelease.AddListener(OnDrop);
#endif

#if UNITY_WEBGL
        if (ClientManagerWeb.instance)
        {
            ClientManagerWeb.instance.Manager.Socket.On<string, byte[]>("MethodCallToClientByteArray", MethodCalledFromServer);
        }
#endif
    }


#if UNITY_ANDROID
    /// <summary>
    /// Compresses trajectory data into a byte array for easier sending over network
    /// </summary>
    /// <param name="pos">position of object</param>
    /// <param name="vel">velocity of object</param>
    /// <returns>byte array data of trajectory</returns>
    public new byte[] SerializeTrajectory(Vector3 pos, Vector3 vel)
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(objectID);
                writer.Write(pos);
                writer.Write(vel);
            }
            return m.ToArray();
        }
    }

    /// <summary>
    /// Called when grabbable is dropped. Triggers trajectory information to be sent
    /// </summary>
    /// <param name="h">Hand that dropped grabbable</param>
    /// <param name="g">Grabbbable that was dropped</param>
    protected void OnDrop(Hand h, Grabbable g)
    {
        StartCoroutine("SendTrajectoryAsync");
    }

    /// <summary>
    /// Called to manually trigger trajectory information to be sent other than OnDrop. EX: Fireballs spawning in the air from fireball powerup
    /// </summary>
    public void ManualSendTrajectory()
    {
        StartCoroutine("SendTrajectoryAsync");
    }

    IEnumerator SendTrajectoryAsync()
    {
        yield return new WaitForSeconds(0);

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByteArray", "Trajectory", SerializeTrajectory(rb.position, rb.velocity));
        }
    }
#endif

#if UNITY_WEBGL
/// <summary>
/// Decompresses trajectory data after receiving it over network
/// </summary>
/// <param name="data">byte array trajectory data</param>
    public void DeserializeTrajectory(byte[] data)
    {
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                if (!reader.ReadByte().Equals(objectID)) return;

                constraint.constraintActive = false;
                constraint.enabled = false;
                isDropped = true;

                rb.isKinematic = false;
                rb.useGravity = true;

                rb.position = reader.ReadVector3();
                rb.velocity = reader.ReadVector3();
            }
        }
    }

    protected void MethodCalledFromServer(string methodName, byte[] data)
    {
        if (methodName.Equals("Trajectory"))
        {
            DeserializeTrajectory(data);
        }
    }
#endif
}