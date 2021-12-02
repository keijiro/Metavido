using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Decoder {

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public sealed class BibcamBackground : MonoBehaviour
{
    #region Scene object references

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demux = null;

    #endregion

    #region Editable attributes

    [SerializeField] float _depthOffset = 0;
    [SerializeField] Color _depthColor = Color.white;
    [SerializeField] Color _stencilColor = Color.red;

    #endregion

    #region Hidden asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private objects

    Material _material;

    #endregion

    #region MonoBehaviour implementation

    void OnDestroy()
      => ObjectUtil.Destroy(_material);

    void LateUpdate()
    {
        // Run it only when the textures are ready.
        if (_demux.ColorTexture == null) return;

        // Camera parameters
        var meta = _decoder.Metadata;
        var ray = BibcamRenderUtils.RayParams(meta);
        var iview = BibcamRenderUtils.InverseView(meta);

        // Lazy initialization for the background shader
        if (_material == null) _material = ObjectUtil.NewMaterial(_shader);

        // Material property update
        _material.SetVector(ShaderID.RayParams, ray);
        _material.SetMatrix(ShaderID.InverseView, iview);
        _material.SetVector(ShaderID.DepthRange, meta.DepthRange);
        _material.SetFloat(ShaderID.DepthOffset, _depthOffset);
        _material.SetColor(ShaderID.DepthColor, _depthColor);
        _material.SetColor(ShaderID.StencilColor, _stencilColor);
        _material.SetTexture(ShaderID.ColorTexture, _demux.ColorTexture);
        _material.SetTexture(ShaderID.DepthTexture, _demux.DepthTexture);
    }

    #endregion

    #region Draw methods

    // Public draw method for SRPs
    public void PushDrawCommand(UnityEngine.Rendering.CommandBuffer cmd)
      => cmd.DrawProcedural
           (Matrix4x4.identity, _material, 0, MeshTopology.Triangles, 6);

    // OnRenderObject implementation for the built-in render pipeline
    void OnRenderObject()
    {
        // Test if it's the target camera. This always fails on SRPs.
        if (GetComponent<Camera>() != Camera.current) return;

        // Run it only when the textures are ready.
        if (_demux.ColorTexture == null) return;

        // Fullscreen quad drawcall
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, 1);
    }

    #endregion
}

} // namespace Bibcam.Decoder
