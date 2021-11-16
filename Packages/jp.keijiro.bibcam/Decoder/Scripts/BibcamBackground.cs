using UnityEngine;

namespace Bibcam.Decoder {

[RequireComponent(typeof(Camera))]
public sealed class BibcamBackground : MonoBehaviour
{
    #region Scene object references

    [SerializeField] BibcamTextureDemuxer _demux = null;

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
        var camera = GetComponent<Camera>();
        if (camera != Camera.current) return;

        // Run it only when the textures are ready.
        if (_demux.ColorTexture == null) return;

        // Camera parameters
        var ray = BibcamRenderUtils.RayParams(camera);
        var iview = BibcamRenderUtils.InverseView(camera);

        // Material property update
        _material.SetVector(ShaderID.RayParams, ray);
        _material.SetMatrix(ShaderID.InverseView, iview);
        _material.SetTexture(ShaderID.ColorTexture, _demux.ColorTexture);
        _material.SetTexture(ShaderID.DepthTexture, _demux.DepthTexture);

        // Fullscreen quad drawcall
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, 1);
    }

    #endregion
}

} // namespace Bibcam.Decoder
