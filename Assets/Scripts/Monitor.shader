Shader "Hidden/Bibcam/Monitor"
{
    Properties
    {
        _MainTex("", 2D) = "black" {}
    }

    CGINCLUDE

#include "UnityCG.cginc"
#include "Packages/jp.keijiro.bibcam/Common/Shaders/Common.hlsl"

sampler2D _MainTex;

// Vertex shader
void Vertex(float4 position : POSITION,
            float2 texCoord : TEXCOORD,
            out float4 outPosition : SV_Position,
            out float2 outTexCoord : TEXCOORD)
{
    outPosition = UnityObjectToClipPos(position);
    outTexCoord = texCoord;
}

// Fragment shader
float4 Fragment(float4 position : SV_Position,
                float2 texCoord : TEXCOORD) : SV_Target
{
    float3 c = tex2D(_MainTex, UV_ColorToFull(texCoord)).xyz;
    float  s = tex2D(_MainTex, UV_StencilToFull(texCoord)).x;
    float3 z = tex2D(_MainTex, UV_DepthToFull(texCoord)).xyz;

    float d = RGB2Hue(z);

    return float4(lerp(c * (1 - d), float3(0, 1, 1), s * 0.5), 1);
}

    ENDCG

    SubShader
    {
        Pass
        {
            Cull Off ZTest Always ZWrite Off
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
