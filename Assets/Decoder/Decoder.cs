using UnityEngine;
using UnityEngine.Video;

namespace Bibcam {

sealed class Decoder : MonoBehaviour
{
    #region External scene object references

    [Space]
    [SerializeField] VideoPlayer _source = null;
    [SerializeField] Camera _camera = null;

    #endregion

    #region Hidden external asset references

    [SerializeField, HideInInspector] ComputeShader _shader = null;

    #endregion

    #region Private objects

    Texture2D _texture;
    GraphicsBuffer _buffer;
    Matrix4x4[] _readback;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _buffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, 16, sizeof(float));
        _readback = new Matrix4x4[1];
    }

    void OnDestroy()
    {
        if (_texture != null) Destroy(_texture);
        if (_buffer != null) _buffer.Dispose();
    }

    void Update()
    {
        var src = _source.texture;
        if (src == null) return;

        if (_texture == null)
            _texture = new Texture2D
              (src.width, src.height, TextureFormat.RGBA32, false);

        Graphics.CopyTexture(src, _texture);

        _shader.SetTexture(0, "Source", _texture);
        _shader.SetBuffer(0, "Output", _buffer);
        _shader.Dispatch(0, 1, 1, 1);
        _buffer.GetData(_readback);

        var meta = new Metadata(_readback[0]);
        _camera.transform.position = meta.CameraPosition;
        _camera.transform.rotation = meta.CameraRotation;
        _camera.projectionMatrix = meta.ProjectionMatrix;
    }

    #endregion
}

} // namespace Bibcam
