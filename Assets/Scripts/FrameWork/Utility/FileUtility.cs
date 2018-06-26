using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace FrameWork.Utility
{
    public sealed class FileUtility
    {
        public static bool Exists(string path)
        {
            bool exists = File.Exists(path);
            if(!exists)
            {
                Debug.LogWarning(string.Format("File Not Exists : {0}", path));
            }
            return exists;
        }    
        
        public static void CreateDirectory(string path)
        {
            if(Directory.Exists(path))
            {
                Debug.LogWarning(string.Format("Directory Exists : {0}", path));
                return;
            }

            Directory.CreateDirectory(path);
        }
    }
}
