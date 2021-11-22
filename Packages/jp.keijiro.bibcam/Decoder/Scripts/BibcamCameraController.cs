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

        // Projection matrices
        var cp = camera.projectionMatrix;   // from camera
        var mp = meta.ProjectionMatrix;     // from metadata

        // Aspect ratio conversion
        if (camera.aspect < 16.0f / 9)
            mp[1, 1] = mp[0, 0] * camera.aspect;
        else
            mp[0, 0] = mp[1, 1] / camera.aspect;

        // Copy depth-dependent elements from the camera.
        mp[2, 2] = cp[2, 2];
        mp[2, 3] = cp[2, 3];

        // Camera update
        transform.position = meta.CameraPosition;
        transform.rotation = meta.CameraRotation;
        camera.projectionMatrix = mp;
    }

    #endregion
}

} // namespace Bibcam.Decoder
