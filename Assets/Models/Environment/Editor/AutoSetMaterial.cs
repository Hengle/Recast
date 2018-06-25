using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Author mingzhang02
/// Desc 在目标路径下搜索同名材质球,并替换.
/// 美术工程下的部分材质球没有设置正确.重新导入后自动创建了新的材质球,使用默认材质球.
/// 为了不手动重新设置几百个材质球, 所以写了这个蛋疼的脚本.
/// Date 20180402
/// </summary>
public class AutoSetMaterial : EditorWindow {

    [MenuItem("辅助工具/自动材质设置")]
    static void Apply()
    {
        EditorWindow.GetWindow(typeof(AutoSetMaterial));
    }

    GameObject target = null;
    string path;

    private void OnGUI()
    {
        path = EditorGUILayout.TextField("材质路径", path);
        target = (GameObject)EditorGUILayout.ObjectField(target, typeof(GameObject), true);
        if (GUILayout.Button("Set", GUILayout.Width(200)))
        {
            string path2 = "/Models/Environment/Models/Environment/" + path;
            string realPath = Application.dataPath + path2;

            if (Directory.Exists(realPath) && null != target)
            {
                Renderer[] rs = target.GetComponentsInChildren<Renderer>();
                string matName = "";
                for (int i = 0; i < rs.Length; i++)
                {
                    matName = rs[i].sharedMaterial.name;
                    matName = matName.Replace(" Instance", "");
                    Material mat = (Material)AssetDatabase.LoadAssetAtPath("Assets" + path2 + "/" + matName+".mat", typeof(Material));
                    if (mat != null)
                    {
                        rs[i].sharedMaterial = mat;
                    }
                }
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError(realPath+" not exit or target " + target + " is null.");
            }
        }
    }
}
