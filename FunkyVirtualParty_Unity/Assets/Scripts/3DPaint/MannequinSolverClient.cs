using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MannequinSolverClient : MonoBehaviour
{
    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField]
    MeshCollider meshCollider;

    Mesh colliderMesh;

    public void SetPoseColliders()
    {
        Invoke("SetPoseCollidersDelayed", 0.5f);
    }

    void SetPoseCollidersDelayed()
    {
        colliderMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(colliderMesh, true); ;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = colliderMesh;
    }
}