using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor.Map
{
    public class GenerateMapPanel : ScriptableWizard
    {
        // 水平格子数    X
        [SerializeField] private int horizontalGridNumber = 144;

        // 垂直格子数    Z
        [SerializeField] private int verticalGridNumber = 172;

        // 水平格子单位   X
        [SerializeField] private float horizontalGridUnit = 1.0f;

        // 垂直格子单位   Z
        [SerializeField] private float verticalGridUnit = 1.0f;

        // 校准
        [SerializeField] private MapAnchor alignment = MapAnchor.MiddleCenter;

        //private const string prefabPath = "Assets/Editor/Tools/Map/GridMap.prefab";

        private const string mapResPath = "Assets/Editor/Tools/Map/Map Res/";
        private const string prefabName = "GridMap.prefab";

        [MenuItem("Map/Generate Map Panel")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<GenerateMapPanel>("Generate Map Panel");
        }

        private void OnWizardCreate()
        {
            float width = horizontalGridNumber * horizontalGridUnit;
            float height = verticalGridNumber * verticalGridUnit;

            GameObject mapObj = Instantiate(AssetDatabase.LoadAssetAtPath(mapResPath + prefabName, typeof(GameObject)) as GameObject);
            mapObj.name = "Map";
            GameObject plane = mapObj.transform.Find("MapPlane").gameObject;
            if(null == plane)
            {
                return;
            }

            plane.transform.position = new Vector3(horizontalGridNumber / 2, 0, verticalGridNumber / 2);

            MeshFilter meshFilter = plane.GetComponent<MeshFilter>();
            MeshFilter terrianMeshFilter = plane.transform.Find("Terrian").GetComponent<MeshFilter>();

            string assetname = mapObj.name + "W" + width + "H" + height + alignment.ToString() + ".asset";
            Mesh mesh = AssetDatabase.LoadAssetAtPath(mapResPath + assetname, typeof(Mesh)) as Mesh;
            if(null == mesh)
            {
                mesh = GenerateMesh(horizontalGridNumber, verticalGridNumber, horizontalGridUnit, verticalGridUnit, alignment);
            }

            meshFilter.sharedMesh = mesh;
            terrianMeshFilter.sharedMesh = mesh;
            mesh.RecalculateBounds();
            plane.AddComponent<BoxCollider>();

            MeshRenderer ren = plane.GetComponent<MeshRenderer>();
            ren.sharedMaterial.SetTextureScale("_MainTex", new Vector2(horizontalGridNumber, verticalGridNumber));
            ren.sharedMaterial.SetVector("_Bound", new Vector4(0, verticalGridNumber, horizontalGridNumber, verticalGridNumber));

            Selection.activeObject = plane;
        }
        
        /// <summary>
        /// 生成网格 mesh
        /// </summary>
        /// <param name="horGridNum"> 水平格子数 </param>
        /// <param name="vertGridNum"> 竖直格子数 </param>
        /// <param name="horGridUnit"> 水平格子单位 </param>
        /// <param name="vertGridUnit"> 竖直格子单位 </param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        private Mesh GenerateMesh(int horGridNum, int vertGridNum, float horGridUnit, float vertGridUnit, MapAnchor alignment)
        {
            float width = horizontalGridNumber * horizontalGridUnit;
            float height = verticalGridNumber * verticalGridUnit;
            Vector2 anchorOffset = Vector2.zero;
            switch (alignment)
            {
                case MapAnchor.UpperLeft:
                    anchorOffset = new Vector2(0, -height);
                    break;
                case MapAnchor.UpperCenter:
                    anchorOffset = new Vector2(-width / 2, -height);
                    break;
                case MapAnchor.UpperRight:
                    anchorOffset = new Vector2(-width, -height);
                    break;
                case MapAnchor.MiddleLeft:
                    anchorOffset = new Vector2(0, -height / 2);
                    break;
                case MapAnchor.MiddleCenter:
                    anchorOffset = new Vector2(-width / 2, -height / 2);
                    break;
                case MapAnchor.MiddleRight:
                    anchorOffset = new Vector2(-width, -height / 2);
                    break;
                case MapAnchor.LowerLeft:
                    anchorOffset = new Vector2(0, 0);
                    break;
                case MapAnchor.LowerCenter:
                    anchorOffset = new Vector2(-width / 2, 0);
                    break;
                case MapAnchor.LowerRight:
                    anchorOffset = new Vector2(-width, 0);
                    break;
            }

            Mesh mesh = new Mesh();
            int horVerticeNum = horGridNum + 1;
            int vertVerticeNum = vertGridNum + 1;
            int triangleCount = horGridNum * vertGridNum * 6;
            int verticeCount = horVerticeNum * vertVerticeNum;

            Vector3[] vertices = new Vector3[verticeCount];
            Vector2[] uvs = new Vector2[verticeCount];
            Vector4[] tangents = new Vector4[verticeCount];
            int[] triangles = new int[triangleCount];
            Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

            int index = 0;
            float uvFactorWidth = 1.0f / horGridNum;
            float uvFactorHeight = 1.0f / vertGridNum;
            for (int i = 0; i < vertVerticeNum; ++ i)
            {
                for(int j = 0; j < horVerticeNum; ++ j)
                {
                    vertices[index] = new Vector3( j * horGridUnit + anchorOffset.x, 0, i * vertGridUnit + anchorOffset.y);
                    uvs[index] = new Vector2(j * uvFactorWidth, i * uvFactorHeight);
                    tangents[index] = tangent;
                    ++index;
                }
            }

            index = 0;
            for(int i = 0; i < vertGridNum; ++ i)
            {
                for(int j = 0; j < horGridNum; ++ j)
                {
                    triangles[index] = j + (1 + i) * horVerticeNum;
                    triangles[index + 1] = j + 1 + (i + 1) * horVerticeNum;
                    triangles[index + 2] = j + i * horVerticeNum; 

                    triangles[index + 3] = j + 1 + (1 + i) * horVerticeNum;
                    triangles[index + 4] = j + 1 + i * horVerticeNum;
                    triangles[index + 5] = j + i * horVerticeNum; 
                    index += 6;
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.tangents = tangents;
            mesh.RecalculateNormals();

            string assetname = "Map" + "W" + horGridNum + "H" + vertGridNum + alignment.ToString() + ".asset";
            AssetDatabase.CreateAsset(mesh, mapResPath + assetname);
            AssetDatabase.SaveAssets();

            return mesh;
        }
    }
}
