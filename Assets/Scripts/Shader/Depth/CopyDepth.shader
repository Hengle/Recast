// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// 自定的深度获取 

Shader "Custom/CopyDepth" {
	Properties{
		_MainTex("", 2D) = "white" {}
	_Cutoff("", Float) = 0.5
		_Color("", Color) = (1,1,1,1)
	}

		SubShader{
		Tags{ "RenderType" = "Transparent" }
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
		struct v2f {
		float4 pos : SV_POSITION;
		float4 nz : TEXCOORD0;
	};
	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.nz.xyz = COMPUTE_VIEW_NORMAL;		// 改点的法线信息
		o.nz.w = COMPUTE_DEPTH_01;			// 改点的深度信息
		return o;
	}

	fixed4 _Color;
	fixed4 frag(v2f i) : SV_Target
	{
		//clip(_Color.a-0.01);
		return EncodeDepthNormal(i.nz.w, i.nz.xyz);
	}
		ENDCG
	}
	}
		Fallback Off
}