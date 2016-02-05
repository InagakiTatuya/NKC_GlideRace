//3Dオブジェクトに擬似グロー効果を付与する。
//_GlowColorは発光色
//_Strengthは発光の強度

Shader "Custom/GlowShader" {
    Properties {
        _GlowColor ("Glow's Color", Color) = (1, 0, 0, 0)
        _Strength ("Glow Strength", Range(5.0, 1.0)) = 2.0
    }
    SubShader {
		Pass {      
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM

            #pragma vertex vert 
            #pragma fragment frag

			float4 _LightColor0; 

            float4 vert(float4 vertexPos : POSITION) : SV_POSITION {
                return mul(UNITY_MATRIX_MVP, vertexPos);
            }

            float4 frag(void) : COLOR {
                return _LightColor0; 
            }

            ENDCG 
        }

        Pass {
            Tags {
                "LightMode" = "ForwardBase"
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
            }
			
            ZWrite ON
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members normalDirection,viewDirection)
#pragma exclude_renderers d3d11 xbox360

			#pragma only_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag

            uniform float4 _GlowColor;
            uniform float  _Strength;

            struct vInput {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };

            struct v2f {
                float4 position : SV_POSITION;
                float3 normalDirection: NORMAL1;
                float3 viewDirection: NORMAL;
            };

            v2f vert(vInput i) {
                v2f o;

                float4x4 modelMatrix        = _Object2World;
                float4x4 modelMatrixInverse = _World2Object;

                float3 normalDirection = normalize(mul(i.normal, modelMatrixInverse)).xyz;
                float3 viewDirection   = normalize(_WorldSpaceCameraPos - mul(modelMatrix, i.vertex).xyz);

                float4 pos = i.vertex + (i.normal * 0.01);
				
                o.position        = mul(UNITY_MATRIX_MVP, pos);
                o.normalDirection = normalDirection;
                o.viewDirection   = viewDirection;

                return o;
            }

            float4 frag(v2f i) : COLOR {
                float3 normalDirection = normalize(i.normalDirection);
                float3 viewDirection   = normalize(i.viewDirection);
                float strength = abs(dot(viewDirection, normalDirection));
                float opacity  = pow(strength, _Strength);
                return float4(_GlowColor.xyz, pow(1.0-opacity,0.5)*0.8);
            }

            ENDCG
        }
    }
}