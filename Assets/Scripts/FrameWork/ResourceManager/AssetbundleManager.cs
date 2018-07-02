using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.Utility;

namespace FrameWork
{
	public class AssetbundleManager : Singleton<AssetbundleManager>
	{
		private Dictionary<string, ReferenceAssetBundle> m_LoadedAssetBundles = new Dictionary<string, ReferenceAssetBundle>();

		public AssetBundleObject LoadAssetBundle(string name)
		{
			ReferenceAssetBundle assetBundle;
			if(m_LoadedAssetBundles.TryGetValue(name, out assetBundle))
			{
				
			}
		}
	}
}

