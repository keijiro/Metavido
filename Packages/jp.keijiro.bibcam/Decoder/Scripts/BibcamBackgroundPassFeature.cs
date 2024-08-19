#if BIBCAM_HAS_URP

using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Bibcam.Decoder {

sealed class BibcamBackgroundRenderPass : ScriptableRenderPass
{
    class PassData { public BibcamBackground Driver { get; set; } }

    public override void RecordRenderGraph(RenderGraph graph,
                                           ContextContainer context)
    {
        // Play mode not supported
        if (!UnityEngine.Application.isPlaying) return;

        // BibcamBackground component reference
        var camera = context.Get<UniversalCameraData>().camera;
        var driver = camera.GetComponent<BibcamBackground>();
        if (driver == null || !driver.enabled || !driver.IsReady) return;

        // Render pass building
        using var builder =
          graph.AddRasterRenderPass<PassData>("Bibcam BG", out var data);

        data.Driver = driver;

        var resource = context.Get<UniversalResourceData>();
        builder.SetRenderAttachment(resource.activeColorTexture, 0);
        builder.SetRenderAttachmentDepth(resource.activeDepthTexture,
                                         AccessFlags.Write);

        builder.AllowPassCulling(false);

        // Render function registration
        builder.SetRenderFunc((PassData data, RasterGraphContext context)
                                => data.Driver.PushDrawCommand(context));
    }
}

public sealed class BibcamBackgroundPassFeature : ScriptableRendererFeature
{
    BibcamBackgroundRenderPass _pass;

    public override void Create()
      => _pass = new BibcamBackgroundRenderPass
           { renderPassEvent = RenderPassEvent.AfterRenderingOpaques };

    public override void AddRenderPasses(ScriptableRenderer renderer,
                                         ref RenderingData renderingData)
      => renderer.EnqueuePass(_pass);
}

} // namespace Bibcam.Decoder

#endif
