using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bibcam.Decoder {

public sealed class BibcamFrameFeeder : IDisposable
{
    #region Private members

    (BibcamMetadataDecoder decoder, BibcamTextureDemuxer demuxer) _target;

    Queue<(RenderTexture rt, int index)> _queue
      = new Queue<(RenderTexture rt, int index)>();

    int _count;

    #endregion

    #region Public methods

    public BibcamFrameFeeder
      (BibcamMetadataDecoder decoder, BibcamTextureDemuxer demuxer)
      => _target = (decoder, demuxer);

    public void Dispose()
    {
        while (_queue.Count > 0)
            RenderTexture.ReleaseTemporary(_queue.Dequeue().rt);
    }

    public void AddFrame(Texture source)
    {
        // Source texture copy into a temporary RT
        var tempRT = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.CopyTexture(source, tempRT);

        // Decode queuing
        _target.decoder.RequestDecodeAsync(tempRT);
        _queue.Enqueue((tempRT, _count++));
    }

    public void Update()
    {
        // Decoder progress check
        if (_target.decoder.DecodeCount <= _queue.Peek().index) return;

        // Skipped frame disposal
        while (_queue.Peek().index < _target.decoder.DecodeCount - 1)
            RenderTexture.ReleaseTemporary(_queue.Dequeue().rt);

        // Demuxing with latest decoded frame
        var decoded = _queue.Dequeue().rt;
        _target.demuxer.Demux(decoded, _target.decoder.Metadata);
        RenderTexture.ReleaseTemporary(decoded);
    }

    #endregion
}

} // namespace Bibcam.Decoder
