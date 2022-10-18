Shader "Custom/PlayerDiffuse" {
    Properties {
        _NotVisibleColor ("NotVisibleColor (RGB)", Color) = (0.3,0.3,0.3,1)
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass {
            ZTest Greater
            Lighting Off
            ZWrite Off
            Color [_NotVisibleColor]
        }

CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
ENDCG
    } 
    Fallback "Legacy Shaders/VertexLit"
}
