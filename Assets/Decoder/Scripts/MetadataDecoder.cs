using UnityEngine;
using UnityEngine.Video;

namespace Bibcam.Decoder {

sealed class MetadataDecoder : MonoBehaviour
{
    #region Hidden external asset references

    [SerializeField, HideInInspector] ComputeShader _shader = null;

    #endregion

    #region Public accessor properties

    public ref readonly Metadata Metadata => ref _metadata;

    #endregion

    #region Private members

    GraphicsBuffer _readbackBuffer;
    Matrix4x4[] _readbackArray;
    Metadata _metadata;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _readbackBuffer = GfxUtil.StructuredBuffer(16, sizeof(float));
        _readbackArray = new Matrix4x4[1];
    }

    void OnDestroy()
      => _readbackBuffer.Dispose();

    void Update()
    {
        var video = GetComponent<VideoPlayer>();
        if (video.texture == null) return;

        _shader.SetTexture(0, "Source", video.texture);
        _shader.SetBuffer(0, "Output", _readbackBuffer);
        _shader.Dispatch(0, 1, 1, 1);

        _readbackBuffer.GetData(_readbackArray);
        _metadata = new Metadata(_readbackArray[0]);
    }

    #endregion
}

} // namespace Bibcam.Decoder
