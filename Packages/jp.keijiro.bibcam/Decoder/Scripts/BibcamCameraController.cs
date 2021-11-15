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
        transform.position = meta.CameraPosition;
        transform.rotation = meta.CameraRotation;
        GetComponent<Camera>().projectionMatrix = meta.ProjectionMatrix;
    }

    #endregion
}

} // namespace Bibcam.Decoder
