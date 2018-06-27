using UnityEditor;

namespace FrameWork.Editor.AssetBundle
{
    public sealed class BuildSetting
    {

        public static BuildOptions options = BuildOptions.None;
        public static BuildAssetBundleOptions optionsAssetBundle = BuildAssetBundleOptions.None;
        private static bool s_IsDebug = false;
        public static bool isDebug
        {
            get
            {
                return s_IsDebug;
            }
            set
            {
                s_IsDebug = value;
                options = s_IsDebug ?
                    BuildOptions.AllowDebugging |
                    BuildOptions.AutoRunPlayer |
                    BuildOptions.Development |
                    BuildOptions.ConnectWithProfiler |
                    BuildOptions.ShowBuiltPlayer
                    : BuildOptions.None;
            }
        }

        public static string platformName { get; private set; }

        public static string dataPath
        {
            get
            {
                return string.Format("../{0}/{1}", PlayerSettings.productName, platformName);
            }
        }

        public static string uploadPath
        {
            get
            {
                return string.Format("../{0}/Upload/{1}", PlayerSettings.productName, platformName);
            }
        }

        public static BuildTarget buildTarget{get;set;}
        public static void SwitchPlatform(BuildTarget target)
        {
            buildTarget = target;
            BuildTargetGroup group = BuildTargetGroup.Unknown;
            switch(target)
            {
                case BuildTarget.StandaloneWindows:
                    group = BuildTargetGroup.Standalone;
                    platformName = "PC";
                    break;
                case BuildTarget.Android:
                    group = BuildTargetGroup.Android;
                    platformName = "Android";
                    break;
                case BuildTarget.iOS:
                    group = BuildTargetGroup.iOS;
                    platformName = "IOS";
                    break;
                case BuildTarget.WebGL:
                    group = BuildTargetGroup.WebGL;
                    platformName = "Web";
                    break;
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
            PlayerSettings.bundleVersion = ConstantBuildData.version;

            // 刷新编辑器
            AssetDatabase.Refresh();
        }

    }
}
