using UnityEngine;
using Bibcam.Common;

namespace Bibcam.Decoder {

[RequireComponent(typeof(Camera))]
public sealed class BibcamCameraController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] bool _interpolation = false;

    #endregion

    #region Private members

    // External component reference
    Camera _camera;

    // Keyframes for interpolation
    (Vector3 p, Quaternion r, float t, float hash) _key1, _key2;

    // Transform update without interpolation
    void UpdateTransform(in Metadata meta)
    {
        transform.position = meta.CameraPosition;
        transform.rotation = meta.CameraRotation;
    }

    // Transform update with interpolation
    void UpdateTransformLerped(in Metadata meta)
    {
        // Current time, extrapolated time of next frame
        var (t, nt) = (Time.time, Time.time + Time.deltaTime);

        // Keyframe update
        if (_key2.hash != meta.Hash)
        {
            _key1 = _key2;
            _key2 = (meta.CameraPosition, meta.CameraRotation, t, meta.Hash);
        }

        // Interpolation parameter
        var ip = Mathf.Clamp01((nt - _key2.t) / (_key2.t - _key1.t));

        // Transform update
        transform.position = Vector3.Lerp(_key1.p, _key2.p, ip);
        transform.rotation = Quaternion.Slerp(_key1.r, _key2.r, ip);
    }

    // Camera matrix update
    void UpdateCameraMatrix(in Metadata meta)
      => _camera.projectionMatrix =
           meta.ReconstructProjectionMatrix(_camera.projectionMatrix);

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _camera = GetComponent<Camera>();

    void LateUpdate()
    {
        var meta = _decoder.Metadata;
        if (!meta.IsValid) return;

        if (_interpolation)
            UpdateTransformLerped(meta);
        else
            UpdateTransform(meta);

        UpdateCameraMatrix(meta);
    }

    #endregion
}

} // namespace Bibcam.Decoder
