Shader "Custom/FlatRing"
{
    Properties
    {
        _Color      ("Ring Color", Color) = (1, 0.85, 0, 1)
        _Opacity    ("Opacity", Range(0, 1)) = 1
        _Radius     ("Radius", Range(0.1, 0.5)) = 0.4
        _Thickness  ("Thickness", Range(0.005, 0.15)) = 0.04
        _FadeStart  ("Fade Start Dist", Float) = 10
        _FadeEnd    ("Fade End Dist", Float) = 50
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "FlatRingForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _Opacity;
                half _Radius;
                half _Thickness;
                float _FadeStart;
                float _FadeEnd;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half fade : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 center = TransformObjectToWorld(float3(0, 0, 0));
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv - 0.5;

                float camDist = distance(center, GetCameraPositionWS());
                output.fade = 1 - saturate((camDist - _FadeStart) / (_FadeEnd - _FadeStart));

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half dist = length(input.uv);
                half ring = step(dist, _Radius + _Thickness)
                         * step(_Radius - _Thickness, dist);

                return half4(_Color.rgb, _Color.a * _Opacity * ring * input.fade);
            }
            ENDHLSL
        }
    }
}
