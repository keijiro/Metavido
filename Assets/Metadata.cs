using UnityEngine;

namespace Bibcam {

readonly struct Metadata
{
    readonly Matrix4x4 _data;

    public Matrix4x4 AsMatrix => _data;

    public Metadata
      (Transform camera, in Matrix4x4 projection,
       float minDepth, float maxDepth)
    {
        var p = camera.position;
        var r = camera.rotation;

        _data = default(Matrix4x4);

        _data[0, 0] = p.x;
        _data[0, 1] = p.y;
        _data[0, 2] = p.z;
        _data[0, 3] = r.x;

        _data[1, 0] = r.y;
        _data[1, 1] = r.z;
        _data[1, 2] = projection[0, 0];
        _data[1, 3] = projection[0, 2];

        _data[2, 0] = projection[1, 1];
        _data[2, 1] = projection[1, 2];
        _data[2, 2] = projection[2, 2];
        _data[2, 3] = projection[2, 3];

        _data[3, 0] = minDepth;
        _data[3, 1] = maxDepth;
        _data[3, 2] = Random.value;
        _data[3, 3] = Random.value;
    }
}

} // namespace Bibcam
