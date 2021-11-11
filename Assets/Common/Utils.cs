using UnityEngine;

namespace Bibcam {

static class ShaderID
{
    public static readonly int TextureY = Shader.PropertyToID("_textureY");
    public static readonly int TextureCbCr = Shader.PropertyToID("_textureCbCr");
    public static readonly int HumanStencil = Shader.PropertyToID("_HumanStencil");
    public static readonly int EnvironmentDepth = Shader.PropertyToID("_EnvironmentDepth");
    public static readonly int DepthRange = Shader.PropertyToID("_DepthRange");
    public static readonly int AspectFix = Shader.PropertyToID("_AspectFix");
    public static readonly int Metadata = Shader.PropertyToID("_Metadata");
}

} // namespace Bibcam {
