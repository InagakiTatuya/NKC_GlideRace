Shader "Custom/raytrace"
{
    Properties
    {
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
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
			
			float _WhiteFlag;
			float _TransTime;
			sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
				float t = _TransTime;
                half4 c = tex2D (_MainTex, IN.uv);

				if(_WhiteFlag==0){
					if(1-IN.uv.y<_TransTime) c.rgb = half3(0,1-IN.uv.y,1)*c.a;
				}else{
					c.rgb = half3(0,1-IN.uv.y,1)*c.a;
					if(1-IN.uv.y<_TransTime && 1-IN.uv.y>_TransTime-0.01f) c.rgb = 1.0f;
				}

                return c;
            }
        ENDCG
        }
    }
	FallBack "Default"
}