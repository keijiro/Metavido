// Fixed aspect ratio element from Unity Manual
// https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-create-aspect-ratios-custom-control.html

using UnityEngine;
using UnityEngine.UIElements;

namespace Bibcam.UI {

[UxmlElement]
public partial class AspectRatioElement : VisualElement
{
    [UxmlAttribute("width"), Range(1, 100)]
    public int ratioWidth
      { get => _ratio.x; set => UpdateAspect(value, -1); }

    [UxmlAttribute("height"), Range(1, 100)]
    public int ratioHeight
      { get => _ratio.y; set => UpdateAspect(-1, value); }

    (int x, int y) _ratio = (16, 9);

    public AspectRatioElement()
    {
        RegisterCallback<GeometryChangedEvent>(UpdateAspectAfterEvent);
        RegisterCallback<AttachToPanelEvent>(UpdateAspectAfterEvent);
    }

    static void UpdateAspectAfterEvent(EventBase e)
      => (e.target as AspectRatioElement)?.UpdateAspect(-1, -1);

    void UpdateAspect(int x, int y)
    {
        _ratio.x = (x > 0) ? Mathf.Max(1, x) : _ratio.x;
        _ratio.y = (y > 0) ? Mathf.Max(1, y) : _ratio.y;

        var resolved = (x: resolvedStyle.width, y: resolvedStyle.height);
        var designRatio = (float)_ratio.x / _ratio.y;
        var currRatio = resolved.x / resolved.y;
        var diff = currRatio - designRatio;

        if (diff > 0)
        {
            var pad = resolved.x - (resolved.y * designRatio);
            style.paddingLeft = style.paddingRight = pad / 2;
            style.paddingTop = style.paddingBottom = 0;
        }
        else if (diff < 0)
        {
            var pad = resolved.y - (resolved.x / designRatio);
            style.paddingLeft = style.paddingRight = 0;
            style.paddingTop = style.paddingBottom = pad / 2;
        }
        else
        {
            style.paddingLeft = style.paddingRight = 0;
            style.paddingBottom = style.paddingTop = 0;
        }
    }
}

} // namespace Bibcam.UI
