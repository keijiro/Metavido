using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Bibcam {

sealed class Controller : MonoBehaviour
{
    #region External scene object references

    [Space]
    [SerializeField] Camera _camera = null;
    [SerializeField] ARCameraManager _cameraManager = null;
    [SerializeField] AROcclusionManager _occlusionManager = null;
    [SerializeField] RawImage _mainView = null;

    #endregion

    #region Editable parameters

    [Space]
    [SerializeField] float _minDepth = 0.2f;
    [SerializeField] float _maxDepth = 3.2f;

    #endregion

    #region Hidden external asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Internal objects

    Matrix4x4 _projection;
    Material _material;
    RenderTexture _buffer;

    #endregion

    #region Public methods (UI callback)

    public void ResetOrigin()
      => _camera.transform.parent.position = -_camera.transform.localPosition;

    #endregion

    #region AR foundation callbacks

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        // No operation for no texture
        if (args.textures.Count == 0) return;

        // Y/CbCr textures
        for (var i = 0; i < args.textures.Count; i++)
        {
            var id = args.propertyNameIds[i];
            var tex = args.textures[i];
            if (id == ShaderID.TextureY)
                _material.SetTexture(ShaderID.TextureY, tex);
            else if (id == ShaderID.TextureCbCr)
                _material.SetTexture(ShaderID.TextureCbCr, tex);
        }

        // Projection matrix
        if (args.projectionMatrix.HasValue)
        {
            _projection = args.projectionMatrix.Value;
            // Aspect ratio compensation (camera vs. 16:9)
            _projection[1, 1] *= (16.0f / 9) / _camera.aspect;
        }

        // Source texture aspect ratio
        var tex1 = args.textures[0];
        var texAspect = (float)tex1.width / tex1.height;

        // Aspect ratio compensation factor for the multiplexer
        var aspectFix = texAspect / (16.0f / 9);
        _material.SetFloat(ShaderID.AspectFix, aspectFix);
    }

    void OnOcclusionFrameReceived(AROcclusionFrameEventArgs args)
    {
        // Stencil/depth textures.
        for (var i = 0; i < args.textures.Count; i++)
        {
            var id = args.propertyNameIds[i];
            var tex = args.textures[i];
            if (id == ShaderID.HumanStencil)
                _material.SetTexture(ShaderID.HumanStencil, tex);
            else if (id == ShaderID.EnvironmentDepth)
                _material.SetTexture(ShaderID.EnvironmentDepth, tex);
        }
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Shader setup
        _material = new Material(_shader);

        // Multiplexing buffer
        _buffer = new RenderTexture(Screen.width, Screen.height, 0);
        _buffer.Create();

        // UI initialization
        _mainView.texture = _buffer;
    }

    void OnDestroy()
    {
        Destroy(_material);
        Destroy(_buffer);
    }

    void OnEnable()
    {
        // Camera callback setup
        _cameraManager.frameReceived += OnCameraFrameReceived;
        _occlusionManager.frameReceived += OnOcclusionFrameReceived;
    }

    void OnDisable()
    {
        // Camera callback termination
        _cameraManager.frameReceived -= OnCameraFrameReceived;
        _occlusionManager.frameReceived -= OnOcclusionFrameReceived;
    }

    void Update()
    {
        _material.SetVector
          (ShaderID.DepthRange, new Vector2(_minDepth, _maxDepth));
        _material.SetMatrix
          (ShaderID.Metadata, new Metadata(_camera.transform, _projection,
                                           _minDepth, _maxDepth).AsMatrix);
        Graphics.Blit(null, _buffer, _material);
    }

    #endregion
}

} // namespace Bibcam
