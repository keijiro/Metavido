using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Decoder {

[RequireComponent(typeof(Camera))]
public sealed class BibcamBackground : MonoBehaviour
{
    #region Scene object references

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demux = null;

    #endregion

    #region Editable attributes

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

    void Start()
      => _material = new Material(_shader);

    void OnDestroy()
      => Destroy(_material);

    void OnRenderObject()
    {
        // Run it only on the target camera.
        if (GetComponent<Camera>() != Camera.current) return;

        // Run it only when the textures are ready.
        if (_demux.ColorTexture == null) return;

        // Camera parameters
        var meta = _decoder.Metadata;
        var ray = BibcamRenderUtils.RayParams(meta);
        var iview = BibcamRenderUtils.InverseView(meta);

        // Material property update
        _material.SetVector(ShaderID.RayParams, ray);
        _material.SetMatrix(ShaderID.InverseView, iview);
        _material.SetVector(ShaderID.DepthRange, meta.DepthRange);
        _material.SetTexture(ShaderID.ColorTexture, _demux.ColorTexture);
        _material.SetTexture(ShaderID.DepthTexture, _demux.DepthTexture);
        _material.SetColor(ShaderID.DepthColor, _depthColor);
        _material.SetColor(ShaderID.StencilColor, _stencilColor);

        // Fullscreen quad drawcall
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, 1);
    }

    #endregion
}

} // namespace Bibcam.Decoder
