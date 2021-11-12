using UnityEngine;
using UnityEngine.Video;

namespace Bibcam.Decoder {

sealed class MetadataDecoder : MonoBehaviour
{
    #region Hidden external asset references

    [SerializeField, HideInInspector] Shader _demuxShader = null;
    [SerializeField, HideInInspector] ComputeShader _decodeShader = null;

    #endregion

    #region Public accessor properties

    public bool IsReady => _texture.buffer != null;

    public RenderTexture ColorTexture => _texture.color;
    public RenderTexture DepthTexture => _texture.depth;

    public Vector3 CameraPosition
      => IsReady ? _metadata.CameraPosition : Vector3.zero;

    public Quaternion CameraRotation
      => IsReady ? _metadata.CameraRotation : Quaternion.identity;

    public Matrix4x4 ProjectionMatrix
      => IsReady ? _metadata.ProjectionMatrix : Matrix4x4.identity;

    public Matrix4x4 CameraToWorldMatrix
      => Matrix4x4.TRS(CameraPosition, CameraRotation, new Vector3(1, 1, -1));

    #endregion

    #region Private members

    (Texture2D buffer, RenderTexture color, RenderTexture depth) _texture;
    (GraphicsBuffer buffer, Matrix4x4[] array) _readback;
    Material _demuxMaterial;
    Metadata _metadata;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _readback.buffer = GfxUtil.StructuredBuffer(16, sizeof(float));
        _readback.array = new Matrix4x4[1];
        _demuxMaterial = new Material(_demuxShader);
    }

    void OnDestroy()
    {
        Destroy(_texture.buffer);
        Destroy(_texture.color);
        Destroy(_texture.depth);
        _readback.buffer.Dispose();
        Destroy(_demuxMaterial);
    }

    void Update()
    {
        var video = GetComponent<VideoPlayer>();
        if (video.texture == null) return;
        PrepareTexture(video.texture);
        RunDecoder();
        RunDemuxer();
    }

    #endregion

    #region Private methods

    void PrepareTexture(Texture source)
    {
        if (_texture.buffer == null)
        {
            var (w, h) = (source.width, source.height);
            _texture.buffer = GfxUtil.RGBATexture(w, h);
            _texture.color = GfxUtil.RGBARenderTexture(w / 2, h);
            _texture.depth = GfxUtil.RHalfRenderTexture(w / 2, h / 2);
        }
        Graphics.CopyTexture(source, _texture.buffer);
    }

    void RunDecoder()
    {
        _decodeShader.SetTexture(0, "Source", _texture.buffer);
        _decodeShader.SetBuffer(0, "Output", _readback.buffer);
        _decodeShader.Dispatch(0, 1, 1, 1);
        _readback.buffer.GetData(_readback.array);
        _metadata = new Metadata(_readback.array[0]);
    }

    void RunDemuxer()
    {
        _demuxMaterial.SetVector(ShaderID.DepthRange, _metadata.DepthRange);
        Graphics.Blit(_texture.buffer, _texture.color, _demuxMaterial, 0);
        Graphics.Blit(_texture.buffer, _texture.depth, _demuxMaterial, 1);
    }

    #endregion
}

} // namespace Bibcam.Decoder
