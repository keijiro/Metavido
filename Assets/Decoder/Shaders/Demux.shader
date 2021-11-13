Shader "Hidden/Bibcam/Demux"
{
    Properties
    {
        _MainTex("", 2D) = "black"{}
    }

    CGINCLUDE

#include "../../Common/Shaders/Common.hlsl"

sampler2D _MainTex;
float2 _DepthRange;

void Vertex(float4 position : POSITION,
            float2 texCoord : TEXCOORD,
            out float4 outPosition : SV_Position,
            out float2 outTexCoord : TEXCOORD)
{
    outPosition = float4(position.x * 2 - 1, 1 - position.y * 2, 1, 1);
    outTexCoord = texCoord;
}

float4 FragmentColor(float4 position : SV_Position,
                     float2 texCoord : TEXCOORD0) : SV_Target
{
    float3 rgb = tex2D(_MainTex, UV_ColorToFull(texCoord)).xyz;
    float a = tex2D(_MainTex, UV_StencilToFull(texCoord)).x;
    return float4(rgb, a);
}

float4 FragmentDepth(float4 position : SV_Position,
                     float2 texCoord : TEXCOORD) : SV_Target
{
    float hue = RGB2Hue(tex2D(_MainTex, UV_DepthToFull(texCoord)).xyz);
    return lerp(_DepthRange.x, _DepthRange.y, hue);
}

    ENDCG

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentColor
            ENDCG
        }
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentDepth
            ENDCG
        }
    }
}
