using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Decoder {

public sealed class BibcamMetadataDecoder : MonoBehaviour
{
    #region Hidden asset references

    [SerializeField, HideInInspector] ComputeShader _shader = null;

    #endregion

    #region Public members

    public Metadata Metadata { get; private set; }

    public void Decode(Texture source)
    {
        // Decoder kernel dispatching
        _shader.SetTexture(0, "Source", source);
        _shader.SetBuffer(0, "Output", _readbackBuffer);
        _shader.Dispatch(0, 1, 1, 1);

        // Synchronized readback (slow!)
        _readbackBuffer.GetData(_readbackArray);
        Metadata = new Metadata(_readbackArray[0]);
    }

    #endregion

    #region Private members

    GraphicsBuffer _readbackBuffer;
    Matrix4x4[] _readbackArray = new Matrix4x4[1];

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _readbackBuffer = GfxUtil.StructuredBuffer(16, sizeof(float));

    void OnDestroy()
      => _readbackBuffer.Dispose();

    #endregion
}

} // namespace Bibcam.Decoder
