#ifndef __INCLUDE_BIBCAM_DECODER_UTILS_HLSL__
#define __INCLUDE_BIBCAM_DECODER_UTILS_HLSL__

// Linear distance to Z depth
float DistanceToDepth(float d)
{
    return (1 / d - _ZBufferParams.w) / _ZBufferParams.z;
}

// Inversion projection into the world space
float3 DistanceToWorldPosition
  (float2 uv, float d, in float4 rayParams, in float4x4 inverseView)
{
    float3 ray = float3((uv - 0.5) * 2, 1);
    ray.xy = (ray.xy + rayParams.xy) * rayParams.zw;
    return mul(inverseView, float4(ray * d, 1)).xyz;
}

#endif
