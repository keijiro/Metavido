Shader "Hidden/Bibcam/Demux"
{
    Properties
    {
        _MainTex("", 2D) = "black"{}
    }

    CGINCLUDE

sampler2D _MainTex;
float2 _DepthRange;

float2 UVMargin(float2 uv)
{
    const float margin = 0.02;
    uv.x = lerp(margin, 1, uv.x);
    return uv;
}

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
    float2 uv1 = UVMargin(texCoord * float2(0.5, 1.0));
    float2 uv2 = UVMargin(texCoord * float2(0.5, 0.5) + float2(0.5, 0));
    float3 rgb = tex2D(_MainTex, uv1).xyz;
    float mask = tex2D(_MainTex, uv2).x;
    return float4(rgb, mask);
}

// Hue value calculation
float RGB2Hue(float3 c)
{
    float minc = min(min(c.r, c.g), c.b);
    float maxc = max(max(c.r, c.g), c.b);
    float div = 1 / (6 * max(maxc - minc, 1e-5));
    float r = (c.g - c.b) * div;
    float g = 1.0 / 3 + (c.b - c.r) * div;
    float b = 2.0 / 3 + (c.r - c.g) * div;
    return lerp(r, lerp(g, b, c.g < c.b), c.r < max(c.g, c.b));
}

// Depth calculation
float RGB2Depth(float3 rgb)
{
    return lerp(_DepthRange.x, _DepthRange.y, RGB2Hue(rgb));
}

float4 FragmentDepth(float4 position : SV_Position,
                     float2 texCoord : TEXCOORD) : SV_Target
{
    float2 uv = UVMargin(texCoord * float2(0.5, 0.5) + float2(0.5, 0.5));
    return RGB2Depth(tex2D(_MainTex, uv).xyz);
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
