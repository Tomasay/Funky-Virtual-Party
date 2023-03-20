using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MannequinSolverClient : MonoBehaviour
{
    [SerializeField]
    Transform[] ikRefs;

    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField]
    MeshCollider meshCollider;

    Mesh colliderMesh;

    private void Awake()
    {
        if (ClientManagerWeb.instance)
        {
            ClientManagerWeb.instance.Manager.Socket.On<string, byte[]>("MethodCallToClientByteArray", MethodCalledFromServer);
        }
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClientByteArray");
#endif
    }

    protected virtual void MethodCalledFromServer(string methodName, byte[] data)
    {
        if (methodName.Equals("IKData"))
        {
            Deserialize(data);
            SetPoseColliders();
        }
    }

    public void Deserialize(byte[] data)
    {
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                foreach (Transform t in ikRefs)
                {
                    t.position = reader.ReadVector3();
                    t.rotation = reader.ReadQuaternion();
                }
            }
        }
    }

    void SetPoseColliders()
    {
        colliderMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(colliderMesh, true); ;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = colliderMesh;
    }
}
