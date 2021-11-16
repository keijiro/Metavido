using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Decoder {

public static class BibcamRenderUtils
{
    #region UV to ray conversion parameters

    public static Vector4 RayParams(Matrix4x4 pm)
      => new Vector4(pm[0, 2], pm[1, 2], 1 / pm[0, 0], 1 / pm[1, 1]);

    public static Vector4 RayParams(Camera camera)
      => RayParams(camera.projectionMatrix);

    public static Vector4 RayParams(in Metadata meta)
      => RayParams(meta.ProjectionMatrix);

    #endregion

    #region Inverse view matrix

    public static Matrix4x4 InverseView(Vector3 position, Quaternion rotation)
      => Matrix4x4.TRS(position, rotation, Vector3.one);

    public static Matrix4x4 InverseView(Camera camera)
      => InverseView(camera.transform.position, camera.transform.rotation);

    public static Matrix4x4 InverseView(in Metadata meta)
      => InverseView(meta.CameraPosition, meta.CameraRotation);

    #endregion
}

} // namespace Bibcam.Decoder
