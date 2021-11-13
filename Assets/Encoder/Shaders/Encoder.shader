Shader "Hidden/Bibcam/Encoder"
{
    Properties
    {
        _MainTex("", 2D) = "black" {}
        _textureY("", 2D) = "black" {}
        _textureCbCr("", 2D) = "black" {}
        _HumanStencil("", 2D) = "black" {}
        _EnvironmentDepth("", 2D) = "black" {}
    }

    CGINCLUDE

#include "../../Common/Shaders/Common.hlsl"

// Uniforms from AR Foundation
sampler2D _textureY;
sampler2D _textureCbCr;
sampler2D _HumanStencil;
sampler2D _EnvironmentDepth;

// Additional camera parameters
float2 _DepthRange;
float _AspectFix;

// Metadata as matrix
float4x4 _Metadata;

// Aspect ratio compensation & vertical flip
float2 UVFix(float2 uv)
{
    uv.y = (0.5 - uv.y) * _AspectFix + 0.5;
    return uv;
}

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
    // Texture coordinate remapping
    float2 uv_m = UV_FullToMeta(texCoord);
    float2 uv_c = UVFix(UV_FullToColor(texCoord));
    float2 uv_z = UVFix(UV_FullToDepth(texCoord));
    float2 uv_s = UVFix(UV_FullToStencil(texCoord));

    // Metadata
    float m = EncodeMetadata(_Metadata, uv_m);

    // Color
    float y = tex2D(_textureY, uv_c).x;
    float2 cbcr = tex2D(_textureCbCr, uv_c).xy;
    float3 c = YCbCrToSRGB(y, cbcr);

    // Hue-encoded depth
    float depth = tex2D(_EnvironmentDepth, uv_z).x;
    depth = (depth - _DepthRange.x) / (_DepthRange.y - _DepthRange.x);
    float3 z = Hue2RGB(clamp(depth, 0, 0.8));

    // Human stencil
    float s = tex2D(_HumanStencil, uv_s).x;

    // Output
    float3 rgb = uv_m.x < 1 ? m : (uv_c.x < 1 ? c : (texCoord.y < 0.5 ? z : s));
    return float4(rgb, 1);
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
