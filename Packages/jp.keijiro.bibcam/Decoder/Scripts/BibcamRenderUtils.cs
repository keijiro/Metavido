using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Decoder {

public static class BibcamRenderUtils
{
    #region UV to ray conversion parameters

    public static Vector4 RayParams(in Metadata meta)
    {
        var s = meta.CenterShift;
        var h = Mathf.Tan(meta.FieldOfView / 2);
        return new Vector4(s.x, s.y, h * 16 / 9, h);
    }

    #endregion

    #region Inverse view matrix

    public static Matrix4x4 InverseView(in Metadata meta)
      => Matrix4x4.TRS(meta.CameraPosition, meta.CameraRotation, Vector3.one);

    #endregion
}

} // namespace Bibcam.Decoder
