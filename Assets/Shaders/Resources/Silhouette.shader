Shader "Unlit/Silhouette"
{
    Properties
    {
        [MainColor] _BaseColour("Base Colour", Color) = (1, 1, 1, 1) 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColour;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return _BaseColour;
            }
            ENDHLSL
        }
    }
}
