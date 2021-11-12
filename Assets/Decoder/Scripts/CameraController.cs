using UnityEngine;

namespace Bibcam.Decoder {

sealed class CameraController : MonoBehaviour
{
    [SerializeField] MetadataDecoder _decoder = null;

    void Update()
    {
        transform.position = _decoder.CameraPosition;
        transform.rotation = _decoder.CameraRotation;
        GetComponent<Camera>().projectionMatrix = _decoder.ProjectionMatrix;
    }
}

} // namespace Bibcam.Decoder
