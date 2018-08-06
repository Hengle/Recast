using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace FrameWork
{
    public static class PathHelper
    {
        public static string GetResourcePath(ResourcesType type, ResourcesPathMode mode)
        {
            int index = (int)mode;
            string[] path = null;
            switch (type)
            {
                case ResourcesType.UIWnd:
                    path = UIWndPath;
                    break;
                case ResourcesType.UIWndItem:
                    path = UIWndItemPath;
                    break;
                case ResourcesType.UIAtlas:
                    path = UIAtlasPath;
                    break;
                case ResourcesType.UIPublicWndAtlas:
                    path = UIPublicAtlasPath;
                    break;
                case ResourcesType.SceneItem:
                    path = SceneItemPath;
                    break;

                // hero
                case ResourcesType.ActorHero:
                    path = ActorHeroPath;
                    break;
                case ResourcesType.ActorShowHero:
                    path = ActorHeroShowPath;
                    break;
                case ResourcesType.HeroAnim:
                    path = HeroAnimPath;
                    break;

                // soldier
                case ResourcesType.ActorSoldier:
                    path = ActorSoldierPath;
                    break;
                case ResourcesType.ActorSoldierMaterial:
                    path = ActorSoldierMaterialPath;
                    break;
                case ResourcesType.ActorSoldierMesh:
                    path = ActorSoldierMeshPath;
                    break;
                case ResourcesType.ActorSoldierAnim:
                    path = ActorSoldierAnimPath;
                    break;

                case ResourcesType.Building:
                    path = BuildingPath;
                    break;
                case ResourcesType.BuildingModel:
                    path = BuildingModelPath;
                    break;
                case ResourcesType.BuildingSite:
                    path = BuildingSitePath;
                    break;
                case ResourcesType.CityTree:
                    path = CityTreePath;
                    break;
                case ResourcesType.BuildingTexture:
                    path = BuildingTexturePath;
                    break;
                case ResourcesType.PngTexture:
                    path = PngTexturePath;
                    break;

                case ResourcesType.Effect:
                    path = EffectPath;
                    break;
                case ResourcesType.Skill:
                    path = SkillPath;
                    break;
                case ResourcesType.Audio:
                    path = AudioPath;
                    break;
                case ResourcesType.Shader:
                    path = ShaderPath;
                    break;
                case ResourcesType.Map:
                    path = MapPath;
                    break;
                case ResourcesType.Scene:
                    path = ScenePath;
                    break;
                case ResourcesType.MapData:
                    path = MapDataPath;
                    break;
                case ResourcesType.LuaData:
                    path = LuaDataPath;
                    break;
                case ResourcesType.ComMaterial:
                    path = ComMaterialPath;
                    break;
                case ResourcesType.Depend:  //LightMapPath
                    path = DependPath;
                    break;
                case ResourcesType.Config:
                    path = ConfigPath;
                    break;
            }

            if (null == path)
                return string.Empty;

            return path[index];
        }

        public static string GetFileExtension(ResourcesType type)
        {
            return GetResourcePath(type, ResourcesPathMode.Extension);
        }

        public static string GetResourceFullPath(ResourcesType type, string name)
        {
            string path = GetResourcePath(type, ResourcesPathMode.Editor);
            string extension = GetFileExtension(type);

            return string.Format("{0}{1}{2}", path, name, extension);
        }

        private static readonly string[] UIWndPath = { "Data/wnd/panel/", "wnd/panel/", ".prefab" };
        private static readonly string[] UIWndItemPath = { "Data/wnd/items/", "wnd/items/", ".prefab" };
        private static readonly string[] UIAtlasPath = { "Data/Atlas/Items/", "atlas/item/", ".png" };
        private static readonly string[] UIPublicAtlasPath = { "Data/Atlas/WndPublic/", "atlas/wndpublic/", ".png" };
        private static readonly string[] SceneItemPath = { "Prefabs/sceneItem/", "SceneItem/", ".prefab" };

        private static readonly string[] ActorHeroPath = { "Prefabs/Actor/Hero/", "hero/prefab/", ".prefab" };
        private static readonly string[] ActorHeroShowPath = { "Prefabs/Actor/HeroShow/", "showhero/prefab/", ".prefab" };
        private static readonly string[] HeroAnimPath = { "Animations/Units/Hero/", "hero/anim/", ".anim" };

        private static readonly string[] ActorSoldierPath = { "Prefabs/Actor/Soldier/", "soldier/prefab/", ".prefab" };
        private static readonly string[] ActorSoldierMaterialPath = { "Art/Animations/Material/", "soldier/material/", ".mat" };
        private static readonly string[] ActorSoldierMeshPath = { "Art/Animations/MeshAnimation/", "soldier/meshanimation/", ".asset" };
        private static readonly string[] ActorSoldierAnimPath = { "Art/Animations/MeshAnimation/", "soldier/meshanimation/", ".asset" };

        private static readonly string[] BuildingPath = { "Prefabs/Building/", "building/", ".prefab" };
        private static readonly string[] BuildingModelPath = { "Prefabs/BuildingModel/", "buildingmodel/", ".prefab" };
        private static readonly string[] BuildingSitePath = { "Prefabs/BuildingSite/", "buildingsite/", ".prefab" };
        private static readonly string[] CityTreePath = { "Prefabs/TreeBlock/", "tree/", ".prefab" };
        private static readonly string[] BuildingTexturePath = { "Models/Environment/Models/Environment/COL3Building/Textures/", "BuildingTexture/", ".tga" };
        private static readonly string[] PngTexturePath = { "Textures/CommonPng/", "CommonTextures/Png/", ".png" };

        private static readonly string[] EffectPath = { "Prefabs/Effect/", "effect/", ".prefab" };
        private static readonly string[] SkillPath = { "Prefabs/Skill/", "skill/", ".prefab" };
        private static readonly string[] AudioPath = { "Data/Audio/", "Audio/", ".ogg" };
        private static readonly string[] ShaderPath = { "Shader/Core/", "shader/", ".shader" };
        private static readonly string[] MapPath = { "Prefabs/Map/", "map/", ".prefab" };
        private static readonly string[] ScenePath = { "Scene/", "scene/", ".unity" };
        private static readonly string[] MapDataPath = { "Data/MapData/", "mapdata/", ".bytes" };
        private static readonly string[] LuaDataPath = { "Scripts/lua/", "lua/", ".lua" };
        private static readonly string[] ComMaterialPath = { "Materials/Common/", "common/material/", ".mat" };

        private static readonly string[] DependPath = { "depend", "depend/", ".prefab" };

        private static readonly string[] ConfigPath = { "Data/config/", "config/", ".asset" };
    }

    public enum ResourcesPathMode
    {
        Editor,
        AssetBundle,
        Extension
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
        LuaData,
        ComMaterial,

        Depend,     //LightMapPath

        Config,

        Quantity
    }
}
