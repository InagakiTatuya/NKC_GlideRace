//#Dオブジェクトに登録して使用する
//3Dオブジェクトに指定したテクスチャの形で特定の色で発光させるシェーダ

//発光色
//half4 _EmissionColor;
//合成元テクスチャ
//sampler2D _MainTex;
//合成したいテクスチャ
//sampler2D _EmissionTex;

Shader "Custom/bloom3D" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_EmissionTex ("Emission Texture", 2D) = "white" {}
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
	}
	SubShader {
        Tags
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};

		half4 _EmissionColor;
		sampler2D _MainTex;
		sampler2D _EmissionTex;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			float t = ((2 * _SinTime.w * _CosTime.w) + 1.0) * 0.5;
			float e = tex2D (_EmissionTex, IN.uv_MainTex).a * t;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = _EmissionColor * e;
		}
		ENDCG
	} 
	FallBack "Unlit/TransParent"
}
