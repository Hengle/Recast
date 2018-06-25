using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FrameWork.Editor.Map
{
    public class MapMeshMgr 
    {
        static string tagName = "BigMap";
        public static void ModifyMeshCollider(bool add)
        {
            var tagObj = GameObject.FindGameObjectsWithTag(tagName);

            foreach (var tag in tagObj)
            {
                var rens = tag.GetComponentsInChildren<MeshRenderer>();

                foreach (var ren in rens)
                {

                    MeshCollider meshCol = null;
                    meshCol = ren.gameObject.GetComponent<MeshCollider>();
                    if (add)
                    {


                        if (meshCol == null)
                        {
                            MeshFilter meshFilter = ren.gameObject.GetComponent<MeshFilter>();
                            if (meshFilter == null)
                                continue;

                            Mesh curMesh = meshFilter.sharedMesh;
                            meshCol = ren.gameObject.AddComponent<MeshCollider>();
                            meshCol.sharedMesh = curMesh;

                        }

                    }
                    else
                    {

                        if (meshCol != null)
                            GameObject.DestroyImmediate(meshCol);
                    }
                }

            }

            Debug.Log("modify mesh col finished");
        }



        














    }






}

