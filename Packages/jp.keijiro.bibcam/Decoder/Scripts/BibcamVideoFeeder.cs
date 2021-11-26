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

#else

    void OnValidate()
      => Debug.LogError("UnityEngine.Video is missing.");

#endif
}

} // namespace Bibcam.Decoder
