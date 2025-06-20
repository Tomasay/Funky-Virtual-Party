using System;
using UnityEngine;

namespace Glitch9.Editor
{
    internal enum VersionIncrement
    {
        Major,
        Minor,
        Patch
    }

    [Serializable]
    internal class SerializableVersion : IEquatable<SerializableVersion>, IComparable<SerializableVersion>
    {
        [SerializeField] private int build;
        [SerializeField] private int previousBuild;
        [SerializeField] private long releaseDate;
        [SerializeField] private long previousReleaseDate;

        internal int Major
        {
            get => SerializableVersionUtil.GetMajor(build);
            set => SerializableVersionUtil.SetMajor(ref build, ref releaseDate, value);
        }

        internal int Minor
        {
            get => SerializableVersionUtil.GetMinor(build);
            set => SerializableVersionUtil.SetMinor(ref build, ref releaseDate, value);
        }

        internal int Patch
        {
            get => SerializableVersionUtil.GetPatch(build);
            set => SerializableVersionUtil.SetPatch(ref build, ref releaseDate, value);
        }

        internal int Build
        {
            get => build;
            set
            {
                build = value;
                releaseDate = UnixTime.Now;
            }
        }

        internal UnixTime ReleaseDate
        {
            get => releaseDate;
            set => releaseDate = value;
        }

        internal SerializableVersion()
        {
        }

        internal SerializableVersion(string version)
        {
            string[] parts = version.Split('.');
            if (parts.Length != 3) throw new ArgumentException("Invalid version format. Must be in the format 'major.minor.patch'.");

            Major = int.Parse(parts[0]);
            Minor = int.Parse(parts[1]);
            Patch = int.Parse(parts[2]);
        }

        internal void Increase(VersionIncrement increment)
        {
            previousBuild = build;
            previousReleaseDate = releaseDate;

            switch (increment)
            {
                case VersionIncrement.Major:
                    Major++;
                    Minor = 0;
                    Patch = 0;
                    break;
                case VersionIncrement.Minor:
                    Minor++;
                    Patch = 0;
                    break;
                case VersionIncrement.Patch:
                    Patch++;
                    break;
            }
        }

        internal void UndoIncrease()
        {
            if (previousBuild == 0 && previousReleaseDate == 0)
            {
                throw new InvalidOperationException("No previous version to revert to.");
            }

            build = previousBuild;
            releaseDate = previousReleaseDate;

            // 백업 초기화
            previousBuild = 0;
            previousReleaseDate = 0;
        }

        // internal void BackupCurrent()
        // {
        //     previousBuild = build;
        //     previousReleaseDate = releaseDate;
        // }

        // internal void SetVersion(int major, int minor, int patch)
        // {
        //     build = CalcBuildNumber(major, minor, patch);
        //     releaseDate = UnixTime.Now;
        // }

        // Utility methods
        internal static int CalcBuildNumber(SerializableVersion version)
        {
            if (version == null) return -1;
            return CalcBuildNumber(version.Major, version.Minor, version.Patch);
        }

        internal static int CalcBuildNumber(int major, int minor, int patch)
        {
            return major * 10000 + minor * 100 + patch;
        }

        public bool Equals(SerializableVersion other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;
            return build == other.build && releaseDate == other.releaseDate;
        }

        public int CompareTo(SerializableVersion other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            if (ReferenceEquals(null, this)) return -1;

            if (build != other.build) return build.CompareTo(other.build);
            return releaseDate.CompareTo(other.releaseDate);
        }

        public static bool operator >(SerializableVersion version1, SerializableVersion version2)
        {
            return version1.CompareTo(version2) > 0;
        }

        public static bool operator <(SerializableVersion version1, SerializableVersion version2)
        {
            return version1.CompareTo(version2) < 0;
        }

        public static bool operator >=(SerializableVersion version1, SerializableVersion version2)
        {
            return version1.CompareTo(version2) >= 0;
        }

        public static bool operator <=(SerializableVersion version1, SerializableVersion version2)
        {
            return version1.CompareTo(version2) <= 0;
        }

        public static bool operator ==(SerializableVersion version1, SerializableVersion version2)
        {
            if (ReferenceEquals(version1, version2))
                return true;
            if (ReferenceEquals(version1, null))
                return false;
            if (ReferenceEquals(version2, null))
                return false;
            return version1.Equals(version2);
        }

        public static bool operator !=(SerializableVersion version1, SerializableVersion version2)
        {
            return !(version1 == version2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SerializableVersion))
                return false;
            return Build == ((SerializableVersion)obj).Build;
        }

        public override int GetHashCode()
        {
            return Build.GetHashCode();
        }
    }
}