using UnityEngine;
using UnityEngine.Video;

namespace Bibcam.Decoder {

sealed class TextureDemuxer : MonoBehaviour
{
    #region Hidden external asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Public accessor properties

    public RenderTexture ColorTexture => _color;
    public RenderTexture DepthTexture => _depth;

    #endregion

    #region Private members

    Material _material;
    RenderTexture _color;
    RenderTexture _depth;

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _material = new Material(_shader);

    void OnDestroy()
    {
        Destroy(_material);
        Destroy(_color);
        Destroy(_depth);
    }

    void Update()
    {
        var video = GetComponent<VideoPlayer>();
        if (video.texture == null) return;

        var (w, h) = (video.texture.width, video.texture.height);

        if (_color == null) _color = GfxUtil.RGBARenderTexture(w / 2, h);
        if (_depth == null) _depth = GfxUtil.RHalfRenderTexture(w / 2, h / 2);

        var meta = GetComponent<MetadataDecoder>().Metadata;
        _material.SetVector(ShaderID.DepthRange, meta.DepthRange);

        Graphics.Blit(video.texture, _color, _material, 0);
        Graphics.Blit(video.texture, _depth, _material, 1);
    }

    #endregion
}

} // namespace Bibcam.Decoder
