// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Spine/Skeleton Light" {
	Properties{
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.1
		_Tint("Tint", Color) = (1,1,1,1)
		_Intensity("Intensity", Range(0,3)) = 1
		[HideInInspector]_Width("Width", range(0, 2)) = 0.25
		[HideInInspector]_Alpha1("Alpha1", range(0,1)) = 1
		[HideInInspector]_Alpha2("Alpha2", range(0,1)) = 1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[NoScaleOffset] _NormalMap ("Normal Map", 2D) = "" {}
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default

		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		// Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		Pass {
			blend srcalpha oneminussrcalpha
			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			// #include "AutoLight.cginc"
			#include "Lighting.cginc"
			sampler2D _MainTex;
			sampler2D _NormalMap;
			fixed _Grayscale;
			fixed4 _Tint;
			fixed _Width;
			fixed _Alpha1;
			fixed _Alpha2;
			half _Intensity;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 modelPos : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 vertexColor : COLOR;
				float3 tangentToWorld[3]:TEXCOORD3;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.modelPos = v.vertex;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexColor = v.vertexColor;
				// float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldNormal = UnityObjectToWorldNormal(float3(0,0,-1));
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				// float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				float3 worldTangent = UnityObjectToWorldDir(float3(1,0,0));
				// float3 worldBinormal = cross(worldNormal,worldTangent)*v.tangent.w;
				float3 worldBinormal = cross(worldNormal,worldTangent)* -1;
				o.tangentToWorld[0] = float3(worldTangent.x,worldBinormal.x,worldNormal.x);
				o.tangentToWorld[1] = float3(worldTangent.y,worldBinormal.y,worldNormal.y);
				o.tangentToWorld[2] = float3(worldTangent.z,worldBinormal.z,worldNormal.z);
				return o;
			}

			half3 UnpackNormal(half4 packedNormal, half scale)
			{
				half3 normal;
				normal.xy = (packedNormal.xy * 2 - 1) * scale;
				normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
				return normal;
			}

			float4 frag (VertexOutput i) : SV_Target {
				float4 texColor = tex2D(_MainTex, i.uv);
				fixed3 ambient = texColor * UNITY_LIGHTMODEL_AMBIENT.xyz;
				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif
				
				//条状显示
				// texColor.a *= _Alpha1;
				// half inputX = i.modelPos.x;
				// inputX -= step(inputX,0) * _Width / 2;	//负的部分向左偏移 _Width/2,避免x为0的地方竖条宽是2倍
				// inputX = abs(inputX);
				// texColor.a *= saturate(1 - step(fmod(inputX, _Width), _Width / 2) + _Alpha2);

				float grey = dot(texColor.rgb, float3(0.299, 0.587, 0.114));
				texColor.rgb = lerp(texColor.rgb, float3(grey, grey, grey), _Grayscale);
				texColor.rgb *= (3 - step(1, _Grayscale)) / 3 * _Intensity;
				texColor *= i.vertexColor;
				texColor.rgba *= _Tint.rgba;

				float3 tangentNormal = UnpackNormal(tex2D(_NormalMap, i.uv), 1);
				// float3 worldNormal = normalize(UnityObjectToWorldNormal(modelNormal));
				float3 N = float3(dot(i.tangentToWorld[0],tangentNormal),dot(i.tangentToWorld[1],tangentNormal),dot(i.tangentToWorld[2],tangentNormal));
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed NdotL = saturate(dot(N, worldLightDir));
				fixed3 diff = texColor.rgb * NdotL;


				return  fixed4(ambient + diff,texColor.a);
				// return _LightColor0;
			}
			ENDCG
		}

		/*Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1
			ZWrite On
			ZTest LEqual

			Fog { Mode Off }
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _Cutoff;

			struct VertexOutput {
				V2F_SHADOW_CASTER;
				float4 uvAndAlpha : TEXCOORD1;
			};

			VertexOutput vert (appdata_base v, float4 vertexColor : COLOR) {
				VertexOutput o;
				o.uvAndAlpha = v.texcoord;
				o.uvAndAlpha.a = vertexColor.a;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				fixed4 texcol = tex2D(_MainTex, i.uvAndAlpha.xy);
				clip(texcol.a * i.uvAndAlpha.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}*/
	}
	CustomEditor "SpineShaderWithOutlineGUI"
}
