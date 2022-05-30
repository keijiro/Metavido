using UnityEngine;
#if BIBCAM_HAS_UNITY_VIDEO
using UnityEngine.Video;
#endif

namespace Bibcam.Decoder {

sealed class BibcamVideoFeeder : MonoBehaviour
{
#if BIBCAM_HAS_UNITY_VIDEO

    #region Scene object reference

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demuxer = null;
    [SerializeField] bool _asynchronous = true;

    #endregion

    #region MonoBehaviour implementation

    BibcamFrameFeeder _feeder;

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
            _feeder = _feeder ?? new BibcamFrameFeeder(_decoder, _demuxer);
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

} // namespace Bibcam.Decoder
