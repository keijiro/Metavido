using UnityEngine;

namespace Bibcam.Decoder {

[RequireComponent(typeof(Camera))]
public sealed class BibcamCameraController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] BibcamMetadataDecoder _decoder = null;

    #endregion

    #region MonoBehaviour implementation

    void LateUpdate()
    {
        var meta = _decoder.Metadata;
        if (!meta.IsValid) return;

        var camera = GetComponent<Camera>();

        // Projection matrix with aspect ratio conversion
        var proj = meta.ProjectionMatrix;
        if (camera.aspect < 16.0f / 9)
            proj[1, 1] = proj[0, 0] * camera.aspect;
        else
            proj[0, 0] = proj[1, 1] / camera.aspect;

        // Transform update
        transform.position = meta.CameraPosition;
        transform.rotation = meta.CameraRotation;

        // Camera parameter update
        camera.projectionMatrix = proj;
    }

    #endregion
}

} // namespace Bibcam.Decoder
