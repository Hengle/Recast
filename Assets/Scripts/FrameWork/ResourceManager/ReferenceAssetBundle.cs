﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork
{
	public class ReferenceAssetBundle : System.IDisposable
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

		public bool Cantain(string name)
		{
			return m_AssetBundle.Contains(name);
		}

		public string[] GetAllAssetNames()
		{
			return m_AssetBundle.GetAllAssetNames();
		}

		public string[] GetAllScenePaths()
		{
			return m_AssetBundle.GetAllScenePaths();
		}

		public Object[] LoadAllAssets()
		{
			return m_AssetBundle.LoadAllAssets();
		}

		public Object[] LoadAllAssets(System.Type type)
		{
			return m_AssetBundle.LoadAllAssets(type);
		}

		public Object[] LoadAllAssets<T>()
		{
			return m_AssetBundle.LoadAllAssets(typeof(T));
		}
		public AssetBundleRequest LoadAllAssetsAsync()
		{
			return m_AssetBundle.LoadAllAssetsAsync();
		}

		public AssetBundleRequest LoadAllAssetsAsync(System.Type type)
		{
			return m_AssetBundle.LoadAllAssetsAsync(type);
		}

		public AssetBundleRequest LoadAllAssetsAsyncAsync<T>()
		{
			return m_AssetBundle.LoadAllAssetsAsync(typeof(T));
		}

		public AssetBundleObject LoadAsset(string name)
		{
			Object obj = m_AssetBundle.LoadAsset(name);
			if(null == obj)
			{
				Debug.Log(string.Format("AssetBundle [{0}]   LoadAsset With Empty Object  :  {1}", m_AssetBundle.name, name));
			}

			return new AssetBundleObject(obj, this);
		}

		public AssetBundleObject LoadAsset(string name, System.Type type)
		{
			Object obj = m_AssetBundle.LoadAsset(name, type);
			if(null == obj)
			{
				Debug.Log(string.Format("AssetBundle [{0}]   LoadAsset With Empty Object  :  {1}", m_AssetBundle.name, name));
			}

			return new AssetBundleObject(obj, this);
		}

		public AssetBundleObject LoadAsset<T>(string name)
		{
			return LoadAsset(name, typeof(T));
		}

		public AssetBundleRequest LoadAssetAsync(string name)
		{
			return m_AssetBundle.LoadAssetAsync(name);
		}

		public AssetBundleRequest LoadAssetAsync(string name, System.Type type)
		{
			return m_AssetBundle.LoadAssetAsync(name, type);
		}

		public AssetBundleRequest LoadAssetAsync<T>(string name)
		{
			return LoadAssetAsync(name, typeof(T));
		}
		
		        //
        // Summary:
        //     ///
        //     Loads asset and sub assets with name of a given type from the bundle.
        //     ///
        //
        // Parameters:
        //   name:
        //
        //   type:
        public Object[] LoadAssetWithSubAssets(string name, System.Type type)
		{
			return m_AssetBundle.LoadAssetWithSubAssets(name, type);
		}
        public T[] LoadAssetWithSubAssets<T>(string name) where T : Object
		{
			return m_AssetBundle.LoadAssetWithSubAssets<T>(name);
		}
        //
        // Summary:
        //     ///
        //     Loads asset and sub assets with name of type T from the bundle.
        //     ///
        //
        // Parameters:
        //   name:
        public Object[] LoadAssetWithSubAssets(string name)
		{
			return m_AssetBundle.LoadAssetWithSubAssets(name);
		}
        public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name)
		{
			return m_AssetBundle.LoadAssetWithSubAssetsAsync<T>(name);
		}
        //
        // Summary:
        //     ///
        //     Loads asset with sub assets with name of type T from the bundle asynchronously.
        //     ///
        //
        // Parameters:
        //   name:
        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
		{
			return m_AssetBundle.LoadAssetWithSubAssetsAsync(name);
		}
        //
        // Summary:
        //     ///
        //     Loads asset with sub assets with name of a given type from the bundle asynchronously.
        //     ///
        //
        // Parameters:
        //   name:
        //
        //   type:
        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, System.Type type)
		{
			return m_AssetBundle.LoadAssetWithSubAssetsAsync(name, type);
		}
        
	}
}
