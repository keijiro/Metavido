using UnityEngine;

namespace Bibcam.Decoder {

sealed class MetadataDecoder : MonoBehaviour
{
    #region Hidden external asset references

    [SerializeField, HideInInspector] ComputeShader _shader = null;

    #endregion

    #region Public members

    public ref readonly Metadata Metadata => ref _metadata;

    public void Decode(Texture source)
    {
        _shader.SetTexture(0, "Source", source);
        _shader.SetBuffer(0, "Output", _readbackBuffer);
        _shader.Dispatch(0, 1, 1, 1);

        _readbackBuffer.GetData(_readbackArray);
        _metadata = new Metadata(_readbackArray[0]);
    }

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

    #endregion
}

} // namespace Bibcam.Decoder
