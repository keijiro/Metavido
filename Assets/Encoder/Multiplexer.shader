Shader "Hidden/Bibcam/Multiplexer"
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

// Hue encoding
float3 Hue2RGB(float hue)
{
    float h = hue * 6 - 2;
    float r = abs(h - 1) - 1;
    float g = 2 - abs(h);
    float b = 2 - abs(h - 2);
    return saturate(float3(r, g, b));
}

// yCbCr decoding
float3 YCbCrToSRGB(float y, float2 cbcr)
{
    float b = y + cbcr.x * 1.772 - 0.886;
    float r = y + cbcr.y * 1.402 - 0.701;
    float g = y + dot(cbcr, float2(-0.3441, -0.7141)) + 0.5291;
    return float3(r, g, b);
}

// Metadata encoding
bool EncodeMetadata(float2 uv)
{
    uint2 p = (uint2)(uv * float2(4, 128));
    uint bit = p.y & 31;
    uint data = asuint(_Metadata[p.y / 32][p.x]);
    return (data >> bit) & 1;
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
    const float margin = 0.02;

    // Texture coordinate remapping
    float2 uv = (texCoord.xy - float2(margin, 0)) / float2(1 - margin, 1);
    float4 tc = frac(uv.xyxy * float4(1, 1, 2, 2));

    // Aspect ratio compensation & vertical flip
    tc.yw = (0.5 - tc.yw) * _AspectFix + 0.5;

    // Texture samples
    float y = tex2D(_textureY, tc.zy).x;
    float2 cbcr = tex2D(_textureCbCr, tc.zy).xy;
    float mask = tex2D(_HumanStencil, tc.zw).x;
    float depth = tex2D(_EnvironmentDepth, tc.zw).x;

    // Metadata
    float3 c0 = EncodeMetadata(texCoord.xy / float2(margin, 1));

    // Color plane
    float3 c1 = YCbCrToSRGB(y, cbcr);

    // Depth plane
    depth = (depth - _DepthRange.x) / (_DepthRange.y - _DepthRange.x);
    float3 c2 = Hue2RGB(clamp(depth, 0, 0.8));

    // Mask plane
    float3 c3 = mask;

    // Output
    float3 srgb = uv.x < 0 ? c0 : (tc.x < 0.5 ? c1 : (tc.y < 0.5 ? c2 : c3));
    return float4(srgb, 1);
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
