Shader "Custom/Lit_Standard_Emissive"
{
    Properties{
        _Color ("Tint", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _EmissionColor ("Emission Color", Color) = (0,0,0)
        _EmissionMap ("Emission (RGB)", 2D) = "black" {}
        _EmissionStrength ("Emission Strength", Range(0,5)) = 0
    }
    SubShader{
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow
        #pragma target 3.0

        sampler2D _MainTex, _EmissionMap;
        fixed4 _Color;
        half _Metallic, _Smoothness;
        fixed4 _EmissionColor;
        half _EmissionStrength;

        struct Input{ float2 uv_MainTex; float2 uv_EmissionMap; };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = c.a;

            // Emission keeps materials readable in very dark lighting
            fixed3 e = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor.rgb * _EmissionStrength;
            o.Emission = e;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
