//Imageに登録して使用する
//2Dイメージに光帯を通過させるシェーダ

//光帯の位置
//float _TransTime;
//合成元テクスチャ
//sampler2D _MainTex;
//合成したいテクスチャ
//sampler2D _EmissionTex;

Shader "Custom/lightburst"
{
    Properties
    {
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
			
			float _TransTime;
			sampler2D _MainTex;
			sampler2D _EmissionTex;

            fixed4 frag(v2f v) : SV_Target
            {
				half4 c = tex2D (_MainTex,v.uv);
				if(_TransTime>0){
					half2 t = v.uv - _TransTime;
					//75度傾けた状態で計算
					t.x -= v.uv.y * 0.15 - 0.5 - 0.15;

					//テクスチャが裏回りしないように制限
					if(t.x>=0&&t.x<=1.0){
						half4 e = tex2D (_EmissionTex,t);
						c.rgb += e.rgb * e.a;
					}
				}
                return c;
            }
        ENDCG
        }
    }
	FallBack "Diffuse"
}