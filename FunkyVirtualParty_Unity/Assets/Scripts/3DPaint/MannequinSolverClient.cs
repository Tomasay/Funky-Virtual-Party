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

    public void SetPoseColliders()
    {
        colliderMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(colliderMesh, true); ;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = colliderMesh;
    }
}
