// Hue encoding
float3 Hue2RGB(float hue)
{
    float h = hue * 6 - 2;
    float r = abs(h - 1) - 1;
    float g = 2 - abs(h);
    float b = 2 - abs(h - 2);
    return saturate(float3(r, g, b));
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
    return frac(lerp(r, lerp(g, b, c.g < c.b), c.r < max(c.g, c.b)) + 1);
}

// yCbCr decoding
float3 YCbCrToSRGB(float y, float2 cbcr)
{
    float b = y + cbcr.x * 1.772 - 0.886;
    float r = y + cbcr.y * 1.402 - 0.701;
    float g = y + dot(cbcr, float2(-0.3441, -0.7141)) + 0.5291;
    return float3(r, g, b);
}

// Depth hue-encoding
float3 EncodeDepth(float depth, float2 range, float margin)
{
    depth = (depth - range.x) / (range.y - range.x);
    return Hue2RGB(clamp(depth, margin, 1 - margin));
}

// Metadata encoding
bool EncodeMetadata(in float4x4 data, float2 uv)
{
    uint2 p = (uint2)(uv * float2(4, 128));
    uint bit = p.y & 31;
    uint dw = asuint(data[p.y / 32][p.x]);
    return (dw >> bit) & 1;
}

//
// UV coordinate remapping functions
//
// +---+---+-+  S: Human stencil
// | S |   | |  Z: Hue-encoded depth
// +---+ C |M|  C: Color
// | Z |   | |  M: Metadata
// +---+---+-+
//

static const float MetadataWidth = 0.02;

float2 UV_FullToMeta(float2 uv)
{
    uv.x = (uv.x - 1 + MetadataWidth) / MetadataWidth;
    return uv;
}

float2 UV_FullToCZS(float2 uv)
{
    uv.x /= 1 - MetadataWidth;
    return uv;
}

float2 UV_FullToColor(float2 uv)
{
    uv = UV_FullToCZS(uv);
    uv.x = uv.x * 2 - 1;
    return uv;
}

float2 UV_FullToDepth(float2 uv)
{
    return UV_FullToCZS(uv) * 2;
}

float2 UV_FullToStencil(float2 uv)
{
    uv = UV_FullToCZS(uv) * 2;
    uv.y -= 1;
    return uv;
}

float2 UV_MetaToFull(float2 uv)
{
    uv.x = lerp(1 - MetadataWidth, 1, uv.x);
    return uv;
}

float2 UV_CZSToFull(float2 uv)
{
    uv.x *= 1 - MetadataWidth;
    return uv;
}

float2 UV_ColorToFull(float2 uv)
{
    uv.x = lerp(0.5, 1, uv.x);
    return UV_CZSToFull(uv);
}

float2 UV_StencilToFull(float2 uv)
{
    return UV_CZSToFull(uv * 0.5 + float2(0, 0.5));
}

float2 UV_DepthToFull(float2 uv)
{
    return UV_CZSToFull(uv * 0.5);
}

// Multiplexer

float3 BibcamMux(float2 uv, float3 m, float3 c, float3 z, float3 s)
{
    return uv.x > 1 - MetadataWidth ? m :
             (uv.x > (1 - MetadataWidth) / 2 ? c : (uv.y > 0.5 ? s : z));
}
