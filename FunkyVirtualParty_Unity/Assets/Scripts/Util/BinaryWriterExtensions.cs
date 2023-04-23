using UnityEngine;
using System.IO;

static class BinaryWriterExtensions
{
    public static void Write(this BinaryWriter writer, Vector3 vec3)
    {
        writer.Write(vec3.x);
        writer.Write(vec3.y);
        writer.Write(vec3.z);
    }

    public static void Write(this BinaryWriter writer, Vector2 vec2)
    {
        writer.Write(vec2.x);
        writer.Write(vec2.y);
    }

    public static void Write(this BinaryWriter writer, Quaternion quat)
    {
        writer.Write(quat.x);
        writer.Write(quat.y);
        writer.Write(quat.z);
        writer.Write(quat.w);
    }
}