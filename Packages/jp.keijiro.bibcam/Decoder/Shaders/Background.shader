Shader "Hidden/Bibcam/Background"
{
    CGINCLUDE

sampler2D _ColorTexture;
sampler2D _DepthTexture;
float4 _ProjectionVector;
float4x4 _InverseViewMatrix;
float _DepthOffset;

// Linear distance to Z depth
float DistanceToDepth(float d)
{
    return d < _ProjectionParams.y ? 0 :
      (0.5 / _ZBufferParams.z * (1 / d - _ZBufferParams.w));
}

// Inversion projection into the world space
float3 DistanceToWorldPosition(float2 uv, float d)
{
    float3 p = float3((uv - 0.5) * 2, -1);
    p.xy += _ProjectionVector.xy;
    p.xy /= _ProjectionVector.zw;
    return mul(_InverseViewMatrix, float4(p * d, 1)).xyz;
}

// 3-axis Gridline
float Gridline(float3 p)
{
    float3 b = 1 - saturate(0.5 * min(frac(1 - p), frac(p)) / fwidth(p));
    return max(max(b.x, b.y), b.z);
}

void Vertex(uint vid : SV_VertexID,
            out float4 outPosition : SV_Position,
            out float2 outTexCoord : TEXCOORD0)
{
    float u = vid & 1;
    float v = vid < 2 || vid == 5;
    outPosition = float4(u, v, 1, 1) * 2 - 1;
    outTexCoord = float2(u, v);
}

void Fragment(float4 position : SV_Position,
              float2 texCoord : TEXCOORD0,
              out float4 outColor : SV_Target,
              out float outDepth : SV_Depth)
{
    // Color/depth samples
    float4 c = tex2D(_ColorTexture, texCoord);
    float d = tex2D(_DepthTexture, texCoord).x;

    // Inverse projection
    float3 p = DistanceToWorldPosition(texCoord, d);

    // Coloring
    c.rgb = lerp(c.rgb, float3(1, 1, 1), Gridline(p * 10));
    c.rgb = lerp(c.rgb, float3(1, 0, 1), c.a * 0.5);

    // Output
    outColor = c;
    outDepth = DistanceToDepth(d) + _DepthOffset;
}

    ENDCG

    SubShader
    {
        Pass
        {
            Cull Off ZWrite On ZTest LEqual
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
