//汎用シェーダー
//合成したいテクスチャを指定して、アニメーション方向と速度を指定することで合成する

Shader "Custom/brendAnimation"
{
    Properties
    {
		_BrendTex ("Emission Texture", 2D) = "white" {}
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

            struct appdata_t
            {
                float4 vertex		: POSITION;
                float4 color		: COLOR;
                float2 texcoord		: TEXCOORD0;
				float2 noizeCoord	: TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex		: SV_POSITION;
                fixed4 color		: COLOR;
                half2  texcoord		: TEXCOORD0;
				half2  noizeCoord	: TEXCOORD1;
            };

			float _Alpha;
			fixed4 _TransData;
			sampler2D	_MainTex;
			sampler2D	_BrendTex;
			half4		_BrendTex_ST;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex		= mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord	= IN.texcoord;
				OUT.noizeCoord	= IN.texcoord * _BrendTex_ST.xy + _BrendTex_ST.zw;
                OUT.color		= IN.color;

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 c = tex2D (_MainTex, IN.texcoord);

			//	half2 t = IN.texcoord;
			//	t.x += _TransData.x;
			//	t.y += _TransData.y;
			//	if(_TransData.z!=0){
			//		if(t.x<0||t.x>1.0) return c;
			//		if(t.y<0||t.y>1.0) return c;
			//	}
				half4 e = tex2D (_BrendTex,IN.noizeCoord);
				c.rgb	= c.rgb * 0.8f + (e.rgb * e.a * _Alpha);
                return c;
            }
        ENDCG
        }
    }
	FallBack "Diffuse"
}