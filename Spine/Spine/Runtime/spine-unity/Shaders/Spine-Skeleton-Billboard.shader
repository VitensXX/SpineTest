
Shader "Spine/Skeleton Billboard" {
	Properties {
		_Tint ("Tint", COLOR) = (1,1,1,1)
		_Intensity("Intensity", Range(0,3)) = 1
		//_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		[HideInInspector] _Alpha("Alpha", Range(0,1)) = 1
		//[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			//Comp GEqual
			//Comp Equal
			Pass keep
		}

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			//#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma multi_compile __ _TURN_TO_RIGHT
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed4 _Tint;
			fixed _Alpha;
			half _Intensity;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o;

				//Billboard
				float3 center = float3(0, 0, 0);
				float3 viewer = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
#if _TURN_TO_RIGHT
				float3 normalDir = viewer;
#else
				float3 normalDir = -viewer;
#endif
				normalDir.x = 0;
				normalDir = normalize(normalDir);
				//float3 upDir = abs(normalDir.y) > 0.99 ? float3(0, 0, 1) : float3(0, 1, 0);
				float3 upDir =float3(0, 1, 0);
				float3 rightDir = normalize(cross(upDir, normalDir));
				upDir = normalize(cross(normalDir, rightDir));
				float3 centerOffs = v.vertex.xyz - center;
				float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y;
				o.pos = UnityObjectToClipPos(float4(localPos, 1));

				o.uv = v.uv;
				o.vertexColor = v.vertexColor;
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv) * i.vertexColor * _Alpha;

				/*#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif*/

				//fixed4 col = texColor ;

				//Gray
				if (_Tint.r < 0.01 && _Tint.g < 0.01 && _Tint.b < 0.01)
				{
					float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
					col.rgb = float3(grey, grey, grey);
				}
				else 
				{
					col *= _Tint;
				}

				col.rgb *= _Intensity;

				return col;
			}
			ENDCG
		}
	}
	//CustomEditor "SpineShaderWithOutlineGUI"
}
