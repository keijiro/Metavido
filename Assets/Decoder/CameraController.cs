using UnityEngine;

namespace Bibcam {

sealed class CameraController : MonoBehaviour
{
    [SerializeField] Camera _camera = null;
    [SerializeField] Decoder _decoder = null;

    void Update()
    {
        _camera.transform.position = _decoder.CameraPosition;
        _camera.transform.rotation = _decoder.CameraRotation;
        _camera.projectionMatrix = _decoder.ProjectionMatrix;
    }
}

} // namespace Bibcam
