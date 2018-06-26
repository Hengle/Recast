using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Editor.AssetBundle
{
    public class BuildPackageWnd : EditorWindow
    {       
        [MenuItem("版本发布/出版本")]
        static void CreateBuildPackageWindow()
        {
            EditorUtility.ClearProgressBar();
            BuildPackageWnd window = EditorWindow.GetWindow<BuildPackageWnd>("Build Package");
            window.minSize = new UnityEngine.Vector2(200, 250);
        }

        private delegate void ToggleAction(Toggle toggle);
        private Func<string, ToggleAction, Toggle> CreateItem = (label, action) => { Toggle toggle = new Toggle(); toggle.label = label; toggle.data = action;  return toggle; };

        private bool isInitialized = false;
        private List<Toggle> m_Toggles = new List<Toggle>();

        private int toggleWidth = 250;
        private int toggleHeight = 17;
        


        private void OnGUI()
        {
            GUI.BeginScrollView(new Rect(0, 0, position.width, position.height - 50), new Vector2(position.width - 20, 10), new Rect(0, 0, position.width, position.height - 50));
            if(GUI.Toggle(new Rect(20, 17, 250, 17), false, new GUIContent("1. BuildAssetBundle")))
            {
                BuildPipeline.BuildAssetBundles();
            }
            foreach(var toggle in m_Toggles)
            {
                toggle.Draw();
            }
            GUI.EndScrollView();
        }

        private void InitGUI()
        {

            m_Toggles.Add(CreateItem("1. Build AssetBundle", (toggle) => { BuildPipeline.BuildAssetBundles(); }));
            for(int i = 0; i < m_Toggles.Count; ++ i)
            {
                m_Toggles[i].rect.Set(20, i * toggleHeight, toggleWidth, toggleHeight);
            }
            Toggle debugToggle = CreateItem("2. Mark is Debug", (toggle) => { BuildSetting.isDebug = toggle.isOn; });
            debugToggle.rect.Set(20, m_Toggles.Count * toggleHeight, toggleWidth, toggleHeight);
            Toggle buildToggle = CreateItem("3. Build Package", (toggle) => { BuildSetting.SwitchPlatform(EditorUserBuildSettings.activeBuildTarget); });
            buildToggle.rect.Set(20, (m_Toggles.Count + 1) * toggleHeight, toggleWidth, toggleHeight);
        }
    }
}
