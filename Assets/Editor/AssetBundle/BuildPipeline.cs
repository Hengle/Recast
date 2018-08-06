using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

// https://blog.csdn.net/l_jinxiong/article/details/50877926
namespace FrameWork.Editor.AssetBundle
{
    public class BuildPipeline
    {
        public static void BuildAssetBundles()
        { 
            string dataPath = BuildSetting.dataPath;
            if(Directory.Exists(dataPath))
            {
                Directory.Delete(dataPath, true);
            }

            string assetbundlePath = Application.dataPath + "/" + BuildSetting.dataPath;
            if(!Directory.Exists(assetbundlePath))
            {
                Directory.CreateDirectory(assetbundlePath);
            }

            UnityEditor.BuildPipeline.BuildAssetBundles(assetbundlePath, BuildSetting.optionsAssetBundle, BuildSetting.buildTarget);
            EditorUtility.ClearProgressBar();


            files.Clear();
            paths.Clear();
            RecursiveDirectory(dataPath);
            string detailFilePath = assetbundlePath + "/detail.txt";
            if(File.Exists(detailFilePath))
            {
                File.Delete(detailFilePath);
            }

            FileStream fs = new FileStream(detailFilePath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            foreach(var file in files)
            {
                if(file.EndsWith(".meta") || file.Contains(".DS_Store"))
                {
                    continue;
                }

                string md5 = BuildHelper.GenerateMD5(file);
                string value = file.Replace(dataPath, string.Empty);
                sw.Write(value + "|" + md5);
            }
        }

        private static List<string> files;
        private static List<string> paths;
        private static void RecursiveDirectory(string path)
        {}

        public static void BuildPlayer()
        { }















    }
}