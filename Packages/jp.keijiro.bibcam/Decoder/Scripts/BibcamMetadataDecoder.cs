using UnityEngine;
using UnityEngine.Rendering;
using Bibcam.Common;

namespace Bibcam.Decoder {

[ExecuteInEditMode]
public sealed class BibcamMetadataDecoder : MonoBehaviour
{
    #region Hidden asset references

    [SerializeField, HideInInspector] ComputeShader _shader = null;

    #endregion

    #region Public members

    public Metadata Metadata { get; private set; }

    public int DecodeCount { get; private set; }

    public void DecodeSync(Texture source)
    {
        DispatchDecoder(source);

        // Synchronized readback (slow!)
        DecodeBuffer.GetData(_readbackArray);
        Metadata = _readbackArray[0];
        DecodeCount++;
    }

    public void RequestDecodeAsync(Texture source)
    {
        DispatchDecoder(source);

        // Async readback request
        AsyncGPUReadback.Request(DecodeBuffer, OnReadback);
    }

    #endregion

    #region Private members

    GraphicsBuffer _decodeBuffer;

    GraphicsBuffer DecodeBuffer
      => _decodeBuffer ??
           (_decodeBuffer = GfxUtil.StructuredBuffer(12, sizeof(float)));

    void DispatchDecoder(Texture source)
    {
        _shader.SetTexture(0, "Source", source);
        _shader.SetBuffer(0, "Output", DecodeBuffer);
        _shader.Dispatch(0, 1, 1, 1);
    }

    Metadata[] _readbackArray = new Metadata[1];

    void OnReadback(AsyncGPUReadbackRequest req)
    {
        if (!req.hasError) Metadata = req.GetData<Metadata>()[0];
        DecodeCount++;
    }

    #endregion

    #region MonoBehaviour implementation

    void OnDisable() => OnDestroy();

    void OnDestroy()
    {
        _decodeBuffer?.Dispose();
        _decodeBuffer = null;
    }

    #endregion
}

} // namespace Bibcam.Decoder
