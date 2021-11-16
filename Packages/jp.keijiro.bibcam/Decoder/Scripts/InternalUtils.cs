using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Decoder {

static class ShaderID
{
    public static readonly int ColorTexture = Shader.PropertyToID("_ColorTexture");
    public static readonly int DepthRange = Shader.PropertyToID("_DepthRange");
    public static readonly int DepthTexture = Shader.PropertyToID("_DepthTexture");
    public static readonly int InverseView = Shader.PropertyToID("_InverseView");
    public static readonly int RayParams = Shader.PropertyToID("_RayParams");
}

static class GfxUtil
{
    public static RenderTexture RGBARenderTexture(int width, int height)
      => new RenderTexture(width, height, 0);

    public static RenderTexture RHalfRenderTexture(int width, int height)
      => new RenderTexture(width, height, 0, RenderTextureFormat.RHalf);

    public static GraphicsBuffer StructuredBuffer(int count, int stride)
      => new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, stride);
}

} // namespace Bibcam.Decoder
