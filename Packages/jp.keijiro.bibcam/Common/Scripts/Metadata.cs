using UnityEngine;

namespace Bibcam.Common {

//
// Burnt-in metadata
//
// Burnt-in data area has 4x4 float capacity, so we use Matrix4x4 as a storage.
//
public readonly struct Metadata
{
    #region Private data

    readonly Matrix4x4 _data;

    #endregion

    #region Public accessors

    public Matrix4x4 AsMatrix
      => _data;

    public bool IsValid
      => _data.m21 != 0;

    public Vector3 CameraPosition
      => new Vector3(_data.m00, _data.m10, _data.m20);

    public Quaternion CameraRotation
      => ReconstructRotation(_data.m30, _data.m01, _data.m11);

    public Matrix4x4 ProjectionMatrix
      => ReconstructProjectionMatrix(_data);

    public float MinDepth
      => _data.m03;

    public float MaxDepth
      => _data.m13;

    public Vector2 DepthRange
      => new Vector2(_data.m03, _data.m13);

    #endregion

    #region Public constructors

    public Metadata(in Matrix4x4 source)
      => _data = source;

    public Metadata(Transform camera, in Matrix4x4 projection,
                    float minDepth, float maxDepth)
      => _data = PackData(camera.position, camera.rotation.normalized,
                          projection, minDepth, maxDepth);

    public Metadata(Transform camera, in Matrix4x4 projection,
                    Vector2 depthRange)
      => _data = PackData(camera.position, camera.rotation.normalized,
                          projection, depthRange.x, depthRange.y);

    #endregion

    #region Private helper functions

    static Quaternion ReconstructRotation(float x, float y, float z)
      => new Quaternion(x, y, z, Mathf.Sqrt(1 - x * x - y * y - z * z));

    static Matrix4x4 ReconstructProjectionMatrix(in Matrix4x4 data)
    {
        var m = default(Matrix4x4);

        m.m00 = data.m21;
        m.m10 = 0;
        m.m20 = 0;
        m.m30 = 0;

        m.m01 = 0;
        m.m11 = data.m02;
        m.m21 = 0;
        m.m31 = 0;

        m.m02 = data.m31;
        m.m12 = data.m12;
        m.m22 = data.m22;
        m.m32 = -1;

        m.m03 = 0;
        m.m13 = 0;
        m.m23 = data.m32;
        m.m33 = 0;

        return m;
    }

    static Matrix4x4 PackData(in Vector3 position, in Quaternion rotation,
                              in Matrix4x4 projection,
                              float minDepth, float maxDepth)
    {
        var rsign = rotation.w < 0 ? -1.0f : 1.0f;
        var m = new Matrix4x4();

        m.m00 = position.x;
        m.m10 = position.y;
        m.m20 = position.z;
        m.m30 = rotation.x * rsign;

        m.m01 = rotation.y * rsign;
        m.m11 = rotation.z * rsign;
        m.m21 = projection.m00;
        m.m31 = projection.m02;

        m.m02 = projection.m11;
        m.m12 = projection.m12;
        m.m22 = projection.m22;
        m.m32 = projection.m23;

        m.m03 = minDepth;
        m.m13 = maxDepth;
        m.m23 = Random.value;
        m.m33 = Random.value;

        return m;
    }

    #endregion
}

} // namespace Bibcam.Common
