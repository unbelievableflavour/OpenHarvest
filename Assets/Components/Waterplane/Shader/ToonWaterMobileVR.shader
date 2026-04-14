Shader "Custom/ToonWaterMobileVR"
{
    Properties
    {
        _WaterColor ("Water Color", Color) = (0.2, 0.6, 1.0, 0.8)
        _FoamColor ("Foam Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _WaveSpeed ("Wave Speed", Range(0, 2)) = 0.5
        _WaveHeight ("Wave Height", Range(0, 0.5)) = 0.1
        _WaveFrequency ("Wave Frequency", Range(0.1, 5)) = 1.0
        _FoamWidth ("Foam Width", Range(0, 1)) = 0.1
        _Transparency ("Transparency", Range(0, 1)) = 0.8
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Back
        
        Pass
        {
            Name "ToonWaterPass"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _WaterColor;
                float4 _FoamColor;
                float _WaveSpeed;
                float _WaveHeight;
                float _WaveFrequency;
                float _FoamWidth;
                float _Transparency;
            CBUFFER_END
            
            // Simple noise function for waves
            float SimpleNoise(float2 uv)
            {
                return sin(uv.x * 6.28318) * sin(uv.y * 6.28318);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                
                // Simple wave animation - optimized for mobile VR
                float time = _Time.y * _WaveSpeed;
                float wave1 = sin(positionWS.x * _WaveFrequency + time) * _WaveHeight;
                float wave2 = cos(positionWS.z * _WaveFrequency * 0.8 + time * 1.2) * _WaveHeight * 0.5;
                positionWS.y += wave1 + wave2;
                
                output.positionHCS = TransformWorldToHClip(positionWS);
                output.positionWS = positionWS;
                output.uv = input.uv;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.screenPos = ComputeScreenPos(output.positionHCS);
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Simple toon-style water coloring
                float3 worldPos = input.positionWS;
                float time = _Time.y * _WaveSpeed;
                
                // Animated UV for surface movement
                float2 animatedUV = input.uv + float2(time * 0.1, time * 0.05);
                
                // Proper foam effect at UV edges
                float2 uvEdges = abs(input.uv - 0.5) * 2.0; // Convert to 0-1 from edges
                float edgeDistance = 1.0 - max(uvEdges.x, uvEdges.y); // Distance from nearest edge
                float foam = smoothstep(0.0, _FoamWidth, edgeDistance);
                
                // Add animated foam variation for more dynamic look
                float foamNoise = sin(time * 2.0 + input.positionWS.x * 0.5 + input.positionWS.z * 0.3) * 0.5 + 0.5;
                foam = saturate(foam + foamNoise * foam * 0.3); // Enhance foam with noise
                
                // Simple lighting calculation for toon style
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float3 normal = normalize(input.normalWS);
                float NdotL = saturate(dot(normal, lightDir));
                
                // Toon-style stepped lighting (3 levels for better mobile performance)
                float toonLight = floor(NdotL * 2.0) / 2.0 + 0.3; // 2 steps + ambient
                
                // Combine colors with better contrast
                float3 waterColor = _WaterColor.rgb * toonLight;
                float3 finalColor = lerp(waterColor, _FoamColor.rgb, foam);
                
                // Use transparency setting
                float alpha = lerp(_Transparency, 1.0, foam * 0.5); // Foam is more opaque
                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Lit"
}
