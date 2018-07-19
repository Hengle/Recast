using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Utility;
using System;
using UnityEngine.Events;

namespace FrameWork.Manager
{
	public class ResourceManager : Singleton<ResourceManager>
    {
        [Serializable]
        public class LoadCallback : UnityEvent<GameObject> { }

        private static MonoBehaviour s_AsyncGo;
        private static Transform s_AsyncTransform;
        public static MonoBehaviour AsyncGo { get { return s_AsyncGo; } set { s_AsyncGo = value; if (null != value) s_AsyncTransform = value.transform; } }

        private static Dictionary<string, PrefabInfo> m_Prefabs = new Dictionary<string, PrefabInfo>();

        private static Dictionary<string, List<GameObject>> m_Pools = new Dictionary<string, List<GameObject>>();

        public static void LoadGameObject(ResourcesType type, string name, Transform parent, bool async, LoadCallback callback, bool unload = true)
        {
            string path = PathHelper.GetResourceFullPath(type, name);

            PrefabInfo prefabInfo = null;
            if(m_Prefabs.TryGetValue(path, out prefabInfo))
            {
            }
        }

        private static GameObject PopGameObject(string path, PrefabInfo prefabInfo, Transform parent, LoadCallback callback)
        {
            GameObject go = null;
            List<GameObject> pool = null;
            if(m_Pools.TryGetValue(path, out pool))
            {
                if(pool.Count > 0)
                {
                    go = pool[0];
                    pool.RemoveAt(0);
                    go.transform.SetParent(parent);
                }
            }

            if(null == go)
            {
                go = GameObject.Instantiate(prefabInfo.prefab, parent);
                go.name = path;
            }

            go.SetActive(true);

            ++prefabInfo.count;

            if(null != callback)
            {
                callback.Invoke(go);
            }

            return go;
        }

        public static void PushGameObject(GameObject go, bool destroy = false)
        {
            if (null == go)
                return;

            PrefabInfo prefabInfo = null;
            if(m_Prefabs.TryGetValue(go.name, out prefabInfo))
            {
                --prefabInfo.count;
            }

            if(!destroy && null != s_AsyncGo)
            {
                go.transform.SetParent(s_AsyncTransform);
                go.transform.localPosition = prefabInfo.position;
                go.transform.localEulerAngles = prefabInfo.rotation;
                go.transform.localScale = prefabInfo.scale;
                go.SetActive(false);

                List<GameObject> pool;
                if(!m_Pools.TryGetValue(go.name, out pool))
                {
                    pool = new List<GameObject>();
                    m_Pools.Add(go.name, pool);
                }

                pool.Add(go);
            }
        }

        static void ClearPool()
        {
            foreach(var pool in m_Pools)
            {
                foreach(var go in pool.Value)
                {
                    GameObject.Destroy(go);
                }

                pool.Value.Clear();
            }
            m_Pools.Clear();

            foreach(var prefab in m_Prefabs)
            { }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public class PrefabInfo
        {
            public GameObject prefab;
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public int count;
        }
    }
}
