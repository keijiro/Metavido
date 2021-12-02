using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Decoder {

static class ObjectUtil
{
    public static Material NewMaterial(Shader shader)
      => new Material(shader) { hideFlags = HideFlags.HideAndDontSave };

    public static void Destroy(Object obj)
    {
        if (obj == null) return;
#if UNITY_EDITOR
        if (Application.isPlaying && !UnityEditor.EditorApplication.isPaused)
            Object.Destroy(obj);
        else
            Object.DestroyImmediate(obj);
#else
        Object.Destroy(obj);
#endif
    }
}

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
