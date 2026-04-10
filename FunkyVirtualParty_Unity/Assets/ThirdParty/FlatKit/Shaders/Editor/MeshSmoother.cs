// MeshSmoother.cs — stub re-created to fix missing-class compile error after FlatKit upgrade.
// FlatKit uses UV channel 7 to store averaged smooth normals for outline passes.

using System.Collections.Generic;
using UnityEngine;

public static class MeshSmoother {
    private const int SmoothNormalsUVChannel = 7;

    public static bool HasSmoothNormals(Mesh mesh) {
        var uvs = new List<Vector3>();
        mesh.GetUVs(SmoothNormalsUVChannel, uvs);
        return uvs.Count > 0;
    }

    public static void SmoothNormals(Mesh mesh) {
        var vertices = mesh.vertices;
        var normals  = mesh.normals;

        // Accumulate normals per unique vertex position.
        var accumulated = new Dictionary<Vector3, Vector3>(vertices.Length);
        for (int i = 0; i < vertices.Length; i++) {
            var v = vertices[i];
            accumulated.TryGetValue(v, out var sum);
            accumulated[v] = sum + normals[i];
        }

        // Normalize accumulated normals.
        var keys = new List<Vector3>(accumulated.Keys);
        foreach (var key in keys) {
            accumulated[key] = accumulated[key].normalized;
        }

        // Build per-vertex list and write to UV7.
        var smoothNormals = new List<Vector3>(vertices.Length);
        for (int i = 0; i < vertices.Length; i++) {
            smoothNormals.Add(accumulated[vertices[i]]);
        }

        mesh.SetUVs(SmoothNormalsUVChannel, smoothNormals);
    }
}
