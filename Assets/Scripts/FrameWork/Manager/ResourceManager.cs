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

        private static void LoadGameObject(string type, string name, Transform parent, bool async, LoadCallback callback, bool unload = true)
        { }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
