using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public static class ConstantData
    {
        /// <summary>
        /// 更新地址
        /// </summary>
        public static string updateURL
        {
            get
            {
#if UNITY_EDITOR
                string url = "";
#elif UNITY_IOS
                string url = "";
#elif UNITY_ANDROID
                string url = "";
#else
                string url = "";
#endif 
                return string.Format("{0}{1}", url, ConstantBuildData.version);
            }
        }

        /// <summary>
        /// 更新资源绝对路径
        /// </summary>
        private static string m_UpdatePath;
        public static string updatePath
        {
            get
            {
                if(string.IsNullOrEmpty(m_UpdatePath))
                {
                    m_UpdatePath = string.Format("{0}/update", Application.persistentDataPath);
                }

                return m_UpdatePath;
            }
        }

        public static string AssetBundlePath
        {
            get
            {
#if UNITY_STANDALONE_WIN
                return Application.streamingAssetsPath + "/ab/" + ConstantBuildData.version + "/";
#else
			return Application.persistentDataPath + "/ab/"; 
#endif
            }
        }
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

