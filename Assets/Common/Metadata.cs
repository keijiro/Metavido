using UnityEngine;

namespace Bibcam {

readonly struct Metadata
{
    #region Private data

    readonly Matrix4x4 _data;

    #endregion

    #region Accessors

    public Matrix4x4 AsMatrix => _data;

    public Vector3 CameraPosition
      => new Vector3(_data.m00, _data.m10, _data.m20);

    public Quaternion CameraRotation
      => MathUtil.NormalizedRotation
           (new Vector3(_data.m30, _data.m01, _data.m11));

    public Matrix4x4 ProjectionMatrix
      => ReconstructProjectionMatrix();

    public float MinDepth => _data.m03;
    public float MaxDepth => _data.m13;

    #endregion

    #region Constructors

    public Metadata(in Matrix4x4 source)
      => _data = source;

    public Metadata
      (Transform camera, in Matrix4x4 projection,
       float minDepth, float maxDepth)
    {
        var p = camera.position;
        var r = camera.rotation.normalized;
        var rs = r.w < 0 ? -1.0f : 1.0f;

        _data = default(Matrix4x4);

        _data.m00 = p.x;
        _data.m10 = p.y;
        _data.m20 = p.z;
        _data.m30 = r.x * rs;

        _data.m01 = r.y * rs;
        _data.m11 = r.z * rs;
        _data.m21 = projection.m00;
        _data.m31 = projection.m02;

        _data.m02 = projection.m11;
        _data.m12 = projection.m12;
        _data.m22 = projection.m22;
        _data.m32 = projection.m23;

        _data.m03 = minDepth;
        _data.m13 = maxDepth;
        _data.m23 = Random.value;
        _data.m33 = Random.value;
    }

    #endregion

    #region Private helper functions

    Matrix4x4 ReconstructProjectionMatrix()
    {
        var m = default(Matrix4x4);

        m.m00 = _data.m21;
        m.m10 = 0;
        m.m20 = 0;
        m.m30 = 0;

        m.m01 = 0;
        m.m11 = _data.m02;
        m.m21 = 0;
        m.m31 = 0;

        m.m02 = _data.m31;
        m.m12 = _data.m12;
        m.m22 = _data.m22;
        m.m32 = -1;

        m.m03 = 0;
        m.m13 = 0;
        m.m23 = _data.m32;
        m.m33 = 0;

        return m;
    }

    #endregion
}

} // namespace Bibcam
