using UnityEngine;

namespace Bibcam.Decoder {

sealed class CameraController : MonoBehaviour
{
    [SerializeField] MetadataDecoder _decoder = null;

    void LateUpdate()
    {
        var meta = _decoder.Metadata;
        if (!meta.IsValid) return;
        transform.position = meta.CameraPosition;
        transform.rotation = meta.CameraRotation;
        GetComponent<Camera>().projectionMatrix = meta.ProjectionMatrix;
    }
}

} // namespace Bibcam.Decoder
