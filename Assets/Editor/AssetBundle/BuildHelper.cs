using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using System.IO;
using System.Security.Cryptography;

namespace FrameWork.Editor.AssetBundle
{
    public sealed class BuildHelper
    {        
        public static void CopyAsset2Resources()
        {
            if(ConstantBuildData.type.Equals(VersionType.Full))
            {

            }
        }

        public static string GenerateMD5(string path)
        {
            try
            {
                using(var fileStream = File.OpenRead(path))
                {
                    int len = checked((int)fileStream.Length);
                    byte[] data = new byte[len];
                    fileStream.Read(data, 0, len);
                    MD5 md5 = new MD5CryptoServiceProvider();

                    byte [] fileMD5Bytes = md5.ComputeHash(data);
                    return System.BitConverter.ToString(fileMD5Bytes).Replace("-", "").ToLower();
                }
            }
            catch(FileNotFoundException e)
            {
                return "";
            }
        }
    }
}
