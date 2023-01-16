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
    [SerializeField] protected GameObject handAnchorLeft, handAnchorRight;

    protected bool isDropped;

    protected override void Awake()
    {
        base.Awake();

#if UNITY_ANDROID
        grabbable.onRelease.AddListener(OnDrop);
        grabbable.onGrab.AddListener(OnGrab);
#endif

#if UNITY_WEBGL
        if (ClientManagerWeb.instance)
        {
            ClientManagerWeb.instance.Manager.Socket.On<string, byte[]>("MethodCallToClientByteArray", MethodCalledFromServer);
            ClientManagerWeb.instance.Manager.Socket.On<string, byte>("MethodCallToClientByte", MethodCalledFromServer);
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
    /// Called when grabbable is grabbed. Triggers constraint info to be set for clients
    /// </summary>
    /// <param name="h">Hand that grabbed grabbable</param>
    /// <param name="g">Grabbbable that was grabbed</param>
    protected void OnGrab(Hand h, Grabbable g)
    {
        if (h.left)
        {
            if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByte", "OnGrabLeft", objectID);
        }
        else
        {
            if (ClientManager.instance) ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByte", "OnGrabRight", objectID);
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
    protected virtual void MethodCalledFromServer(string methodName, byte[] data)
    {
        if (methodName.Equals("Trajectory"))
        {
            DeserializeTrajectory(data);
        }
    }

    protected virtual void MethodCalledFromServer(string methodName, byte data)
    {
        if (objectID == data)
        {
            if (methodName.Equals("OnGrabLeft"))
            {
                EnableConstraint(true);
            }
            else if (methodName.Equals("OnGrabRight"))
            {
                EnableConstraint(false);
            }
        }
    }

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

                //Disable constraint to hand
                constraint.constraintActive = false;
                constraint.enabled = false;
                isDropped = true;

                //Physics
                rb.isKinematic = false;
                rb.useGravity = true;

                rb.position = reader.ReadVector3();
                rb.velocity = reader.ReadVector3();
            }
        }
    }

    /// <summary>
    /// Enables constraint so that grabbable sticks to hand when we have received message from host that grabbable has been grabbed
    /// </summary>
    /// <param name="isLeft">Was the grabbable picked up with the left hand</param>
    protected void EnableConstraint(bool isLeft)
    {
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = isLeft ? handAnchorLeft.transform : handAnchorRight.transform;
        src.weight = 1;

        if (constraint.sourceCount > 0)
        {
            constraint.SetSource(0, src);
        }
        else
        {
            constraint.AddSource(src);
        }
        constraint.constraintActive = true;
        constraint.enabled = true;
    }
#endif
}