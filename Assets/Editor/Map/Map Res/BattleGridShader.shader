Shader "Aoi/BattleGridShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_BoundColor("BoundColor", Color) = (1,1,1,1)
		_BoundSize("BoundSize", Vector) = (1,1,1,1)
		_Bound("_Bound", Vector) = (0,0,1,1)
		_Alpha("_Alpha", Float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "LightMode"="ForwardBase" "Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			stencil{
				Ref 0
				Comp Equal
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _BoundColor;
			float4 _Bound;
			float4 _BoundSize;
			float _Alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				v.uv.y = 1.0 - v.uv.y;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				//_Bound.x = _BoundSize.x - _Bound.x;
				_Bound.y = _BoundSize.y - _Bound.y;
				//_Bound.z = _BoundSize.x - _Bound.z;
				//_Bound.w = _BoundSize.y - _Bound.w;
				
				if ((i.uv.x > _Bound.x && i.uv.x < _Bound.z) && (i.uv.y > _Bound.y && i.uv.y < _Bound.w)) {
					col *= _Color;
				}
				else {
					col *= _BoundColor;
				}
				
				//col.a = _Alpha;
				return col;
			}
			ENDCG
		}
	}
}
