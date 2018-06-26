using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public static class ConstantData
    {
    }

    public enum VersionType
    {
        Full,
        Increment,
    }

    public sealed class ConstantBuildData
    {
        public static readonly string version = "0.8.0";

        public static readonly VersionType type = VersionType.Full;
    }
}

