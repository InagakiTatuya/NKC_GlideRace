//Imageに登録して使用する
//2Dイメージに指定したテクスチャの形で特定の色で発光させるシェーダ

//発光色
//half4 _EmissionColor;
//合成元テクスチャ
//sampler2D _MainTex;
//合成したいテクスチャ
//sampler2D _EmissionTex;

Shader "Custom/bloom2D"
{
    Properties
    {
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_EmissionTex ("Emission Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos   : SV_POSITION;
                half2 uv  : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				return o;
            }
			
			half4 _EmissionColor;
			sampler2D _MainTex;
			sampler2D _EmissionTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 c = tex2D (_MainTex, IN.uv);
				half4 e = tex2D (_EmissionTex, IN.uv);
				float t = ((2 * _SinTime.w * _CosTime.w) + 1.0) * 0.5;

				e.rgb = _EmissionColor;
				e.a *= t;
                c.rgb = c.rgb * (1 - e.a) + e.rgb * e.a;

                return c;
            }
        ENDCG
        }
    }
	FallBack "Diffuse"
}