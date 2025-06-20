using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal class SerializableVersionUtil
    {
        internal static int GetMajor(int build) => build / 10000;
        internal static int GetMinor(int build) => build % 10000 / 100;
        internal static int GetPatch(int build) => build % 100;

        internal static void SetMajor(ref int build, ref long releaseDate, int major)
        {
            build = (major * 10000) + GetMinor(build) * 100 + GetPatch(build);
            releaseDate = UnixTime.Now;
        }

        internal static void SetMinor(ref int build, ref long releaseDate, int minor)
        {
            build = GetMajor(build) * 10000 + (minor * 100) + GetPatch(build);
            releaseDate = UnixTime.Now;
        }

        internal static void SetPatch(ref int build, ref long releaseDate, int patch)
        {
            build = GetMajor(build) * 10000 + GetMinor(build) * 100 + patch;
            releaseDate = UnixTime.Now;
        }
    }
}