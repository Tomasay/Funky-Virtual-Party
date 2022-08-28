using UnityEngine;
[System.Serializable]

public struct SerializedQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializedQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public SerializedQuaternion(Quaternion q)
    {
        this.x = q.x;
        this.y = q.y;
        this.z = q.z;
        this.w = q.w;
    }

    public override bool Equals(object obj)
    {
        if ((obj is SerializedQuaternion) == false)
        {
            return false;
        }

        var s = (SerializedQuaternion)obj;
        return x == s.x && y == s.y && z == s.z && w == s.w;
    }

    public override int GetHashCode()
    {
        var hashCode = 373119288;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + z.GetHashCode();
        return hashCode;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }

    public static bool operator ==(SerializedQuaternion a, SerializedQuaternion b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
    }

    public static bool operator !=(SerializedQuaternion a, SerializedQuaternion b)
    {
        return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
    }

    public static implicit operator Quaternion(SerializedQuaternion x)
    {
        return new Quaternion(x.x, x.y, x.z, x.w);
    }

    public static implicit operator SerializedQuaternion(Quaternion x)
    {
        return new SerializedQuaternion(x.x, x.y, x.z, x.w);
    }

}