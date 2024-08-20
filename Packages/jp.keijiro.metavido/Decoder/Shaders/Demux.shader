Shader "Hidden/Metavido/Demux"
{
    Properties
    {
        _MainTex("", 2D) = "black"{}
    }

    CGINCLUDE

#include "UnityCG.cginc"
#include "Packages/jp.keijiro.metavido/Common/Shaders/Common.hlsl"

Texture2D _MainTex;
uint _Margin;
float2 _DepthRange;

void VertexColor(float4 position : POSITION,
                 float2 texCoord : TEXCOORD,
                 out float4 outPosition : SV_Position,
                 out float4 outTexCoord : TEXCOORD)
{
    outPosition = UnityObjectToClipPos(position);
    outTexCoord = texCoord.xyxy * MetavidoFrameSize.xyxy / float4(2, 1, 2, 2);
    outTexCoord.x += MetavidoFrameSize.x / 2;
}

float4 FragmentColor(float4 position : SV_Position,
                     float4 texCoord : TEXCOORD0) : SV_Target
{
    float3 c = _MainTex[texCoord.xy].rgb;
    float  s = _MainTex[texCoord.zw].r;
    #ifndef UNITY_NO_LINEAR_COLORSPACE
    s = LinearToGammaSpace(s);
    #endif
    s = saturate(lerp(-0.1, 1, s)); // Compression noise filter
    return float4(c, s);
}

void VertexDepth(float4 position : POSITION,
                 float2 texCoord : TEXCOORD,
                 out float4 outPosition : SV_Position,
                 out float2 outTexCoord : TEXCOORD)
{
    outPosition = float4(position.x * 2 - 1, 1 - position.y * 2, 1, 1);
    outTexCoord = texCoord * MetavidoFrameSize / 2;
    outTexCoord.y += MetavidoFrameSize.y / 2;
}

float4 FragmentDepth(float4 position : SV_Position,
                     float2 texCoord : TEXCOORD) : SV_Target
{
    uint2 tc = texCoord;
    tc.x = min(tc.x, MetavidoFrameSize.x / 2 - 1 - _Margin);
    tc.y = max(tc.y, MetavidoFrameSize.y / 2 + _Margin);
    float3 rgb = _MainTex[tc].rgb;
    #ifndef UNITY_NO_LINEAR_COLORSPACE
    rgb = LinearToGammaSpace(rgb);
    #endif
    return DecodeDepth(rgb, _DepthRange);
}

    ENDCG

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            CGPROGRAM
            #pragma vertex VertexColor
            #pragma fragment FragmentColor
            ENDCG
        }
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            CGPROGRAM
            #pragma vertex VertexDepth
            #pragma fragment FragmentDepth
            ENDCG
        }
    }
}
