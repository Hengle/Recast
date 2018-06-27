using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FrameWork
{
	public class ReferenceAssetBundle : IDisposable
	{
		private AssetBundle m_AssetBundle;

		private uint m_Count;

		public ReferenceAssetBundle(AssetBundle assetBundle)
		{
			m_AssetBundle = assetBundle;
		}

		public void Dispose()
		{
			if(--m_Count == 0)
			{
				m_AssetBundle.Unload(true);
			}
		}

		public AssetBundleObject LoadAsset(string name)
		{
			UnityEngine.Object obj = m_AssetBundle.LoadAsset(name);
			if(null == obj)
			{
				Debug.Log(string.Format("AssetBundle [{0}]   LoadAsset With Empty Object  :  {0}", m_AssetBundle.name, name));
			}

			return new AssetBundleObject(obj, this);
		}
		public AssetBundleObject LoadAsset<T>(string name)
		{
			T obj = m_AssetBundle.LoadAsset(name);
		}
	}
}
