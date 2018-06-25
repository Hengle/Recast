/*
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameWork.LightMap
{

    public class BuildingEditor : EditorWindow
    {
        
        static string path = "Assets/Prefabs/BuildingModel2";
        static string originPath = "Assets/Prefabs/BuildingModel";
        static string treePath = "Assets/Prefabs/TreeBlock";

        static string BUILDING_LAYER_NAME = "BigMapBuilding";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;

        public BuildingInfoList BuildingInfos;

        public static BuildingPrefabInfo m_BuildingPrefabInfo;
        public static string m_BuilingPrefabInfoPath = "";
        //public static GameObject Eff1;
        //public static GameObject Eff3;



        [MenuItem("LightMap/Building/RemoveLightData")]
        static void RemoveLightData()
        {
            Transform target = Selection.activeGameObject.transform;
            PrefabLightmapData[] plds = target.GetComponentsInChildren<PrefabLightmapData>();
            foreach (var pld in plds)
            {
                GameObject.DestroyImmediate(pld);

            }

            Debug.Log("Remove all children components");
        }


        // Add menu named "My Window" to the Window menu
        [MenuItem("LightMap/Building/BuildingMenu")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            BuildingEditor window = (BuildingEditor)EditorWindow.GetWindow(typeof(BuildingEditor));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            path = EditorGUILayout.TextField("Prefab Folder", path);

            BuildingInfos = (BuildingInfoList)EditorGUILayout.ObjectField("buildingData", BuildingInfos, typeof(BuildingInfoList), false);
            m_BuildingPrefabInfo = (BuildingPrefabInfo)EditorGUILayout.ObjectField("prefabData", m_BuildingPrefabInfo, typeof(BuildingPrefabInfo), false);

            //Eff1 = (GameObject)EditorGUILayout.ObjectField("lv up eff1", Eff1, typeof(GameObject), false);

            //Eff3 = (GameObject)EditorGUILayout.ObjectField("lv up eff3", Eff3, typeof(GameObject), false);


            if (EditorGUILayout.DropdownButton(new GUIContent("Create"), FocusType.Keyboard, GUILayout.Width(100)))
                CreatePrefab();

            EditorGUILayout.Space();
            if (EditorGUILayout.DropdownButton(new GUIContent("Check Prefab"), FocusType.Keyboard, GUILayout.Width(200)))
                CheckBuildingPrefab();

            EditorGUILayout.Space();
            if (EditorGUILayout.DropdownButton(new GUIContent("记录选中建筑信息"), FocusType.Keyboard, GUILayout.Width(200)))
                SaveBuildingInfo();


            EditorGUILayout.Space();
            if (EditorGUILayout.DropdownButton(new GUIContent("替换原模型"), FocusType.Keyboard, GUILayout.Width(200)))
                Replace();


            EditorGUILayout.Space();
            if (EditorGUILayout.DropdownButton(new GUIContent("Add Point"), FocusType.Keyboard, GUILayout.Width(200)))
                AddPoint();

            EditorGUILayout.Space();
            if (EditorGUILayout.DropdownButton(new GUIContent("同步Model2中的建筑信息"), FocusType.Keyboard, GUILayout.Width(200)))
                SyncBuildingModel2();


            EditorGUILayout.Space();
            if (EditorGUILayout.DropdownButton(new GUIContent("生成建筑配置信息"), FocusType.Keyboard, GUILayout.Width(200)))
               GenerateBuildingInfo();

            EditorGUILayout.Space();
            if (EditorGUILayout.DropdownButton(new GUIContent("调整节点旋转信息"), FocusType.Keyboard, GUILayout.Width(200)))
                AdjustPoint();


            EditorGUILayout.Space();

            if (EditorGUILayout.DropdownButton(new GUIContent("检测lineRender/trail"), FocusType.Keyboard, GUILayout.Width(200)))
                CheckLineTrail();

            EditorGUILayout.Space();
            if (EditorGUILayout.DropdownButton(new GUIContent("树预制添加脚本"), FocusType.Keyboard, GUILayout.Width(200)))
                ModifyTreePrefab();


        }

        static void CreatePrefab()
        {
            PrefabLightmapData[] pldArr = GameObject.FindObjectsOfType<PrefabLightmapData>();
            foreach (var data in pldArr)
            {
                var bb = data.transform.Find("bodyCenter");
                if (bb == null)
                {
                    GameObject bodyCenter = new GameObject();
                    bodyCenter.name = "bodyCenter";
                    bodyCenter.transform.parent = data.transform;

                    bodyCenter.transform.localPosition = Vector3.zero;
                    bodyCenter.transform.localRotation = Quaternion.identity;
                }



                PrefabUtility.CreatePrefab(path + "/" + data.name + ".prefab", data.gameObject);


            }

            AssetDatabase.Refresh();
            Debug.Log("create prefab successfully");



        }

        private static Dictionary<int, int> buildingDic;
        static void CreateBuildingList()
        {
            buildingDic = new Dictionary<int, int>();
            buildingDic.Add(1011,1);
            buildingDic.Add(1021, 1);
            buildingDic.Add(2001, 3);
            buildingDic.Add(2005, 3);
            buildingDic.Add(2007, 3);
            buildingDic.Add(3001, 3);
            buildingDic.Add(3004, 3);
        }


        const string BUILDING_WORKER_STRING_AXE = "Villager_Axe";
        const string BUILDING_WORKER_STRING_BASKET = "Villager_Basket";
        const string BUILDING_WORKER_STRING_HOE = "Villager_Hoe";
        const string BUILDING_WORKER_STRING_EMPTY_HAND = "Villager_Emptyhand";


        static BuildingWorker.WorkerType GetWorkerTypeByName(string name)
        {
            switch (name)
            {

                case BUILDING_WORKER_STRING_AXE:
                    return BuildingWorker.WorkerType.Axe;
                case BUILDING_WORKER_STRING_BASKET:
                    return BuildingWorker.WorkerType.Basket;
                case BUILDING_WORKER_STRING_HOE:
                    return BuildingWorker.WorkerType.Hoe;
                case BUILDING_WORKER_STRING_EMPTY_HAND:
                    return BuildingWorker.WorkerType.EmptyHand;
                default:
                    return BuildingWorker.WorkerType.EmptyHand;



            }
        }

        static string GetNameByWorkerType(BuildingWorker.WorkerType type)
        {

            switch (type)
            {
                case BuildingWorker.WorkerType.Axe:
                    return BUILDING_WORKER_STRING_AXE;
                case BuildingWorker.WorkerType.Basket:
                    return BUILDING_WORKER_STRING_BASKET;
                
                case BuildingWorker.WorkerType.EmptyHand:
                    return BUILDING_WORKER_STRING_EMPTY_HAND;
                case BuildingWorker.WorkerType.Hoe:
                    return BUILDING_WORKER_STRING_HOE;
                


            }

            return BUILDING_WORKER_STRING_EMPTY_HAND;


        }


        static void SaveBuildingInfo()
        {
            m_BuilingPrefabInfoPath =  Application.dataPath + "/Data/MapData/buildingdata.txt";
           


            var guids2 = Selection.assetGUIDs;

            m_BuildingPrefabInfo.BuildingInfos.Clear();
            m_BuildingPrefabInfo.BuildingPrefabNames.Clear();

            foreach (var guid in guids2)
            {
                
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));


                BuildingPrefab bp = new BuildingPrefab();
                string name = obj.name;
                Transform airAnchor = obj.transform.Find("AirAnchor");
                Transform innerAnchor = airAnchor.GetChild(0);

                BoxCollider bc = innerAnchor.GetComponent<BoxCollider>();

                bp.airAnchorPos = airAnchor.position;
                bp.airAnchorScale = airAnchor.localScale;
                bp.airAnchorRotation = airAnchor.eulerAngles;

                bp.innerAnchorPos = innerAnchor.position;
                bp.innerAnchorScale = innerAnchor.localScale;
                bp.innerAnchorRotation = innerAnchor.eulerAngles;

                int childRootLength = childRootNames.Length;
                bp.anchorPos = new Vector3[childRootLength];
                bp.anchorRotation = new Vector3[childRootLength];

                for (int i=0;i< childRootLength;i++)
                {
                    Transform targetTran = obj.transform.Find(childRootNames[i]);
                    if (targetTran != null)
                    {
                        bp.anchorPos[i] = targetTran.position;
                        bp.anchorRotation[i] = targetTran.rotation.eulerAngles;

                    }

                }

                var villagerTrans = airAnchor.Find("Villager");
                if (villagerTrans != null)
                {
                    int length = villagerTrans.childCount;
                    bp.villagerPos = new Vector3[length];
                    var pathComp = villagerTrans.GetComponent<BuildingWorkerPath>();
                    bp.workerType = GetWorkerTypeByName(pathComp.WorkerName);

                    //bp.waitting = new List<float>();

                    //foreach (var time in pathComp.WaitList)
                    //{
                    //    bp.waitting.Add(time);

                    //}

                    bp.waitting = pathComp.WaitList;

                    for (int i=0; i< length; i++)
                    {
                        var point = villagerTrans.GetChild(i);
                        Vector3 pointPos = point.position;

                        bp.villagerPos[i] = pointPos;

                    }
                    

                }

                if (bc != null)
                {
                    bp.boxCenter = bc.center;
                    bp.boxSize = bc.size;

                }



                if (!m_BuildingPrefabInfo.BuildingPrefabNames.Contains(name))
                {
                    m_BuildingPrefabInfo.BuildingPrefabNames.Add(name);
                    m_BuildingPrefabInfo.BuildingInfos.Add(bp);


                }
                else
                    Debug.LogError("repeat building name:" + name);

            }


            if (m_BuilingPrefabInfoPath.Length != 0)
            {
                //string fullPath = M_mapDataAsset.MapDataFilePath + fileName;
                if (!System.IO.File.Exists(m_BuilingPrefabInfoPath))
                {
                    var fs = System.IO.File.Create(m_BuilingPrefabInfoPath);
                    fs.Close();

                }

                System.IO.File.WriteAllText(m_BuilingPrefabInfoPath, EditorJsonUtility.ToJson(m_BuildingPrefabInfo, true));

               


                AssetDatabase.SaveAssets();
                Debug.Log("write builidng data to path: " + m_BuilingPrefabInfoPath + " successfully");
                AssetDatabase.Refresh();

            }


        }




        static string eff1Path = "Assets/Prefabs/Effect/e_con_upgrade_1.prefab";
        static string eff3Path = "Assets/Prefabs/Effect/e_con_upgrade_3.prefab";
        static string con2001AdditionPath = "Assets/Prefabs/Building/Con2001TeamPos.prefab";
        static string villagerPath = "Assets/Prefabs/BuildingModel2/BuildingVillager.prefab";
        static string logoutPath = "Assets/Prefabs/BuildingModel/";
        static string[] excludeBuildings = new string[] { "Con3201", "Con3202", "Con3103", "Con3102", "Con6041","Con2008" };
        static string[] villagerBuildingArrays = new string[] { "Con6011", "Con6021", "Con6022", "Con6012" };
        static string[] buildingShadowArrays = new string[] { "Con1011", "Con1012", "Con2005", "Con2007", "Con2008", "Con3101", "Con3102", "Con3103", "Con3104"
                                                };


        static void ShadowPlaneProcess(Transform target)
        {


        }



        static void CheckBuildingPrefab()
        {
            m_BuilingPrefabInfoPath = Application.dataPath + "/Data/MapData/buildingdata.txt";
            CreateBuildingList();

            EditorJsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(m_BuilingPrefabInfoPath), m_BuildingPrefabInfo);
            //var guids2 = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            var guids2 = Selection.assetGUIDs;
            GameObject Eff1 = GameObject.Instantiate <GameObject>( AssetDatabase.LoadAssetAtPath<GameObject>(eff1Path) );
            GameObject Eff3 = GameObject.Instantiate<GameObject>( AssetDatabase.LoadAssetAtPath<GameObject>(eff3Path) );

            foreach (var guid in guids2)
            {
                //Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
               


                GameObject target = GameObject.Instantiate<GameObject>(obj);
                target.transform.localPosition = Vector3.zero;

                var buildingnames = m_BuildingPrefabInfo.BuildingPrefabNames;
                BuildingPrefab curInfo = null;
                int curIdx = -1;
                if (buildingnames.Contains(obj.name))
                {

                    curInfo = m_BuildingPrefabInfo.BuildingInfos[buildingnames.IndexOf(obj.name)] ;
                }

                if (curInfo == null)
                {
                    Debug.LogError("info is null:"+obj.name);
                    //continue;
                }

                BoxCollider box = target.GetComponent<BoxCollider>();
                if (box == null)
                {
                    box = target.AddComponent<BoxCollider>();
                    if (curInfo != null)
                    {
                        box.center = curInfo.boxCenter;
                        box.size = curInfo.boxSize;
                    }
                   

                }



                //if (target.name.StartsWith("COL3"))
                //    continue;

                target.name = target.name.Remove(target.name.IndexOf("(Clone)"));


                //fx

                string[] names= target.name.Split(new char[] { 'n'});

                int numIdx = int.Parse(names[1]);
                GameObject copyEff = null;
                if (numIdx == 1)
                    copyEff = Eff1;
                else
                    copyEff = Eff3;

                GameObject eff = GameObject.Instantiate<GameObject>(copyEff);
                eff.name = "eff";
                eff.transform.parent = target.transform;
                eff.transform.localPosition = Vector3.zero;
              
                RecursiveChangeLayer(target, BUILDING_LAYER_NAME);

                


 

                GameObject root = new GameObject();
                root.name = target.name;
                target.name = target.name + "_anchor";
                root.transform.localPosition = Vector3.zero;


                GameObject airAnchor = new GameObject();
                airAnchor.name = "AirAnchor";
                airAnchor.transform.localPosition = Vector3.zero;
                target.transform.parent = airAnchor.transform;
                airAnchor.transform.parent = root.transform;

                ShadowPlaneProcess(target.transform);

                if (curInfo != null)
                {
                    airAnchor.transform.position = curInfo.airAnchorPos;
                    airAnchor.transform.rotation = Quaternion.Euler(curInfo.airAnchorRotation);
                    airAnchor.transform.localScale = curInfo.airAnchorScale;

                    
                    target.transform.position = curInfo.innerAnchorPos;
                    target.transform.rotation = Quaternion.Euler(curInfo.innerAnchorRotation);
                    target.transform.localScale = curInfo.innerAnchorScale;

                }

                BuildingBodyProxy bb = root.AddComponent<BuildingBodyProxy>();
                List<string> loopNames = new List<string>();
                List<string> upgrateNames = new List<string>();
                bb.modelAnimation = target.GetComponent<Animation>();


                if (obj.name.Equals("Con2001"))
                {
                    GameObject con2001Go = GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(con2001AdditionPath));
                    Vector3 localPos = con2001Go.transform.localPosition;

                    con2001Go.name = "Con2001TeamPos";
                    con2001Go.transform.parent = airAnchor.transform;
                    con2001Go.transform.localPosition = localPos;

                    int childLength = 9;
                    bb.exGo = new GameObject[childLength];
                    for (int i=0; i < childLength;i++)
                    {
                        var childGo = con2001Go.transform.GetChild(i).gameObject;
                        bb.exGo[i] = childGo;
                    }
                    


                }

                foreach (var name in villagerBuildingArrays)
                {
                    if (obj.name.Equals(name))
                    {

                        GameObject villagerGo = GameObject.Instantiate<GameObject>(AssetDatabase.LoadAssetAtPath<GameObject>(villagerPath));
                        Vector3 villagerPos = villagerGo.transform.localPosition;

                        villagerGo.name = "Villager";
                        villagerGo.transform.parent = airAnchor.transform;
                        villagerGo.transform.localPosition = villagerPos;

                        bb.unlockTargetPoint = villagerGo.transform;

                        var pathComp = villagerGo.GetComponent<BuildingWorkerPath>();

                        if (curInfo != null)
                        {
                            
                             pathComp.WorkerName = GetNameByWorkerType(curInfo.workerType);

                            //if (curInfo.waitting != null)
                            //{
                            //    pathComp.m_listWait = new List<WaitForSeconds>();
                            //    foreach (var time in curInfo.waitting)
                            //    {
                            //        pathComp.m_listWait.Add(time);

                            //    }

                            //}

                            pathComp.WaitList = curInfo.waitting;

                            if (curInfo.villagerPos != null)
                            {

                                int childLength = villagerGo.transform.childCount;
                                for (int i = 0; i < childLength; i++)
                                {
                                    var targetTran = villagerGo.transform.GetChild(i);
                                    if(curInfo.villagerPos[i] != null)
                                        targetTran.position = curInfo.villagerPos[i];
                                    


                                }
                            }



                        }


                    }
                }

               


                int lvLength = 23;
                foreach (var name in excludeBuildings)
                {
                    if (obj.name.Equals(name))
                    {
                        lvLength = 1;
                        break;
                    }

                }


                for (int i = 1; i <= lvLength; i++)
                {

                    string loopName = "lv" + i + "_loop";
                    string upgrateName = "lv" + i + "_show";
                    loopNames.Add(loopName);
                    upgrateNames.Add(upgrateName);


                }

                bb.listAniLoop = loopNames;
                bb.listAniUpgrate = upgrateNames;




                int length = childRootNames.Length;
                for (int i = 0; i < length; i++)
                {

                    ///todo
                    GameObject child = new GameObject();
                    child.name = childRootNames[i];

                    child.transform.localPosition = Vector3.zero;
                    if (curInfo != null)
                    {
                        if (curInfo.anchorPos != null)
                        {
                            if (curInfo.anchorPos[i] != null)
                            {

                                child.transform.position = curInfo.anchorPos[i];
                            }

                        }

                        if (curInfo.anchorRotation != null)
                        {
                            if (curInfo.anchorRotation[i] != null)
                            {
                                child.transform.rotation = Quaternion.Euler(curInfo.anchorRotation[i]);
                            }



                        }


                    }

                    
                    child.transform.parent = root.transform;

                    var fieldInfo = bb.GetType().GetField(childRootNames[i]);
                    if (fieldInfo != null)
                    {

                        fieldInfo.SetValue(bb, child.transform);
                    }



                }


                var effTran = eff.transform;

                int childCount = effTran.childCount;
                List<GameObject> effs = new List<GameObject>();
                for (int i = 0; i < childCount; i++)
                {
                    var children = effTran.GetChild(i);
                    effs.Add(children.gameObject);

                }

                bb.dicEffectNode = new GameObject[24];
                for (int i = 1; i <= 24; i++)
                {
                    //foreach (var go in effs)
                    //{
                    //    if (go.name.EndsWith(i.ToString()))
                    //    {
                    //        bb.dicEffectNode[i - 1] = go;
                    //    }



                    //}

                    bb.dicEffectNode[i - 1] = eff;
                }




                var bodyCenter = target.transform.Find("bodyCenter");


                //bb.floorCenter = root.transform;

                bb.airAnchor = airAnchor.transform;
                if (bodyCenter != null)
                    bb.bodyCenter = bodyCenter;











                string url = logoutPath+root.name+".prefab";
               



                PrefabUtility.CreatePrefab(url, root);
                //PrefabUtility.ReplacePrefab(target, obj);
                //GameObject.DestroyImmediate(target);
                AssetDatabase.SaveAssets();
                Debug.Log(url);

            }


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("CheckBuilding finish");


        }



        private static void RecursiveChangeLayer(GameObject parentGameObject, string layer)
        {
            parentGameObject.layer = LayerMask.NameToLayer(layer);


            foreach (Transform child in parentGameObject.transform)
            {

                RecursiveChangeLayer(child.gameObject, layer);
            }
        }


        //将原建筑airAnchor下的建筑替换为model2下的预制，并将bodyCenter引用重新赋值
        static void Replace()
        {
            var guids2 = AssetDatabase.FindAssets("t:Prefab", new string[] { originPath });
            foreach (var guid in guids2)
            {
                //Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));


                GameObject target = GameObject.Instantiate<GameObject>(obj);

                BuildingBody bb = target.GetComponent<BuildingBody>();


                target.name = target.name.Remove(target.name.IndexOf("(Clone)"));

                int childLength = target.transform.childCount;

                for (int i=0; i < childLength;i++)
                {
                    Transform child = target.transform.GetChild(i);
                    if (child.name.StartsWith("AirAnchor") || child.name.StartsWith("airAnchor"))
                    {
                        Transform originGo = child.GetChild(0);
                        string goName = originGo.name;

                        GameObject newGo = AssetDatabase.LoadAssetAtPath<GameObject>(path+"/"+goName +".prefab");

                        if (newGo == null)
                        {
                            Debug.LogError("go is null : "+ path + "/" + goName + ".prefab");
                            continue;

                        }
                        GameObject newTarget = GameObject.Instantiate<GameObject>(newGo);
                        Transform bodyCenter = newTarget.transform.Find("bodyCenter");

                        newTarget.name = newTarget.name.Remove(newTarget.name.IndexOf("(Clone)"));
                        newTarget.transform.parent = child;
                        newTarget.transform.localPosition = originGo.transform.localPosition;

                        if (bb != null)
                            bb.bodyCenter = bodyCenter;



                        GameObject.DestroyImmediate(originGo.gameObject);


                    }


                }








                PrefabUtility.ReplacePrefab(target, obj);
                GameObject.DestroyImmediate(target);

            }


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("replace finish");






        }



        static string[] childRootNames = new string[] { "hexPoint", "modelInfoTip", "uiHeadTip", "uiProgressLevup", "uiOperate", "uiResource", "uiWaitMove",
            "activeeffectPoint","upgradeEffectPoint","finishedEffectPoint","lockHeadEffectPoint","unlockeffectPoint" };
        static void AddPoint()
        {


            
            //var guids2 = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            var guids2 = Selection.assetGUIDs;
            foreach (var guid in guids2)
            {
                //Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));


                GameObject target = GameObject.Instantiate<GameObject>(obj);


                target.name = target.name.Remove(target.name.IndexOf("(Clone)"));





                GameObject root = new GameObject();
                root.name = target.name;
                target.name = target.name + "_anchor";
                root.transform.localPosition = Vector3.zero;


                GameObject airAnchor = new GameObject();
                airAnchor.name = "AirAnchor";
                airAnchor.transform.localPosition = Vector3.zero;
                target.transform.parent = airAnchor.transform;
                airAnchor.transform.parent = root.transform;
                BuildingBody bb = root.AddComponent<BuildingBody>();
                List<string> loopNames = new List<string>();
                List<string> upgrateNames = new List<string>();
                bb.modelAnimation = target.GetComponent<Animation>();

                for (int i=1; i<= 23;i++)
                {

                    string loopName = "lv" + i + "_loop";
                    string upgrateName = "lv" + i + "_show";
                    loopNames.Add(loopName);
                    upgrateNames.Add(upgrateName);


                }

                bb.listAniLoop = loopNames;
                bb.listAniUpgrate = upgrateNames;




                int length = childRootNames.Length;
                for (int i = 0; i < length; i++)
                {

                    //to do
                    GameObject child = new GameObject();
                    child.name = childRootNames[i];
                    child.transform.localPosition = Vector3.zero;
                    child.transform.parent = root.transform;

                    var fieldInfo = bb.GetType().GetField(childRootNames[i]);
                    if (fieldInfo != null)
                    {

                        fieldInfo.SetValue(bb, child.transform);
                    }

                   

                }


                var effTran = target.transform.Find("eff");

                int childCount = effTran.childCount;
                List<GameObject> effs = new List<GameObject>();
                for (int i = 0; i < childCount; i++)
                {
                    var children = effTran.GetChild(i);
                    effs.Add(children.gameObject);
                    
                }

                bb.dicEffectNode = new GameObject[24] ;
                for (int i=1;i<=24;i++)
                {
                    foreach (var go in effs)
                    {
                        if (go.name.EndsWith(i.ToString()))
                        {
                            bb.dicEffectNode[i-1] = go;
                        }
                           


                    }


                }




                var bodyCenter = target.transform.Find("bodyCenter");


                //bb.floorCenter = root.transform;
                
                bb.airAnchor = airAnchor.transform;
                if(bodyCenter != null)
                    bb.bodyCenter = bodyCenter;
                //bb.hexPoint = hexPoint.transform;
                //bb.uiProgressLevup = uiProgressLevup.transform;
                //bb.uiHeadTip = uiHeadTip.transform;


                PrefabUtility.ReplacePrefab(root, obj);
                GameObject.DestroyImmediate(root);

            }


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Add point finish");



        }


        void GenerateBuildingInfo()
        {



            var guids2 = AssetDatabase.FindAssets("t:Prefab", new string[] { originPath });
            BuildingInfos.BuildingInfos = new List<BuildingInfo>();
            BuildingInfos.BuildingNames = new List<string>();
            foreach (var guid in guids2)
            {
                //Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));


                GameObject target = GameObject.Instantiate<GameObject>(obj);
                target.name = target.name.Remove(target.name.IndexOf("(Clone)"));

                int childCount = target.transform.childCount;

                BuildingInfo buildinfo = new BuildingInfo();


                List<string> names = new List<string>(childRootNames);
                for (int i=0; i< childCount; i++)
                {
                    Transform children = target.transform.GetChild(i);
                    foreach (var name in names)
                    {
                        if (children.name.Contains(name))
                        {
                            buildinfo.AnchorNames.Add(name);
                            buildinfo.Position.Add(children.transform.localPosition);

                        }

                    }
        

                }

             
                BuildingInfos.BuildingInfos.Add(buildinfo);
                BuildingInfos.BuildingNames.Add(target.name);


                GameObject.DestroyImmediate(target);

            }




            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);


            var fullPath = EditorUtility.SaveFilePanel(
               "Save Builidng data",
               Application.dataPath+"/Prefabs/BuildingModel2",
              "",
               "txt");


            if (fullPath.Length != 0)
            {
                //string fullPath = M_mapDataAsset.MapDataFilePath + fileName;
                if (!System.IO.File.Exists(fullPath))
                {
                    var fs = System.IO.File.Create(fullPath);
                    fs.Close();

                }

                System.IO.File.WriteAllText(fullPath, EditorJsonUtility.ToJson(BuildingInfos, true));

       

            }
            else
            {

                EditorUtility.DisplayDialog(
                "Save File",
                "You Must Select a file",
                "Ok");
                return;

            }





            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("__________________Generate Buildinginfo finish_______________________");

        }


        void ModifyTreePrefab()
        {
            var guids2 = AssetDatabase.FindAssets("t:Prefab", new string[] { treePath });
            foreach (var guid in guids2)
            {
                //Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                GameObject target = GameObject.Instantiate<GameObject>(obj);

                target.name = target.name.Remove(target.name.IndexOf("(Clone)"));

                CityTree treeComp = target.GetComponent<CityTree>();
                if (treeComp == null)
                {
                    treeComp = target.AddComponent<CityTree>();
                    treeComp.TreeNode = target.transform;

                    var nameArray = target.name.Split(new char[] { '_'});
                    int idx = 0;

                    if (int.TryParse(nameArray[1], out idx))
                    {
                        treeComp.CityType = idx;

                    }
                    else
                    {
                        Debug.LogError("name parse error:"+ target.name);
                        return;
                    }

                }



                PrefabUtility.ReplacePrefab(target, obj);
                GameObject.DestroyImmediate(target);

            }


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("modify tree prefabs finish");


        }



        void SyncBuildingModel2()
        {
            var guids2 = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            foreach (var guid in guids2)
            {
                //Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                GameObject target = GameObject.Instantiate<GameObject>(obj);
                           
                target.name = target.name.Remove(target.name.IndexOf("(Clone)"));
                int childCount = target.transform.childCount;

                int length = BuildingInfos.BuildingNames.Count;
                int idx = -1;

                if (BuildingInfos.BuildingNames.Contains(target.name))
                    idx = BuildingInfos.BuildingNames.IndexOf(target.name);
                else
                {
                    Debug.Log("building: " + target.name + " cann't find match data");
                    continue;
                }
                   

                BuildingInfo info = BuildingInfos.BuildingInfos[idx];


                for (int i = 0; i < childCount; i++)
                {
                    Transform children = target.transform.GetChild(i);
                    if (info.AnchorNames.Contains(children.name))
                    {
                        int row = info.AnchorNames.IndexOf(children.name);
                        Vector3 pos = info.Position[row];
                        children.localPosition = pos;

                    }
                    


                }


                PrefabUtility.ReplacePrefab(target, obj);
                GameObject.DestroyImmediate(target);

            }


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("CheckBuilding finish");


        }


        string pointName = "modelInfoTip";
        Vector3 rotation = new Vector3(6.951f,1.377f,11.469f);
        void AdjustPoint()
        {
            var guids2 = AssetDatabase.FindAssets("t:Prefab", new string[] { originPath });
            foreach (var guid in guids2)
            {
                //Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                GameObject target = GameObject.Instantiate<GameObject>(obj);

                target.name = target.name.Remove(target.name.IndexOf("(Clone)"));
                int childCount = target.transform.childCount;

                for (int i=0; i< childCount;i++)
                {
                    Transform childTran = target.transform.GetChild(i);
                    if (childTran.name.Contains(pointName))
                    {
                        childTran.localRotation = Quaternion.EulerAngles(rotation);

                    }


                }



                PrefabUtility.ReplacePrefab(target, obj);
                GameObject.DestroyImmediate(target);

            }


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(" 调整节点完成");


        }


        void CheckLineTrail()
        {

            RecursiveCheckRenderer(Selection.activeGameObject);

            Debug.Log("Finish checking");

        }


        private static void RecursiveCheckRenderer(GameObject parentGameObject)
        {

            ParticleSystem ps = parentGameObject.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                //Debug.Log("ps : "+ parentGameObject.name );
                if (ps.rotationBySpeed.enabled||ps.rotationOverLifetime.enabled)
                    Debug.Log("rotation by speed: " + parentGameObject.name);

            }
          



            LineRenderer line = parentGameObject.GetComponent<LineRenderer>();
            TrailRenderer trail = parentGameObject.GetComponent<TrailRenderer>();

            if (line != null)
                Debug.LogError("line renderer: "+ parentGameObject.name);

            if (trail != null)
                Debug.LogError("trail renderer: " + parentGameObject.name);




            foreach (Transform child in parentGameObject.transform)
            {

                RecursiveCheckRenderer(child.gameObject);
            }
        }






    }


   


}

[CreateAssetMenu]
public class BuildingPrefabInfo : ScriptableObject
{
    [SerializeField]
    public List<string> BuildingPrefabNames = new List<string>();
    [SerializeField]
    public List<BuildingPrefab> BuildingInfos = new List<BuildingPrefab>();
    //public Dictionary<string, BuildingPrefab> BuildingPrefabsInfo = new Dictionary<string, BuildingPrefab>();
    
    
}

[Serializable]
public class BuildingPrefab
{
    public Vector3 airAnchorPos;
    public Vector3 airAnchorScale;
    public Vector3 airAnchorRotation;

    public Vector3 innerAnchorPos;
    public Vector3 innerAnchorScale;
    public Vector3 innerAnchorRotation;

    public Vector3 boxCenter;
    public Vector3 boxSize;

    public Vector3[] anchorPos;
    public Vector3[] anchorRotation;

    public Vector3[] villagerPos;
    public BuildingWorker.WorkerType workerType ;
    public List<float> waitting;
    
    

}

*/
