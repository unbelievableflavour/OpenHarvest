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
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            fixed4 _Color;
            half   _Opacity;
            half   _Radius;
            half   _Thickness;
            float  _FadeStart;
            float  _FadeEnd;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                half fade  : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv - 0.5;

                float3 center = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                half camDist = distance(center, _WorldSpaceCameraPos);
                o.fade = 1 - saturate((camDist - _FadeStart) / (_FadeEnd - _FadeStart));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half dist = length(i.uv);
                half ring = step(dist, _Radius + _Thickness)
                         * step(_Radius - _Thickness, dist);

                return fixed4(_Color.rgb, _Color.a * _Opacity * ring * i.fade);
            }
            ENDCG
        }
    }
}
