using UnityEngine;

namespace Bibcam.Common {

// Burnt-in metadata
public readonly struct Metadata
{
    #region Private data

    readonly float _px;
    readonly float _py;
    readonly float _pz;

    readonly float _rx;
    readonly float _ry;
    readonly float _rz;

    readonly float _sx;
    readonly float _sy;
    readonly float _fov;

    readonly float _near;
    readonly float _far;
    readonly float _hash;

    #endregion

    #region Public accessors

    public bool IsValid
      => _fov != 0;

    public Vector3 CameraPosition
      => new Vector3(_px, _py, _pz);

    public Quaternion CameraRotation
      => new Quaternion(_rx, _ry, _rz, RW);

    public Vector2 CenterShift
      => new Vector2(_sx, _sy);

    public float FieldOfView
      => _fov;

    public Vector2 DepthRange
      => new Vector2(_near, _far);

    public float Hash
      => _hash;

    #endregion

    #region Public constructors

    public Metadata(Transform camera, in Matrix4x4 projection, Vector2 range)
    {
        var p = camera.position;
        var r = camera.rotation;
        var rsign = r.w < 0 ? -1.0f : 1.0f;

        _px = p.x;
        _py = p.y;
        _pz = p.z;

        _rx = r.x * rsign;
        _ry = r.y * rsign;
        _rz = r.z * rsign;

        _sx = projection.m02;
        _sy = projection.m12;
        _fov = Mathf.Atan(1 / projection.m11) * 2;

        _near = range.x;
        _far  = range.y;
        _hash = Random.value;
    }

    #endregion

    #region Projection matrix reconstruction

    public Matrix4x4 ReconstructProjectionMatrix(in Matrix4x4 source)
    {
        var m = source;
        var aspect = source.m11 / source.m00;

        if (aspect > 16.0f / 9)
        {
            // Wider than 16:9; Adjust the horizontal FoV.
            m.m11 = 1 / Mathf.Tan(_fov / 2);
            m.m00 = m.m11 / aspect;
        }
        else
        {
            // Narrower than 16:9; Adjust the vertical FoV.
            m.m00 = 9.0f / 16 / Mathf.Tan(_fov / 2);
            m.m11 = m.m00 * aspect;
        }

        m.m02 = _sx;
        m.m12 = _sy;

        return m;
    }

    #endregion

    #region Private helpers

    float RW
      => Mathf.Sqrt(1 - _rx * _rx - _ry * _ry - _rz * _rz);

    #endregion
}

} // namespace Bibcam.Common
