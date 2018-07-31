// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Depth"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			//提前定义
			uniform sampler2D_float _CameraDepthTexture;

			struct VertexInput
			{
				float4 pos : POSITION;

				float2 uv : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;

				float2 uv : TEXCOORD0;
			};

			VertexOutput vert(VertexInput i)
			{
				VertexOutput o;
				o.pos = UnityObjectToClipPos(i.pos);
				o.uv = i.uv;
				return o;
			}

			fixed4 frag(VertexOutput o) : COLOR
			{
				//兼容问题
				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, o.uv));
				depth = Linear01Depth(depth) * 10.0f;
				return fixed4(depth,depth,depth,1);
			}
			ENDCG
		}
	}
}
