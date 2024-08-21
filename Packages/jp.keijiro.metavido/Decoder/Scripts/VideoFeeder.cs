using UnityEngine;
#if METAVIDO_HAS_UNITY_VIDEO
using UnityEngine.Video;
#endif

namespace Metavido.Decoder {

[AddComponentMenu("Metavido/Decoding/Metavido Video Feeder")]
public sealed class VideoFeeder : MonoBehaviour
{
#if METAVIDO_HAS_UNITY_VIDEO

    #region Scene object reference

    [SerializeField] MetadataDecoder _decoder = null;
    [SerializeField] TextureDemuxer _demuxer = null;
    [SerializeField] bool _asynchronous = true;

    #endregion

    #region MonoBehaviour implementation

    FrameFeeder _feeder;

    void OnDestroy()
    {
        _feeder?.Dispose();
        _feeder = null;
    }

    void Update()
    {
        var player = GetComponent<VideoPlayer>();
        if (player.texture == null) return;

        if (_asynchronous)
        {
            // Async mode: Use FrameFeeder.
            _feeder = _feeder ?? new FrameFeeder(_decoder, _demuxer);
            _feeder.AddFrame(player.texture);
            _feeder.Update();
        }
        else
        {
            // Sync mode: Simply decode and demux.
            _decoder.DecodeSync(player.texture);
            _demuxer.Demux(player.texture, _decoder.Metadata);
            return;
        }
    }

    #endregion

#else

    void OnValidate()
      => Debug.LogError("UnityEngine.Video is missing.");

#endif
}

} // namespace Metavido.Decoder
