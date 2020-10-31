Shader "Unit/BaseLit"
{
	Properties
	{
		_Color("Color Tint", Color) = (0.5,0.5,0.5)
		_SpecularPow("SpecularPow",Range(5,50))=20
		_MainTex("MainTex",2D) = "white"{}
	}

	HLSLINCLUDE

	#include "UnityCG.cginc"
	fixed4 _Color;
	sampler2D _MainTex;
	half4 _DLightDir;
	fixed4 _DLightColor;
	half4 _CameraPos;
	float _SpecularPow;

	struct a2v
	{
		float4 position : POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 position : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 worldPos:TEXCOORD1;
		float3 normal : NORMAL;
	};

	v2f vert(a2v v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		o.uv = v.uv;
		o.position = UnityObjectToClipPos(v.position);		
		o.normal = UnityObjectToWorldNormal(v.normal);
		o.worldPos=mul(unity_ObjectToWorld,v.position).xyz;
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 fragColor = half4(_Color.rgb,1.0) * tex2D(_MainTex, i.uv);
		half3 viewDir = normalize(_CameraPos - i.worldPos);
		half3 halfDir = normalize(viewDir + _DLightDir.xyz);
		fixed specular = pow(saturate(dot(i.normal, halfDir)), _SpecularPow);
		half light = saturate(dot(i.normal, _DLightDir));
		fragColor.rgb *= (light+specular) * _DLightColor;
		return fragColor;
	}

		ENDHLSL

		SubShader
	{
		Tags{ "Queue" = "Geometry" }
			LOD 100
			Pass
		{
			Tags{ "LightMode" = "BaseLit" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
	}
}