using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
    public static class PathHelper
    {
        private static Dictionary<ResourcesType, string[]> m_DicResPath = new Dictionary<ResourcesType, string[]>(new ResourcesTypeComparer());

        static PathHelper()
        {
            RegisterPath();
        }

        public static string GetResourcePath(ResourcesType type, ResourcesPathMode mode)
        {
            int index = (int)mode;
            string [] path;
            if(!m_DicResPath.TryGetValue(type, out path))
            {
                Debug.LogError(" GetResourcePath Error : " + type.ToString());
                return string.Empty;
            }

            return path[index];
        }

        public static string GetFileExtension(ResourcesType type)
        {
            return GetResourcePath(type, ResourcesPathMode.Extension);
        }

        private static void RegisterPath()
        {
            m_DicResPath.Add(ResourcesType.UIWnd, new string[]{ "Data/wnd/panel/", "wnd/panel/", ".prefab"});
            m_DicResPath.Add(ResourcesType.UIWndItem, new string[] { "Data/wnd/items/", "wnd/items/", ".prefab" });
            m_DicResPath.Add(ResourcesType.UIAtlas, new string[] { "Data/Atlas/Items/", "atlas/item/", ".png" });
            m_DicResPath.Add(ResourcesType.UIPublicWndAtlas, new string[] { "Data/Atlas/WndPublic/", "atlas/wndpublic/", ".png" });
            m_DicResPath.Add(ResourcesType.SceneItem, new string[] { "Prefabs/sceneItem/", "SceneItem/", ".prefab" });


            m_DicResPath.Add(ResourcesType.ActorHero, new string[] { "Prefabs/Actor/Hero/", "hero/prefab/", ".prefab" });
            m_DicResPath.Add(ResourcesType.ActorShowHero, new string[] { "Prefabs/Actor/HeroShow/", "showhero/prefab/", ".prefab" });
            m_DicResPath.Add(ResourcesType.HeroAnim, new string[] { "Animations/Units/Hero/", "hero/anim/", ".anim" });

            m_DicResPath.Add(ResourcesType.ActorSoldier, new string[] { "Prefabs/Actor/Soldier/", "soldier/prefab/", ".prefab" });
            m_DicResPath.Add(ResourcesType.ActorSoldierMaterial, new string[] { "Art/Animations/Material/", "soldier/material/", ".mat" });
            m_DicResPath.Add(ResourcesType.ActorSoldierMesh, new string[] { "Art/Animations/MeshAnimation/", "soldier/meshanimation/", ".asset" });
            m_DicResPath.Add(ResourcesType.ActorSoldierAnim, new string[] { "Art/Animations/MeshAnimation/", "soldier/meshanimation/", ".asset" });


            m_DicResPath.Add(ResourcesType.Building, new string[] { "Prefabs/Building/", "building/", ".prefab" });
            m_DicResPath.Add(ResourcesType.BuildingModel, new string[] { "Prefabs/BuildingModel/", "buildingmodel/", ".prefab" });
            m_DicResPath.Add(ResourcesType.BuildingSite, new string[] { "Prefabs/BuildingSite/", "buildingsite/", ".prefab" });
            m_DicResPath.Add(ResourcesType.CityTree, new string[] { "Prefabs/TreeBlock/", "tree/", ".prefab" });
            m_DicResPath.Add(ResourcesType.BuildingTexture, new string[] { "Models/Environment/Models/Environment/COL3Building/Textures/", "BuildingTexture/", ".tga" });
            m_DicResPath.Add(ResourcesType.PngTexture, new string[] { "Textures/CommonPng/", "CommonTextures/Png/", ".png" });

            m_DicResPath.Add(ResourcesType.Effect, new string[] { "Prefabs/Effect/", "effect/", ".prefab" });
            m_DicResPath.Add(ResourcesType.Skill, new string[] { "Prefabs/Skill/", "skill/", ".prefab" });
            m_DicResPath.Add(ResourcesType.Audio, new string[] { "Data/Audio/", "Audio/", ".ogg" });
            m_DicResPath.Add(ResourcesType.Shader, new string[] { "Shader/Core/", "shader/", ".shader" });
            m_DicResPath.Add(ResourcesType.Map, new string[] { "Prefabs/Map/", "map/", ".prefab" });
            m_DicResPath.Add(ResourcesType.Scene, new string[] { "Scene/", "scene/", ".unity" });
            m_DicResPath.Add(ResourcesType.MapData, new string[] { "Data/MapData/", "mapdata/", ".bytes" });
            m_DicResPath.Add(ResourcesType.luaData, new string[] { "Scripts/lua/", "lua/", ".lua" });
            m_DicResPath.Add(ResourcesType.ComMaterial, new string[] { "Materials/Common/", "common/material/", ".mat" });

            m_DicResPath.Add(ResourcesType.Depend, new string[] { "depend", "depend/", ".prefab" });

            m_DicResPath.Add(ResourcesType.Config, new string[] { "Data/config/", "config/", ".asset" });
        }
    }

    public enum ResourcesPathMode
    {
        Editor,
        AssetBundle,
        Extension
    }

    class ResourcesTypeComparer : IComparer<ResourcesType>
    {
        public int Compare(ResourcesType x, ResourcesType y)
        {
            if (x > y)
                return -1;
            else if (x == y)
                return 0;

            return 1;
        }
    }

    public enum ResourcesType
    {
        None,

        UIWnd,
        UIWndItem,
        UIAtlas,
        UIPublicWndAtlas,
        SceneItem,

        // hero
        ActorHero,
        ActorShowHero,
        HeroAnim,

        // soldier
        ActorSoldier,
        ActorSoldierMaterial,
        ActorSoldierMesh,
        ActorSoldierAnim,

        Building,
        BuildingModel,
        BuildingSite,
        CityTree,
        BuildingTexture,
        PngTexture,

        Effect,
        Skill,
        Audio,
        Shader,
        Map,
        Scene,
        MapData,
        luaData,
        ComMaterial,

        Depend,     //LightMapPath

        Config,

        Quantity
    }
}
