using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Encoder {

static class GfxUtil
{
    public static RenderTexture RGBARenderTexture(int width, int height)
      => new RenderTexture(width, height, 0)
           { wrapMode = TextureWrapMode.Clamp };

    public static RenderTexture RHalfRenderTexture(int width, int height)
      => new RenderTexture(width, height, 0, RenderTextureFormat.RHalf)
           { wrapMode = TextureWrapMode.Clamp };

    public static GraphicsBuffer StructuredBuffer(int count, int stride)
      => new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, stride);
}

} // namespace Bibcam.Decoder
