using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject m_DepthCameraObj;

    private Camera m_Camera;

    private Camera m_CameraDepth;

    private Shader m_ShaderDepth;

    private Material m_Material;

    private RenderTexture m_TextureDepth;

    private Shader m_ShaderCopy;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();

        m_ShaderDepth = Shader.Find("Custom/Depth");
        m_ShaderCopy = Shader.Find("Custom/CopyDepth");
        m_Material = new Material(m_ShaderDepth);
        
        m_DepthCameraObj = new GameObject("DepthCamera");
        m_DepthCameraObj.AddComponent<Camera>();
        m_CameraDepth = m_DepthCameraObj.GetComponent<Camera>();
        m_CameraDepth.enabled = false;
    }

    // Use this for initialization
    void Start () {
        m_Camera.depthTextureMode = DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void OnPreRender()
    {
        if (m_TextureDepth)
        {
            RenderTexture.ReleaseTemporary(m_TextureDepth);
            m_TextureDepth = null;
        }

        m_CameraDepth.CopyFrom(m_Camera);
        m_TextureDepth = RenderTexture.GetTemporary(m_Camera.pixelWidth, m_Camera.pixelHeight, 16, RenderTextureFormat.ARGB32);
        m_CameraDepth.backgroundColor = new Color(0, 0, 0, 0);
        m_CameraDepth.clearFlags = CameraClearFlags.SolidColor; ;
        m_CameraDepth.targetTexture = m_TextureDepth;
        m_CameraDepth.RenderWithShader(m_ShaderCopy, "RenderType");
        m_Material.SetTexture("_DepthTexture", m_TextureDepth);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (null != m_Material)
        {
            Graphics.Blit(source, destination, m_Material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
