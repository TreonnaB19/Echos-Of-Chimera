Shader "Custom/Hologram_Scanline"
{
    Properties {
        _BaseColor ("Base Color", Color) = (0, 1, 1, 1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _ScanTex ("Scanline Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Range(-5,5)) = 1
        _Dissolve ("Dissolve Amount", Range(0,1)) = 0.5
        _EdgeColor ("Edge Glow", Color) = (0,1,1,1)
        _EdgeWidth ("Edge Width", Range(0.001,0.1)) = 0.02
        _FresnelPower ("Fresnel Power", Range(0.5,8)) = 3
        _Emission ("Emission Strength", Range(0,5)) = 1
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Back

        CGPROGRAM
        #pragma surface surf Standard alpha:fade keepalpha
        #pragma target 3.0

        sampler2D _NoiseTex, _ScanTex;
        fixed4 _BaseColor, _EdgeColor;
        half _ScrollSpeed, _Dissolve, _EdgeWidth, _FresnelPower, _Emission;

        struct Input {
            float2 uv_NoiseTex;
            float3 viewDir;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Scanlines
            float2 uv = IN.uv_NoiseTex;
            uv.y += _Time.y * _ScrollSpeed;
            float scan = tex2D(_ScanTex, uv * 5).r;

            // Dissolve
            float noise = tex2D(_NoiseTex, IN.uv_NoiseTex * 3).r;
            float edge = smoothstep(_Dissolve - _EdgeWidth, _Dissolve + _EdgeWidth, noise);
            clip(edge - 0.01);

            // Fresnel edge
            float fres = pow(1 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);

            o.Albedo = _BaseColor.rgb;
            o.Emission = (_EdgeColor.rgb * fres + _BaseColor.rgb * scan) * _Emission;
            o.Alpha = edge;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
