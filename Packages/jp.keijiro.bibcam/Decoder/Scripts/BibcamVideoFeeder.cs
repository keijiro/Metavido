using UnityEngine;

#if BIBCAM_HAS_UNITY_VIDEO
using UnityEngine.Video;
#endif

#if BIBCAM_HAS_KLAK_HAP
using Klak.Hap;
#endif

#pragma warning disable CS0414

namespace Bibcam.Decoder {

sealed class BibcamVideoFeeder : MonoBehaviour
{
    #region Scene object reference

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demuxer = null;

    #endregion

    #region MonoBehaviour implementation

    RenderTexture _rt;

    void Start()
    {
        _rt = RenderTexture.GetTemporary(1920, 1080);

#if BIBCAM_HAS_UNITY_VIDEO
        var video = GetComponent<VideoPlayer>();
        if (video != null)
        {
            video.renderMode = VideoRenderMode.RenderTexture;
            video.targetTexture = _rt;
        }
#endif

#if BIBCAM_HAS_KLAK_HAP
        var hap = GetComponent<HapPlayer>();
        if (hap != null) hap.targetTexture = _rt;
#endif
    }

    void OnDestroy()
      => RenderTexture.ReleaseTemporary(_rt);

    void Update()
    {
        _decoder.Decode(_rt);
        _demuxer.Demux(_rt, _decoder.Metadata);
    }

    #endregion
}

} // namespace Bibcam.Decoder
