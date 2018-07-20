using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace FrameWork.Editor
{
    public class ExportParameters
    {
        public float framerate = 30f;
        public AnimationClip[] animationClips = new AnimationClip[0];
        public string[] animationNames = new string[0];
        public Transform[] boneTransforms = new Transform[0];
        public Quaternion quaternionOffset = Quaternion.identity;
        public string[] boneNames = new string[0];
        public string outputFolderPath;
        public string outputFilePath;
    }

    public class MeshAnimationBoneTransform
    {
        public List<Vector3> positions;

        public List<Quaternion> rotations;

        public MeshAnimationBoneTransform()
        {
            this.positions = new List<Vector3>();
            this.rotations = new List<Quaternion>();
        }
    }

    public class MeshAnimationBoneGroup
    {
        public List<string> boneNames;

        public Dictionary<string, MeshAnimationBoneTransform> bones;

        public MeshAnimationBoneGroup(List<string> pBoneNames, int pBoneCount)
        {
            this.boneNames = pBoneNames;
            this.bones = new Dictionary<string, MeshAnimationBoneTransform>();
            foreach (string current in pBoneNames)
            {
                this.bones[current] = new MeshAnimationBoneTransform();
            }
        }
    }

    public class MeshAnimationExport 
    {
        public static ExportParameters GenerateDefaultSettings(GameObject pFbxInstance)
        {
            ExportParameters settings = new ExportParameters();
            ReadAnimationSettingsFromFbx(pFbxInstance, ref settings);
            PredictSettings(pFbxInstance, ref settings);
            return settings;
        }

        public static void ReadAnimationSettingsFromFbx(GameObject pFbxInstance, ref ExportParameters pSettings)
        {
            GameObject go = GameObject.Instantiate(pFbxInstance) as GameObject;
            Animation animation = go.GetComponentInChildren<Animation>();
            if (!animation)
            {
                Debug.LogError("Target game object has no Animation component!");
                return;
            }
            int clipCount = animation.GetClipCount();

            int index = 0;
            var animationClips = new AnimationClip[clipCount];
            var animationNames = new string[clipCount];

            var cli = animation.GetClip("idle");
            foreach (AnimationState state in animation)
            {
                animationClips[index] = state.clip;
                animationNames[index] = state.clip.name.Capitalize();
                index++;
            }
            GameObject.DestroyImmediate(go);

            pSettings.animationClips = animationClips;
            pSettings.animationNames = animationNames;
        }

        public static void PredictSettings(GameObject pObjectRoot, ref ExportParameters pSettings)
        {
            string prefabPath = pObjectRoot == null ? "" : AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(pObjectRoot));
            if (prefabPath.Length == 0)
            {
                Debug.LogWarning("Please use this tool with an instance of the prefab");
                return;
                // they drag/dropped the prefab itself. that's not supposed to be how you use this.
            }
            if (PrefabUtility.GetPrefabParent(pObjectRoot.transform.parent) != null)
            {
                Debug.LogWarning("Export settings prediction only works when given the root of the prefab instance");
                return;
            }

            PredictOutputPath(pObjectRoot, prefabPath, ref pSettings);
            PredictEmitterAnchors(pObjectRoot, ref pSettings);
        }

        private static void PredictOutputPath(GameObject pObjectRoot, string prefabPath, ref ExportParameters pSettings)
        {
            string objectFileName = convertStringToFileFormat(pObjectRoot.name);
            string subFolderNameBossOrSoldier = Directory.GetParent(Application.dataPath + prefabPath).Name;

            //string folderPath = Path.Combine(MESH_ANIMATIONS_ABSOLUTE_FOLDER_PATH, subFolderNameBossOrSoldier);
            //modified by @horatio
            string folderPath = MESH_ANIMATIONS_ABSOLUTE_FOLDER_PATH;

            // is there some way to get the prefix number so that we can generate the filename without an existing version?
            string[] possibleMatch = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(filepath => Path.GetFileNameWithoutExtension(filepath.ToLower())
                       .Contains(objectFileName) && !filepath.ToLower().Contains(".meta")).ToArray();

            if (possibleMatch.Length == 1)
            {
                pSettings.outputFilePath = possibleMatch[0];
                pSettings.outputFolderPath = Path.GetDirectoryName(pSettings.outputFilePath);
            }
            else
            {
                pSettings.outputFilePath = "";
                pSettings.outputFolderPath = folderPath;
            }
        }

        private static void PredictEmitterAnchors(GameObject pObjectRoot, ref ExportParameters pSettings)
        {
            Transform[] transforms = pObjectRoot.GetComponentsInChildren<Transform>();
            // the transforms in the object tree not in the prefab are the emitter anchors
            var emitterAnchorsQuery = transforms.Where(transform => PrefabUtility.GetPrefabParent(transform.gameObject) == null);
            Transform[] emitterAnchors = emitterAnchorsQuery.ToArray();
            string[] emitterAnchorNames = emitterAnchorsQuery.Select(transform => transform.gameObject.name).ToArray();
            int anchorCount = emitterAnchors.Length;

            ResizeDataLists(ref pSettings.boneTransforms, ref pSettings.boneNames, pDesiredCapacity: anchorCount);

            // fill data
            pSettings.boneTransforms = emitterAnchors;
            pSettings.boneNames = emitterAnchorNames;
        }

        private static string convertStringToFileFormat(string pInput)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            foreach (char c in pInput)
            {
                if (System.Char.IsUpper(c) && result.Length > 0)
                {
                    result.Append('_');
                }
                result.Append(System.Char.ToLower(c));
            }
            return result.ToString();
        }

        public static void ResizeDataLists<DataType>(ref DataType[] pDataArray, ref string[] pNames, int pDesiredCapacity)
        {
            pDesiredCapacity = Mathf.Clamp(pDesiredCapacity, 0, 100);
            int previousNumber = pNames.Length;
            pNames = ResizeArrayWithCopyExistingElements(pNames, pDesiredCapacity);
            pDataArray = ResizeArrayWithCopyExistingElements(pDataArray, pDesiredCapacity);

            for (int i = previousNumber; i < pNames.Length; i++)
            {
                pNames[i] = "";
            }
        }

        private static T[] ResizeArrayWithCopyExistingElements<T>(T[] pArray, int pNewSize)
        {
            T[] result = new T[pNewSize];
            for (int i = 0; i < pArray.Length && i < pNewSize; i++)
            {
                result[i] = pArray[i];
            }
            return result;
        }

        public static void ExportCombinedTexture(GameObject pFbxInstance, ExportParameters pSettings, System.Action<float> pProgressBarUpdater = null)
        {
            if (!AreParametersValid(pFbxInstance, pSettings))
            {
                return;
            }

            //List<AnimationClip> exportClips = GetClips ();
            //List<Transform> exportBoneTransforms = GetBoneTransforms ();
            //List<string> exportBoneNames = GetBoneNames ();
            AnimationClip[] exportClips = pSettings.animationClips;
            Transform[] exportBoneTransforms = pSettings.boneTransforms;
            string[] exportBoneNames = pSettings.boneNames;
            string[] animationNames = pSettings.animationNames;

            GameObject model = pFbxInstance;
            Transform baseTransform = model.transform;
            Animation animation = model.GetComponentInChildren<Animation>();

            ///origin code
            //SkinnedMeshRenderer renderer = model.GetComponentInChildren<SkinnedMeshRenderer>();
            //Mesh mesh = renderer.sharedMesh;

            SkinnedMeshRenderer[] renderArray = model.GetComponentsInChildren<SkinnedMeshRenderer>();
            int arrayLength = renderArray.Length;
            Mesh[] meshArray = new Mesh[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {

                meshArray[i] = renderArray[i].sharedMesh;
            }


            float frameInterval = 1.0f / pSettings.framerate;


            //清空并重建目标目录
            if (!System.IO.Directory.Exists(pSettings.outputFilePath))
            {
                System.IO.Directory.CreateDirectory(pSettings.outputFilePath);
            }
            else
            {
                System.IO.Directory.Delete(pSettings.outputFilePath, true);
                System.IO.Directory.CreateDirectory(pSettings.outputFilePath);
            }

            ///modify by niexin
            ///Attack1","Attack2","Dead","Hit","Run","Skill1","Wait1","Wait2" 8 default animations
            ///first line marked as attached model info, first color to number,following by vertex counts for each sub mesh,which is the width of texture
            ///某行首位标记attach数量信息，如0则无，那2行则为该物体动画数据，否则往上推，后续像素r分别写入顶点数，即动画纹理宽度
            ///8个动画的顺序依次标记首位8个像素，r通道填入是否存在的信息，g,b分别对应初始行，结束行位置，

            int totalframeCount = 0;

            totalframeCount = 1 + arrayLength;

            int aniFrameCount = 0;


            //pre cacl the length of all frames
            for (int i = 0; i < exportClips.Length; i++)
            {
                AnimationClip clip = exportClips[i];

                if (null == clip)
                {
                    continue;
                }

                float clipLength = clip.length;

                //Mesh frameMesh;
                //List<Vector3[]> frameVertices = new List<Vector3[]>();

                // Get the list of times for each frame
                List<float> frameTimes = GetFrameTimes(clipLength, frameInterval);
                aniFrameCount += frameTimes.Count;



            }

            totalframeCount += aniFrameCount * arrayLength;

            int[] vertexCount = new int[arrayLength];
            int maxVertexCount = 0;
            for (int i = 0; i < arrayLength; i++)
            {
                int count = meshArray[i].vertexCount;
                vertexCount[i] = count;
                if (count > maxVertexCount)
                    maxVertexCount = count;


            }




            Vector3[][] defaultAnimationInfos = new Vector3[arrayLength][];
            for (int i = 0; i < arrayLength; i++)
            {
                defaultAnimationInfos[i] = new Vector3[8];
                for (int j = 0; j < 8; j++)
                {
                    defaultAnimationInfos[i][j] = new Vector3(0, 0, 0);
                }

            }

            string[] defaultAnimationArray = new string[] { "Attack1", "Attack2", "Dead", "Hit", "Run", "Skill1", "Wait1", "Wait2" };
            List<string> defaultAnimationList = new List<string>();
            for (int i = 0; i < defaultAnimationArray.Length; i++)
            {
                defaultAnimationList.Add(defaultAnimationArray[i].ToUpper());

            }


            Texture2D combinedTex = new Texture2D(maxVertexCount, totalframeCount, TextureFormat.RGBAHalf, false);


            combinedTex.SetPixel(0, 0, new Color(arrayLength, maxVertexCount, 0));


            int countData = 1 + arrayLength;

            for (int i = 0; i < arrayLength; i++)
            {
                combinedTex.SetPixel(i + 1, 0, new Color(vertexCount[i], 0, 0));


                countData += meshArray[i].uv.Length / 2 + meshArray[i].triangles.Length / 3;
            }







            //////配置纹理 fps uv count, uv, tri count tri

            int texSize = 64; //不要问我为什么是64
            int texH = (int)Mathf.Ceil(countData / (float)texSize);
            int texW = texSize;

            Texture2D cfgTexture = new Texture2D(texW, texH, TextureFormat.RGBAHalf, false);
            Color[] colors = new Color[texW * texH];

            int cfgDataIdx = 0;
            colors[cfgDataIdx++] = new Color(arrayLength, 0, 0);

            for (int i = 0; i < arrayLength; i++)
            {
                colors[cfgDataIdx++] = new Color(pSettings.framerate, meshArray[i].uv.Length, meshArray[i].triangles.Length);

            }





            int frame = 1 + arrayLength;

            for (int submeshCount = 0; submeshCount < arrayLength; submeshCount++)
            {
                for (int i = 0; i < exportClips.Length; i++)
                {
                    if (pProgressBarUpdater != null)
                    {
                        pProgressBarUpdater((float)i / (float)(exportClips.Length + 1));
                    }
                    MeshAnimationBoneGroup boneGroup = new MeshAnimationBoneGroup(exportBoneNames.ToList(), exportBoneNames.Length);

                    // Set the animation clip to export
                    AnimationClip clip = exportClips[i];

                    if (null == clip)
                    {
                        continue;
                    }

                    int infoIdx = -1;

                    string clipName = clip.name;
                    if (clipName.Equals("walk"))
                        clipName = "Hit";
                    else if (clipName.Equals("cut"))
                        clipName = "Skill1";
                    else if (clipName.Equals("victory"))
                        clipName = "Dead";

                    string upperName = clipName.ToUpper();
                    if (defaultAnimationList.Contains(upperName))
                    {
                        infoIdx = defaultAnimationList.IndexOf(upperName);
                        defaultAnimationInfos[submeshCount][infoIdx].x = 1;

                    }

                    animation.AddClip(clip, clip.name);
                    animation.clip = clip;
                    AnimationState state = animation[clip.name];
                    state.enabled = true;
                    state.weight = 1;

                    float clipLength = clip.length;

                    //Mesh frameMesh;
                    //List<Vector3[]> frameVertices = new List<Vector3[]>();

                    // Get the list of times for each frame
                    List<float> frameTimes = GetFrameTimes(clipLength, frameInterval);

                    ////顶点纹理 r->x,g->y,b->z,a->s

                    //sample each sub mesh

                    //start frame position
                    try
                    {
                        defaultAnimationInfos[submeshCount][infoIdx].y = frame;
                    }
                    catch
                    {
                        Debug.LogError(defaultAnimationInfos.Length + "," + defaultAnimationInfos.LongLength + "," + submeshCount + "," + infoIdx);
                    }
                    foreach (float time in frameTimes)
                    {
                        state.time = time;

                        animation.Play();
                        animation.Sample();

                        // Grab the position and rotation for each bone at the current frame
                        for (int k = 0; k < exportBoneTransforms.Length; k++)
                        {
                            string name = exportBoneNames[k];

                            Vector3 pos = baseTransform.InverseTransformPoint(exportBoneTransforms[k].position);
                            Quaternion rot = exportBoneTransforms[k].rotation * Quaternion.Inverse(baseTransform.rotation);

                            boneGroup.bones[name].positions.Add(pos);
                            boneGroup.bones[name].rotations.Add(rot);


                        }

                        Mesh bakeMesh = null;

                        if (pSettings.quaternionOffset != Quaternion.identity)
                        {
                            Matrix4x4 matrix = new Matrix4x4();
                            matrix.SetTRS(Vector2.zero, pSettings.quaternionOffset, Vector3.one);
                            bakeMesh = BakeFrameAfterMatrixTransform(renderArray[submeshCount], matrix);
                        }
                        else
                        {
                            bakeMesh = new Mesh();
                            renderArray[submeshCount].BakeMesh(bakeMesh);
                        }

                        for (int k = 0; k < bakeMesh.vertexCount; k++)
                        {
                            Vector3 vertex = bakeMesh.vertices[k];
                            combinedTex.SetPixel(k, frame, new Color(vertex.x, vertex.y, vertex.z));
                        }

                        bakeMesh.Clear();
                        Object.DestroyImmediate(bakeMesh);

                        frame++;

                        animation.Stop();
                    }
                    //end frame position,exclude
                    defaultAnimationInfos[submeshCount][infoIdx].z = frame;



                }


                for (int i = 0; i < meshArray[submeshCount].uv.Length / 2; i++)
                {
                    int uvIdx = i * 2;
                    colors[cfgDataIdx++] = new Color(meshArray[submeshCount].uv[uvIdx].x, meshArray[submeshCount].uv[uvIdx].y, meshArray[submeshCount].uv[uvIdx + 1].x, meshArray[submeshCount].uv[uvIdx + 1].y);

                }
                for (int i = 0; i < meshArray[submeshCount].triangles.Length / 3; i++)
                {
                    int triIdx = i * 3;
                    colors[cfgDataIdx++] = new Color(meshArray[submeshCount].triangles[triIdx], meshArray[submeshCount].triangles[triIdx + 1], meshArray[submeshCount].triangles[triIdx + 2]);
                }

            }



            //group.AddAnimation(animationNames[i], frameVertices, boneGroup);


            for (int j = 0; j < arrayLength; j++)
            {

                for (int i = 0; i < 8; i++)
                {
                    combinedTex.SetPixel(i, j + 1, new Color(defaultAnimationInfos[j][i].x, defaultAnimationInfos[j][i].y, defaultAnimationInfos[j][i].z));
                }

            }






            combinedTex.Apply(false);

            string dataPath = pSettings.outputFilePath + "/" + "combinedTex" + ".asset";

            AssetDatabase.CreateAsset(combinedTex, dataPath);





            cfgTexture.SetPixels(colors);
            cfgTexture.Apply();



            AssetDatabase.CreateAsset(cfgTexture, pSettings.outputFilePath + "/" + pFbxInstance.name + ".asset");

            if (pProgressBarUpdater != null)
            {
                pProgressBarUpdater((float)exportClips.Length / (float)(exportClips.Length + 1));
            }

            //MeshAnimationProtobufHelper.SerializeObject<MeshAnimationGroupSerializable>(pSettings.outputFilePath + "/" + pFbxInstance.name + ".bytes", group);

            EditorUtility.DisplayDialog("Tip", "Mesh Animation Export Complete" + pFbxInstance.name, "OK");
            AssetDatabase.Refresh();


        }

        private static bool AreParametersValid(GameObject pFbx, ExportParameters pSettings)
        {
            if (string.IsNullOrEmpty(pSettings.outputFilePath.Trim()))
            {
                EditorUtility.DisplayDialog("Missing Output File", "Please set a output file.", "OK");
                return false;
            }

            if (pFbx == null)
            {
                EditorUtility.DisplayDialog("Missing Base FBX", "Please specify a base FBX.", "OK");
                return false;
            }
            if (PrefabUtility.GetPrefabParent(pFbx) == null)
            {
                EditorUtility.DisplayDialog("GameObject must be an instance of a ModelPrefab", "Please select a valid GameObject", "OK");
            }

            bool clipsNotSet = true;

            for (int i = 0; i < pSettings.animationClips.Length; i++)
            {
                if (pSettings.animationClips[i] != null && string.IsNullOrEmpty(pSettings.animationNames[i].Trim()))
                {
                    EditorUtility.DisplayDialog("Missing Animation Name", "Please specify a name for all animation files.", "OK");
                    return false;
                }

                if (pSettings.animationClips[i] != null)
                {
                    clipsNotSet = false;
                }
            }

            if (clipsNotSet)
            {
                EditorUtility.DisplayDialog("Missing Animation", "Please specify at least one animation file.", "OK");
                return false;
            }

            for (int i = 0; i < pSettings.boneTransforms.Length; i++)
            {
                if (pSettings.boneTransforms[i] != null && string.IsNullOrEmpty(pSettings.boneNames[i].Trim()))
                {
                    EditorUtility.DisplayDialog("Missing Bone Name", "Please specify a name for all bone transforms.", "OK");
                    return false;
                }
            }

            return true;
        }

        public static List<float> GetFrameTimes(float pLength, float pInterval)
        {
            List<float> times = new List<float>();

            float time = 0;

            do
            {
                times.Add(time);
                time += pInterval;
            } while (time < pLength);

            times.Add(pLength);

            return times;
        }

        public static Mesh BakeFrameAfterMatrixTransform(SkinnedMeshRenderer pRenderer, Matrix4x4 matrix)
        {
            Mesh result = new Mesh();
            pRenderer.BakeMesh(result);
            result.vertices = TransformVertices(matrix, result.vertices);
            return result;
        }

        /// <summary>
        /// Convert a set of vertices using the given transform matrix.
        /// </summary>
        /// <returns>Transformed vertices</returns>
        /// <param name="pLocalToWorld">Transform Matrix</param>
        /// <param name="pVertices">Vertices to transform</param>
        public static Vector3[] TransformVertices(Matrix4x4 pLocalToWorld, Vector3[] pVertices)
        {
            Vector3[] result = new Vector3[pVertices.Length];

            for (int i = 0; i < pVertices.Length; i++)
            {
                result[i] = pLocalToWorld * pVertices[i];
            }

            return result;
        }

        private static readonly string MESH_ANIMATIONS_ABSOLUTE_FOLDER_PATH = Path.Combine(Path.Combine(Path.Combine(Application.dataPath, "Art"), "Animations"), "MeshAnimation");
    }



    public class MeshAnimationExportWindow : EditorWindow
    {
        [MenuItem("Utility/Export Mesh Animation ")]
        static void CreateWizard()
        {
            MeshAnimationExportWindow window = EditorWindow.GetWindow<MeshAnimationExportWindow>(typeof(MeshAnimationExportWindow));
            window.ResetSettings();
            window.ShowUtility();
        }

        MeshAnimationExportWindow()
        {
            ResetSettings();
        }

        private void OnGUI()
        {
            if(null == m_OutputFolerPath)
            {
                m_OutputFolerPath = Application.streamingAssetsPath;
            }

            if(null == m_ExportParam)
            {
                ResetSettings();
            }

            m_WindowWidth = position.width;

            #region ScrollView

            m_MainScrollPosition = EditorGUILayout.BeginScrollView(m_MainScrollPosition);

            //烘培定义角度转向
            eulerAngle = EditorGUILayout.Vector3Field("Bake model after apply Rotation Offset XYZ", eulerAngle);
            EditorGUILayout.Space();

            string defaultPath = DEFAULT_OUTPUT_FOLDER + (null == m_GoFBX ? "" : (m_GoFBX.name + "/"));

            m_ExportParam.outputFilePath = m_OutputFilePath = defaultPath;
            m_ExportParam.outputFolderPath = m_OutputFolerPath = Path.GetDirectoryName(defaultPath);

            EditorGUILayout.LabelField("Default Folder:" + m_ExportParam.outputFolderPath);
            EditorGUILayout.Space();


            #region select output file
            EditorGUILayout.BeginHorizontal();
            string projectRelativeFilePath = GetAssetPath(m_OutputFilePath);

            EditorGUILayout.SelectableLabel(projectRelativeFilePath,
                                             EditorStyles.textField,
                                             GUILayout.Height(EditorGUIUtility.singleLineHeight));

            GUI.SetNextControlName("BrowseButton");
            if (GUILayout.Button("Browse"))
            {
                BrowseSaveFile();
            }
            EditorGUILayout.EndHorizontal();
            #endregion select output file

            EditorGUILayout.Space();

            #region select FBX file
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base FBX:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            GameObject newFbx = EditorGUILayout.ObjectField(m_GoFBX, typeof(GameObject), true) as GameObject;

            if (newFbx != null && newFbx != m_GoFBX)
            {
                // error if they drag the prefab itself, since it won't have any transform data
                if (PrefabUtility.GetPrefabParent(newFbx) != null)
                {
                    m_GoFBX = newFbx;
                    m_ExportParam = MeshAnimationExport.GenerateDefaultSettings(newFbx);
                    m_OutputFolerPath = m_ExportParam.outputFolderPath;
                    m_OutputFilePath = m_ExportParam.outputFilePath;
                    m_ClipCount = m_ExportParam.animationClips.Length;
                    m_BoneCount = m_ExportParam.boneTransforms.Length;

                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion select FBX file

            EditorGUILayout.Space();

            #region Framerate Setting
            m_ExportParam.framerate = EditorGUILayout.FloatField("Capture Framerate:", m_ExportParam.framerate);
            m_ExportParam.framerate = Mathf.Max(m_ExportParam.framerate, 1.0f);
            #endregion

            EditorGUILayout.Space();

            #region Clip Count Setting
            m_ClipCount = EditorGUILayout.IntField("Number of Clips:", m_ClipCount);
            ApplyChanges();
            #endregion

            EditorGUILayout.Space();

            m_LabelWidth = GUILayout.Width(m_WindowWidth * 0.35f);

            #region Animation Clip Setting
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Animation Name:", m_LabelWidth);
                EditorGUILayout.LabelField("Animation File:", m_LabelWidth);
                EditorGUILayout.LabelField("Frames:", GUILayout.Width(m_WindowWidth * 0.2f));
            }
            EditorGUILayout.EndHorizontal();

            DrawAnimationArrayGui();
            #endregion

            EditorGUILayout.Space();

            #region Bone Count Setting
            m_BoneCount = EditorGUILayout.IntField("Number of Bones:", m_BoneCount);
            ApplyChanges();
            #endregion

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bone Name:", m_LabelWidth);
            EditorGUILayout.LabelField("Bone Transform:", m_LabelWidth);
            EditorGUILayout.EndHorizontal();

            DrawBoneArrayGui();

            #region Clear and Save Buttons
            EditorGUILayout.BeginHorizontal();
            GUI.SetNextControlName("ClearButton");
            if (GUILayout.Button("Clear"))
            {
                ResetSettings();
                GUI.FocusControl("ClearButton");
            }

            if (GUILayout.Button("Export"))
            {
                //Save 
                if (eulerAngle != Vector3.zero)
                {
                    m_ExportParam.quaternionOffset = Quaternion.Euler(eulerAngle);
                }

                //MeshAnimationExporter.Export(fbx, exportSettings);
                MeshAnimationExport.ExportCombinedTexture(m_GoFBX, m_ExportParam);
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            EditorGUILayout.EndScrollView();

            #endregion ScrollView
        }

        private void ApplyChanges()
        {
            if (Event.current.isKey)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        Event.current.Use();
                        MeshAnimationExport.ResizeDataLists(ref m_ExportParam.boneTransforms, ref m_ExportParam.boneNames, m_BoneCount);
                        MeshAnimationExport.ResizeDataLists(ref m_ExportParam.animationClips, ref m_ExportParam.animationNames, m_ClipCount);
                        break;
                }
            }
        }

        private void ResetSettings()
        {
            m_ExportParam = new ExportParameters();
        }

        private string GetProjectPath()
        {
            return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        }

        /// <summary>
        /// Get the relative project file path from an absolute path
        /// </summary>
        /// <returns>The absolute file path</returns>
        /// <param name="pAbsolutePath">Absolute file path</param>
        public string GetAssetPath(string pAbsolutePath)
        {
            string projectPath = GetProjectPath();
            if (pAbsolutePath.StartsWith(projectPath))
            {
                string relativePath = pAbsolutePath.Substring(projectPath.Length, pAbsolutePath.Length - projectPath.Length);

                if (relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
                {
                    relativePath = relativePath.Substring(1, relativePath.Length - 1);
                }

                return relativePath;
            }

            return null;
        }

        private void BrowseSaveFile()
        {
            string output = EditorUtility.SaveFilePanel(
                "Save binary outpout",
                m_OutputFolerPath,
                "",
                "bytes"
            );

            if (!string.IsNullOrEmpty(output.Trim()))
            {
                m_ExportParam.outputFilePath = m_OutputFilePath = output;
                m_ExportParam.outputFolderPath = m_OutputFolerPath = Path.GetDirectoryName(output);
            }

            GUI.FocusControl("");
        }

        private void DrawAnimationArrayGui()
        {
            float interval = 1.0f / m_ExportParam.framerate;

            if (m_ExportParam.animationNames != null && m_ExportParam.animationNames.Length > 0)
            {
                for (int i = 0; i < m_ExportParam.animationNames.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    m_ExportParam.animationNames[i] = EditorGUILayout.TextField(m_ExportParam.animationNames[i], m_LabelWidth);

                    AnimationClip clip = EditorGUILayout.ObjectField(m_ExportParam.animationClips[i], typeof(AnimationClip), true, m_LabelWidth) as AnimationClip;

                    if (clip != m_ExportParam.animationClips[i] && clip != null)
                    {
                        m_ExportParam.animationNames[i] = clip.name;
                    }

                    m_ExportParam.animationClips[i] = clip;

                    float frameCount = 0;

                    if (clip != null)
                    {
                        frameCount = clip.length / interval;
                    }

                    EditorGUILayout.LabelField(frameCount.ToString(), GUILayout.Width(m_WindowWidth * 0.2f));

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawBoneArrayGui()
        {

            if (m_ExportParam.boneNames != null && m_ExportParam.boneNames.Length > 0)
            {
                for (int i = 0; i < m_ExportParam.boneNames.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    m_ExportParam.boneNames[i] = EditorGUILayout.TextField(m_ExportParam.boneNames[i], m_LabelWidth);

                    Transform bone = EditorGUILayout.ObjectField(m_ExportParam.boneTransforms[i], typeof(Transform), true, m_LabelWidth) as Transform;

                    if (bone != m_ExportParam.boneTransforms[i] && bone != null)
                    {
                        m_ExportParam.boneNames[i] = bone.name;
                    }

                    m_ExportParam.boneTransforms[i] = bone;

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private ExportParameters m_ExportParam;
        private string m_OutputFilePath;
        private string m_OutputFolerPath;

        private float m_WindowWidth;
        private Vector2 m_MainScrollPosition = Vector2.zero;
        private GUILayoutOption m_LabelWidth;

        private int m_ClipCount;
        private int m_BoneCount;

        private GameObject m_GoFBX;

        /// <summary>
        /// 烘焙定义欧拉角度
        /// </summary>
        public static Vector3 eulerAngle = Vector3.zero;

        public const string DEFAULT_OUTPUT_FOLDER = "Assets/Art/Animations/MeshAnimation/";
    }


    public static class PGEditorToolsStringExtensions
    {
        /// <summary>
        /// Converts a string from camel-case to seperate words that start with 
        /// capital letters. Also removes leading underscores.
        /// </summary>
        /// <returns>string</returns>
        public static string DeCamel(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            System.Text.StringBuilder newStr = new System.Text.StringBuilder();

            char c;
            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];

                // Handle spaces and underscores. 
                //   Do not keep underscores
                //   Only keep spaces if there is a lower case letter next, and 
                //       capitalize the letter
                if (c == ' ' || c == '_')
                {
                    // Only check the next character is there IS a next character
                    if (i < s.Length - 1 && char.IsLower(s[i + 1]))
                    {
                        // If it isn't the first character, add a space before it
                        if (newStr.Length != 0)
                        {
                            newStr.Append(' ');  // Add the space
                            newStr.Append(char.ToUpper(s[i + 1]));
                        }
                        else
                        {
                            newStr.Append(s[i + 1]);  // Stripped if first char in string
                        }

                        i++;  // Skip the next. We already used it
                    }
                }  // Handle uppercase letters
                else if (char.IsUpper(c))
                {
                    // If it isn't the first character, add a space before it
                    if (newStr.Length != 0)
                    {
                        newStr.Append(' ');
                        newStr.Append(c);
                    }
                    else
                    {
                        newStr.Append(c);
                    }
                }
                else  // Normal character. Store and move on.
                {
                    newStr.Append(c);
                }
            }

            return newStr.ToString();
        }


        /// <summary>
        /// Capitalizes only the first letter of a string
        /// </summary>
        /// <returns>string</returns>
        public static string Capitalize(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

    }
}
