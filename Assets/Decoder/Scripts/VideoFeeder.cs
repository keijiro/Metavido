using UnityEngine;
using UnityEngine.Video;

namespace Bibcam.Decoder {

sealed class VideoFeeder : MonoBehaviour
{
    #region Scene object reference

    [SerializeField] MetadataDecoder _decoder = null;
    [SerializeField] TextureDemuxer _demuxer = null;

    #endregion

    #region MonoBehaviour implementation

    void Update()
    {
        var video = GetComponent<VideoPlayer>();
        if (video.texture == null) return;
        _decoder.Decode(video.texture);
        _demuxer.Demux(video.texture, _decoder.Metadata);
    }

    #endregion
}

} // namespace Bibcam.Decoder
