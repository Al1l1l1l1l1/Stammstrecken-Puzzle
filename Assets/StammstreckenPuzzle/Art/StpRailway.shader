Shader "Stammstrecke/Railway"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            struct Input { float4 position : POSITION; float3 normal : NORMAL; half4 color : COLOR; };
            struct Varyings { float4 position : SV_POSITION; float3 normal : TEXCOORD0; float3 world : TEXCOORD1; half4 color : COLOR; };
            Varyings Vert(Input v)
            {
                Varyings o;
                o.world = TransformObjectToWorld(v.position.xyz);
                o.position = TransformWorldToHClip(o.world);
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.color = v.color;
                return o;
            }
            half4 Frag(Varyings i) : SV_Target
            {
                Light sun = GetMainLight(TransformWorldToShadowCoord(i.world));
                half3 n = normalize(i.normal);
                half light = smoothstep(-0.25h, 0.8h, dot(n, sun.direction));
                half rim = pow(1.0h - saturate(dot(n, GetWorldSpaceNormalizeViewDir(i.world))), 3.0h);
                return half4(i.color.rgb * (0.47h + 0.6h * light * sun.shadowAttenuation * sun.color) + rim * 0.065h, 1);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}
