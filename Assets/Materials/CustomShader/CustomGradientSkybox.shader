Shader "Custom/GradientSkybox" {
    Properties {
        _TopColor ("Top Color", Color) = (0,0.5,1,1)
        _BottomColor ("Bottom Color", Color) = (1,1,1,1)
        _BottomLevel ("Bottom Level", Range(0,1)) = 0
        _TopLevel ("Top Level", Range(0,1)) = 1
        _BlendPower ("Blend Power", Range(0.1,5)) = 1      
    }
    SubShader {
        Tags { "Queue"="Background" "RenderType"="Background" }
        Cull Off ZWrite Off Lighting Off Fog { Mode Off }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            fixed4 _TopColor;
            fixed4 _BottomColor;
            float _BottomLevel;
            float _TopLevel;
            float _BlendPower;
            
            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 ndc = o.vertex.xy / o.vertex.w;
                o.uv = ndc * 0.5 + 0.5;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                float t = saturate((i.uv.y - _BottomLevel) / (_TopLevel - _BottomLevel));
                t = pow(t, _BlendPower);
                return lerp(_BottomColor, _TopColor, t);
            }
            ENDCG
        }
    }
    FallBack "RenderFX/Skybox"
}