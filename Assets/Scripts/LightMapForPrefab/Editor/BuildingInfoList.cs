using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[CreateAssetMenu]
public class BuildingInfoList : ScriptableObject
{

    [SerializeField]
    public List<BuildingInfo> BuildingInfos = new List<BuildingInfo>();
    public List<string> BuildingNames = new List<string>();

}

[Serializable]
public class BuildingInfo
{
   
    public List<string> AnchorNames = new List<string>();
  
    public List<Vector3> Position = new List<Vector3>();

}
