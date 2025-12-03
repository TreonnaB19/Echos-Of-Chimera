Shader "Custom/Bulb_GlassFresnel_Emissive"
{
    Properties{
        _BaseTint      ("Glass Tint", Color) = (1,1,1,0.1)
        _GlassAlpha    ("Center Transparency", Range(0,1)) = 0.15
        _FresnelPower  ("Fresnel Power", Range(0.5,8)) = 3

        _EmissionColor ("Emission Color", Color) = (1,0.85,0.6,1)
        _EmissionStrength ("Emission Strength", Range(0,10)) = 2

        _FlickerSpeed  ("Flicker Speed", Range(0,20)) = 0
        _FlickerAmp    ("Flicker Amplitude", Range(0,1)) = 0
        _FlickerNoise  ("Flicker Noise Scale", Range(0,10)) = 3
    }
    SubShader{
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 300

        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade keepalpha
        #pragma target 3.0
        #include "UnityCG.cginc"

        fixed4 _BaseTint;
        half   _GlassAlpha, _FresnelPower;
        fixed4 _EmissionColor;
        half   _EmissionStrength;
        half   _FlickerSpeed, _FlickerAmp, _FlickerNoise;

        struct Input{
            float3 worldPos;
            float3 viewDir;
        };

        // tiny hash for per-bulb phase variance
        float hash21(float2 p){
            p = frac(p*float2(123.34, 345.45));
            p += dot(p, p+34.345);
            return frac(p.x*p.y);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Glass: more opaque at edges via Fresnel
            float3 N = normalize(o.Normal); // surface normal from mesh
            float  fres = pow(1.0 - saturate(dot(normalize(IN.viewDir), N)), _FresnelPower);
            float  edgeAlpha = saturate(lerp(_GlassAlpha, 1.0, fres));

            // Base tint contributes a subtle color to the glass
            o.Albedo     = _BaseTint.rgb;
            o.Metallic   = 0.0;
            o.Smoothness = 0.9;
            o.Alpha      = edgeAlpha;  // transparent center, stronger edges

           
            float phase = hash21(IN.worldPos.xy * _FlickerNoise) * 6.2831;
            float flick = (sin(_Time.y * _FlickerSpeed + phase) * 0.5 + 0.5) * _FlickerAmp;

            // Emission drives the glow/bloom
            o.Emission = _EmissionColor.rgb * _EmissionStrength * (1.0 + flick);
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
