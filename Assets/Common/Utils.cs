using UnityEngine;

namespace Bibcam {

static class ShaderID
{
    public static readonly int ColorTexture = Shader.PropertyToID("_ColorTexture");
    public static readonly int DepthTexture = Shader.PropertyToID("_DepthTexture");
    public static readonly int TextureY = Shader.PropertyToID("_textureY");
    public static readonly int TextureCbCr = Shader.PropertyToID("_textureCbCr");
    public static readonly int HumanStencil = Shader.PropertyToID("_HumanStencil");
    public static readonly int EnvironmentDepth = Shader.PropertyToID("_EnvironmentDepth");
    public static readonly int DepthRange = Shader.PropertyToID("_DepthRange");
    public static readonly int AspectFix = Shader.PropertyToID("_AspectFix");
    public static readonly int Metadata = Shader.PropertyToID("_Metadata");
    public static readonly int InverseViewMatrix = Shader.PropertyToID("_InverseViewMatrix");
    public static readonly int ProjectionVector = Shader.PropertyToID("_ProjectionVector");
}

static class GfxUtil
{
    public static Texture2D RGBATexture(int width, int height)
      => new Texture2D(width, height, TextureFormat.RGBA32, false);

    public static RenderTexture RGBARenderTexture(int width, int height)
      => new RenderTexture(width, height, 0);

    public static RenderTexture RHalfRenderTexture(int width, int height)
      => new RenderTexture(width, height, 0, RenderTextureFormat.RHalf);

    public static GraphicsBuffer StructuredBuffer(int count, int stride)
      => new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, stride);
}

} // namespace Bibcam {
