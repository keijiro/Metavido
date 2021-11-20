Shader "Hidden/Bibcam/Encoder"
{
    Properties
    {
        _textureY("", 2D) = "black" {}
        _textureCbCr("", 2D) = "black" {}
        _HumanStencil("", 2D) = "black" {}
        _EnvironmentDepth("", 2D) = "black" {}
    }

    CGINCLUDE

#include "UnityCG.cginc"
#include "Packages/jp.keijiro.bibcam/Common/Shaders/Common.hlsl"

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
    // Metadata
    float m = EncodeMetadata(_Metadata, UV_FullToMeta(texCoord));

    // Color
    float2 uv_c = UVFix(UV_FullToColor(texCoord));
    float y = tex2D(_textureY, uv_c).x;
    float2 cbcr = tex2D(_textureCbCr, uv_c).xy;
    float3 c = YCbCrToSRGB(y, cbcr);

    // Hue-encoded depth
    float depth = tex2D(_EnvironmentDepth, UVFix(UV_FullToDepth(texCoord))).x;
    float3 z = EncodeDepth(depth, _DepthRange, 0.025);

    // Human stencil
    float s = tex2D(_HumanStencil, UVFix(UV_FullToStencil(texCoord))).x;

    // Multiplexing
    float3 rgb = BibcamMux(texCoord, m, c, z, s);

    // Linear color support
    #ifndef UNITY_NO_LINEAR_COLORSPACE
    rgb = GammaToLinearSpace(rgb);
    #endif

    // Output
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
