using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FrameWork
{
	public class AssetBundleObject : IDisposable
	{
		private ReferenceAssetBundle m_Owner;

		private UnityEngine.Object m_Object;
		public UnityEngine.Object GetObject()
		{
			return m_Object;
		}

		public AssetBundleObject(UnityEngine.Object obj, ReferenceAssetBundle owner)
		{
			m_Object = obj;
			m_Owner = owner;
		}

		public void Dispose()
		{
			m_Object = null;
			m_Owner.Dispose();
		}
	}
}
