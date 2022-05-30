using System.Collections.Generic;
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

    #region Frame readback queue

    Queue<(RenderTexture rt, int index)> _queue
      = new Queue<(RenderTexture rt, int index)>();

    int _count;

    #endregion

    #region MonoBehaviour implementation

    void OnDestroy()
    {
        while (_queue.Count > 0)
            RenderTexture.ReleaseTemporary(_queue.Dequeue().rt);
    }

    void Update()
    {
        var player = GetComponent<VideoPlayer>();
        if (player.texture == null) return;

        if (!_asynchronous)
        {
            // Sync pass: Simply decode and demux.
            _decoder.DecodeSync(player.texture);
            _demuxer.Demux(player.texture, _decoder.Metadata);
            return;
        }

        // Async pass:

        // Source texture copy into a temporary RT
        var video = player.texture;
        var tempRT = RenderTexture.GetTemporary(video.width, video.height);
        Graphics.CopyTexture(video, tempRT);

        // Decode queuing
        _decoder.RequestDecodeAsync(tempRT);
        _queue.Enqueue((tempRT, _count++));

        // Decoder progress check
        if (_decoder.DecodeCount <= _queue.Peek().index) return;

        // Skipped frame disposal
        while (_queue.Peek().index < _decoder.DecodeCount - 1)
            RenderTexture.ReleaseTemporary(_queue.Dequeue().rt);

        // Demuxing with latest decoded frame
        var decoded = _queue.Dequeue().rt;
        _demuxer.Demux(decoded, _decoder.Metadata);
        RenderTexture.ReleaseTemporary(decoded);
    }

    #endregion

#else

    void OnValidate()
      => Debug.LogError("UnityEngine.Video is missing.");

#endif
}

} // namespace Bibcam.Decoder
