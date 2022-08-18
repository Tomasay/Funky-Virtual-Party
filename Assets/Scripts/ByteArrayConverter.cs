using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ByteArrayConverter : MonoBehaviour
{
    /// <summary>
    /// Converts specified object to a byte array. *Make sure object is serializable, otherwise it will throw errors*
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="obj">The object being converted to a byte array</param>
    /// <returns></returns>
    public static byte[] ObjectToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    /// <summary>
    /// Converts a byte array back to specified object type. *Make sure object type is the same object type specified when creating byte array*
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="arrBytes">The byte array being converted back into an object</param>
    /// <returns></returns>
    public static T ByteArrayToObject<T>(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        T obj = (T)binForm.Deserialize(memStream);

        return obj;
    }
}