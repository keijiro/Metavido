using UnityEngine;

namespace Bibcam.Common {

// Commonly used shader property identifiers
public static class ShaderID
{
    public static readonly int        AspectFix = Shader.PropertyToID("_AspectFix");
    public static readonly int     ColorTexture = Shader.PropertyToID("_ColorTexture");
    public static readonly int       DepthColor = Shader.PropertyToID("_DepthColor");
    public static readonly int      DepthOffset = Shader.PropertyToID("_DepthOffset");
    public static readonly int       DepthRange = Shader.PropertyToID("_DepthRange");
    public static readonly int     DepthTexture = Shader.PropertyToID("_DepthTexture");
    public static readonly int EnvironmentDepth = Shader.PropertyToID("_EnvironmentDepth");
    public static readonly int     HumanStencil = Shader.PropertyToID("_HumanStencil");
    public static readonly int      InverseView = Shader.PropertyToID("_InverseView");
    public static readonly int           Margin = Shader.PropertyToID("_Margin");
    public static readonly int         Metadata = Shader.PropertyToID("_Metadata");
    public static readonly int        RayParams = Shader.PropertyToID("_RayParams");
    public static readonly int     StencilColor = Shader.PropertyToID("_StencilColor");
    public static readonly int      TextureCbCr = Shader.PropertyToID("_textureCbCr");
    public static readonly int         TextureY = Shader.PropertyToID("_textureY");
}

} // namespace Bibcam.Common
