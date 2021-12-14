// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Spine/Skeleton" {
	Properties{
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.1
		_Tint("Tint", Color) = (1,1,1,1)
		_Intensity("Intensity", Range(0,3)) = 1
		_Width("Width", range(0, 2)) = 0.25
		_Alpha1("Alpha1", range(0,1)) = 1
		_Alpha2("Alpha2", range(0,1)) = 1
		[NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
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
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		//被遮挡部分渲染
		//Pass
		//{
		//	Blend SrcAlpha One
		//	ZWrite Off
		//	ZTest Greater

		//	CGPROGRAM

		//	struct VertexInput {
		//		float4 vertex : POSITION;
		//		float2 uv : TEXCOORD0;
		//	};

		//	struct v2f
		//	{
		//		float4 pos : SV_POSITION;
		//		float2 uv : TEXCOORD0;
		//	};

		//	sampler2D _MainTex;


		//	v2f vert(VertexInput v)
		//	{
		//		v2f o;
		//		o.pos = UnityObjectToClipPos(v.vertex);
		//		o.uv = v.uv;
		//		return o;
		//	}

		//	fixed4 frag(v2f i) : SV_Target
		//	{
		//		float4 texColor = tex2D(_MainTex, i.uv);

		//		//fixed4 color = fixed4(texColor.r,0,0, texColor.a);

		//		return texColor;
		//	}
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	ENDCG
		//}


		Pass {
			Name "Normal"
			blend srcalpha oneminussrcalpha
			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
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
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 modelPos : TEXCOORD1;
				float4 vertexColor : COLOR;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.modelPos = v.vertex;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.vertexColor = v.vertexColor;
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				float4 texColor = tex2D(_MainTex, i.uv);

				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif
				
				//条状显示
				texColor.a *= _Alpha1;
				half inputX = i.modelPos.x;
				inputX -= step(inputX,0) * _Width / 2;	//负的部分向左偏移 _Width/2,避免x为0的地方竖条宽是2倍
				inputX = abs(inputX);
				texColor.a *= saturate(1 - step(fmod(inputX, _Width), _Width / 2) + _Alpha2);

				float grey = dot(texColor.rgb, float3(0.299, 0.587, 0.114));
				texColor.rgb = lerp(texColor.rgb, float3(grey, grey, grey), _Grayscale);
				texColor.rgb *= (3 - step(1, _Grayscale)) / 3 * _Intensity;
				texColor *= i.vertexColor;
				texColor.rgba *= _Tint.rgba;

				return texColor;
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
