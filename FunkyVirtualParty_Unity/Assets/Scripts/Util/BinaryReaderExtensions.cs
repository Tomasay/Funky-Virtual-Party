using UnityEngine;
using System.IO;

static class BinaryReaderExtensions
{
    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        return new Vector2
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle()
        };
    }

    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        return new Vector3
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle(),
            z = reader.ReadSingle()
        };
    }

    public static Quaternion ReadQuaternion(this BinaryReader reader)
    {
        return new Quaternion
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle(),
            z = reader.ReadSingle(),
            w = reader.ReadSingle(),
        };
    }
}